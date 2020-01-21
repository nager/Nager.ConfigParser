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
            var item = configParser.SerializeObject<DeviceConfiguration>(config);
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
            var item = configParser.SerializeObject<DeviceConfiguration>(config);
            Assert.IsNotNull(item);
        }
    }
}
