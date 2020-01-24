using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nager.ConfigParser
{
    public class ConfigConvert
    {
        private readonly ConfigConvertConfig _config;
        private readonly Dictionary<Type, BaseParserUnit> _parserUnits = new Dictionary<Type, BaseParserUnit>();

        public ConfigConvert(ConfigConvertConfig config = default, BaseParserUnit[] customParserUnits = null)
        {
            if (config == default)
            {
                config = new ConfigConvertConfig();
            }
            this._config = config;

            var parserUnits = this.GetBaseParserUnits();
            foreach (var parserUnit in parserUnits)
            {
                this._parserUnits.Add(parserUnit.ParserUnitType, parserUnit);
            }

            if (customParserUnits != null)
            {
                foreach (var parserUnit in customParserUnits)
                {
                    this._parserUnits.Add(parserUnit.ParserUnitType, parserUnit);
                }
            }
        }

        private IEnumerable<BaseParserUnit> GetBaseParserUnits()
        {
            return typeof(BaseParserUnit)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseParserUnit)) && !t.IsAbstract)
            .Select(t =>
            {
                var parameters = t.GetConstructors().FirstOrDefault().GetParameters();

                if (parameters.Length == 2)
                {
                    return (BaseParserUnit)Activator.CreateInstance(t, this._config.CultureInfo, this._config.ValueDelimiter);
                }
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(char))
                {
                    return (BaseParserUnit)Activator.CreateInstance(t, this._config.ValueDelimiter);
                }
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(CultureInfo))
                {
                    return (BaseParserUnit)Activator.CreateInstance(t, this._config.CultureInfo);
                }
                return (BaseParserUnit)Activator.CreateInstance(t);
            });
        }

        public T DeserializeObject<T>(string value) where T : new()
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            var configLines = value.Split(this._config.ConfigDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (configLines.Length == 0)
            {
                return default;
            }

            var configurations = configLines.Select(o => o.Split(new char[] { this._config.KeyValueDelimiter }, 2, StringSplitOptions.RemoveEmptyEntries))
                .Select(o => new Configuration(o.FirstOrDefault(), o.LastOrDefault()));

            var item = new T();

            var properties = item.GetType().GetProperties();
            foreach (var property in properties)
            {
                this.ReadValue(property, item, configurations);
            }

            return item;
        }

        private void ReadValue<T>(PropertyInfo property, T item, IEnumerable<Configuration> configurations)
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
                this.ProcessReadArray(property, key, item, configurations);
                return;
            }

            #endregion

            var configuration = configurations.Where(o => o.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).LastOrDefault();
            if (configuration == null)
            {
                return;
            }

            var parserUnit = this._parserUnits.Where(o => o.Key.Equals(property.PropertyType)).Select(o => o.Value).FirstOrDefault();
            if (parserUnit != null)
            {
                var configData = parserUnit.Deserialize(configuration.Data);
                property.SetValue(item, configData);
                return;
            }

            property.SetValue(item, configuration.Data);
        }

        private void ProcessReadArray<T>(PropertyInfo property, string key, T item, IEnumerable<Configuration> configurations)
        {
            //Filter only valid configurations
            var filteredConfigurations = configurations.Where(o => o.Key.StartsWith(key))
                   .Select(o => new Configuration(o.Key.Remove(0, key.Length), o.Data))
                   .ToList();

            //Get the configurations grouped by Id
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
            .GroupBy(o => o.Index, o => new Configuration(o.Key, o.Data));

            //Create a list object for add x elements
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
                    //Set the array index from the config
                    if (childProperty.Name == nameof(ConfigArrayElement.ConfigArrayIndex))
                    {
                        childProperty.SetValue(instance, childConfigurations.Key);
                        continue;
                    }

                    //Read the value from the config
                    this.ReadValue(childProperty, instance, childConfigurations);
                }

                listContainer.GetType().GetMethod("Add").Invoke(listContainer, new[] { instance });
            }

            #region Convert list to array

            MethodInfo toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
                .MakeGenericMethod(new System.Type[] { property.PropertyType.GetElementType() });

            var castedObject = toArrayMethod.Invoke(null, new object[] { listContainer });

            #endregion

            property.SetValue(item, castedObject);
            return;
        }

        public string SerializeObject<T>(T value)
        {
            var sb = new StringBuilder();

            var properties = value.GetType().GetProperties();
            foreach (var property in properties)
            {
                var configLine = this.WriteValue(property, value);
                if (configLine == null)
                {
                    continue;
                }
                sb.AppendLine(configLine);
            }

            return sb.ToString();
        }

        private string WriteValue<T>(PropertyInfo property, T value)
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
                return this.ProcessWriteArray(property, key, value);
            }

            #endregion

            var parserUnit = this._parserUnits.Where(o => o.Key.Equals(property.PropertyType)).Select(o => o.Value).FirstOrDefault();
            if (parserUnit != null)
            {
                var parserUnitConfigValue = parserUnit.Serialize(property.GetValue(value));
                if (parserUnitConfigValue == null)
                {
                    return null;
                }
                return $"{key}{this._config.KeyValueDelimiter}{parserUnitConfigValue}";
            }

            var configValue = property.GetValue(value);
            if (configValue == null)
            {
                return null;
            }

            return $"{key}{this._config.KeyValueDelimiter}{configValue}";
        }

        private string ProcessWriteArray<T>(PropertyInfo property, string key, T value)
        {
            var items = (Array)property.GetValue(value);
            if (items == null)
            {
                return null;
            }

            var elementType = property.PropertyType.GetElementType();
            var instance = Activator.CreateInstance(elementType);

            var sb = new StringBuilder();

            foreach (var item in items)
            {
                var childProperties = instance.GetType().GetProperties();

                var arrayIndex = "0";
                foreach (var childProperty in childProperties)
                {
                    //Set the array index from the config
                    if (childProperty.Name == nameof(ConfigArrayElement.ConfigArrayIndex))
                    {
                        arrayIndex = childProperty.GetValue(item)?.ToString();
                        break;
                    }
                }

                foreach (var childProperty in childProperties)
                {
                    //Set the array index from the config
                    if (childProperty.Name == nameof(ConfigArrayElement.ConfigArrayIndex))
                    {
                        continue;
                    }

                    var configData = this.WriteValue(childProperty, item);
                    if (configData == null)
                    {
                        continue;
                    }
                    sb.AppendLine($"{key}{arrayIndex}.{configData}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
