using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.ConfigParser.UnitTest.Model;

namespace Nager.ConfigParser.UnitTest
{
    [TestClass]
    public class SerializeUnitTest
    {
        [TestMethod]
        public void SerializeTest1()
        {
            var config = new DeviceConfiguration
            {
                TargetVersion = "MOCK-1234",
                NightHours = new [] { 1, 2, 3, 4 }
            };

            var configParser = new ConfigConvert();
            var item = configParser.SerializeObject(config);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void SerializeTest2()
        {
            var config = new DeviceConfiguration
            {
                TargetVersion = "MOCK-1234",
                NightHours = new[] { 1, 2, 3, 4 },
                Photoservice = new Photoservice[]
                {
                    new Photoservice
                    {
                        ConfigArrayIndex = "1",
                        SpotId = "18"
                    }
                }
            };

            var configParser = new ConfigConvert();
            var item = configParser.SerializeObject(config);
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void SerializeTest3()
        {
            var config = new AlarmSystemConfiguration
            {
                Active = false,
                ActiveSensorIds = new double[] { 11.22, 22.5 }
            };

            var configParser = new ConfigConvert();
            var item = configParser.SerializeObject(config);
            Assert.AreEqual("active=False\r\nactivesensorids=11.22,22.50", item.Trim());
        }
    }
}
