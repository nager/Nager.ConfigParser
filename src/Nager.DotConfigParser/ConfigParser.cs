using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nager.DotConfigParser
{
    public class ConfigParser
    {
        private readonly char _splitChar;
        private readonly Dictionary<Type, BaseParserUnit> _parserUnits = new Dictionary<Type, BaseParserUnit>();

        public ConfigParser(char splitChar = '=')
        {
            this._splitChar = splitChar;

            var parserUnits = this.GetBaseParserUnits();
            foreach (var parserUnit in parserUnits)
            {
                this._parserUnits.Add(parserUnit.ParserUnitType, parserUnit);
            }
        }

        private IEnumerable<BaseParserUnit> GetBaseParserUnits()
        {
            return typeof(BaseParserUnit)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseParserUnit)) && !t.IsAbstract)
            .Select(t => (BaseParserUnit)Activator.CreateInstance(t));
        }

        public T DeserializeObject<T>(string value) where T : new()
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            var configLines = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (configLines.Length == 0)
            {
                return default;
            }

            var configurations = configLines.Select(o => o.Split(new char[] { this._splitChar }, 2, StringSplitOptions.RemoveEmptyEntries))
                .Select(o => new Configuration(o.FirstOrDefault(), o.LastOrDefault()));

            var item = new T();

            var properties = item.GetType().GetProperties();
            foreach (var property in properties)
            {
                var key = property.Name.ToLower();

                if (Attribute.IsDefined(property, typeof(ConfigKeyAttribute)))
                {
                    var attribute = (ConfigKeyAttribute[])property.GetCustomAttributes(typeof(ConfigKeyAttribute), false);
                    key = attribute.FirstOrDefault().Key;
                }

                #region Array logic

                if (Attribute.IsDefined(property, typeof(ConfigArrayAttribute)))
                {
                    var filteredConfigurations = configurations.Where(o => o.Key.StartsWith(key))
                        .Select(o => new Configuration(o.Key.Remove(0, key.Length), o.Data))
                        .ToList();

                    var groupByIndexConfigurations = filteredConfigurations.Select(o =>
                    {
                        var splittedKey = o.Key.Split(new char[] { '.' }, 2);
                        return new
                        {
                            Index = splittedKey[0],
                            Key = o.Key.Remove(0, splittedKey[0].Length + 1),
                            o.Data
                        };
                    })
                    .GroupBy(o => o.Index, o => new { o.Key, o.Data });

                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(property.PropertyType.GetElementType());
                    var listContainer = Activator.CreateInstance(constructedListType);

                    foreach (var childConfigurations in groupByIndexConfigurations)
                    {
                        var elementType = property.PropertyType.GetElementType();
                        var instance = Activator.CreateInstance(elementType);

                        var childProperties = instance.GetType().GetProperties();
                        foreach (var childProperty in childProperties)
                        {
                            if (childProperty.Name == "ConfigArrayIndex")
                            {
                                childProperty.SetValue(instance, childConfigurations.Key);
                                continue;
                            }

                            var childKey = childProperty.Name;
                            if (Attribute.IsDefined(childProperty, typeof(ConfigKeyAttribute)))
                            {
                                var attribute = (ConfigKeyAttribute[])childProperty.GetCustomAttributes(typeof(ConfigKeyAttribute), false);
                                childKey = attribute.FirstOrDefault().Key;
                            }

                            var childConfiguration = childConfigurations.ToList().Where(o => o.Key.Equals(childKey, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (childConfiguration == null)
                            {
                                continue;
                            }

                            childProperty.SetValue(instance, childConfiguration.Data);
                        }

                        listContainer.GetType().GetMethod("Add").Invoke(listContainer, new[] { instance });
                    }

                    #region Convert list to array

                    MethodInfo toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
                        .MakeGenericMethod(new System.Type[] { property.PropertyType.GetElementType() });

                    var castedObject = toArrayMethod.Invoke(null, new object[] { listContainer });

                    #endregion

                    property.SetValue(item, castedObject);
                    continue;
                }

                #endregion

                var configuration = configurations.Where(o => o.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).LastOrDefault();
                if (configuration == null)
                {
                    continue;
                }

                var parserUnit = this._parserUnits.Where(o => o.Key.Equals(property.PropertyType)).Select(o => o.Value).FirstOrDefault();
                if (parserUnit != null)
                {
                    var configData = parserUnit.Deserialize(configuration.Data);
                    property.SetValue(item, configData);
                    continue;
                }

                property.SetValue(item, configuration.Data);
            }

            return item;
        }

        public string SerializeObject<T>(T value)
        {
            return default;
        }
    }
}
