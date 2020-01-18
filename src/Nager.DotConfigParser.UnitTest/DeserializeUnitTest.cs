using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Nager.DotConfigParser.UnitTest
{
    [TestClass]
    public class DeserializeUnitTest
    {
        [TestMethod]
        [DeploymentItem(@"Config1.txt")]
        public void FullTextFileTest()
        {
            var config = File.ReadAllText("Config1.txt");

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            Assert.AreEqual("250", item.SystemId);
            Assert.AreEqual("1", item.Photoservice[0].ConfigArrayIndex);
            Assert.AreEqual("http://127.0.0.1:8080/", item.Photoservice[0].PhotoUrl);
        }

        [TestMethod]
        public void CommentTest1()
        {
            var config = "#system.id=12\r\nsystem.id=14";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            Assert.AreEqual("14", item.SystemId);
        }

        [TestMethod]
        public void UseLatestValueTest1()
        {
            var config = "system.id=12\r\nsystem.id=14";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            Assert.AreEqual("14", item.SystemId);
        }

        [TestMethod]
        public void IntArrayTest1()
        {
            var config = "night.hours=18,19,20,21,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 20, 21, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void IntArrayTest2()
        {
            var config = "night.hours=18,19,,,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void IntArrayTest3()
        {
            var config = "night.hours=18,19,a,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void ConfigArrayTest1()
        {
            var config = "targetversion=MOCK-XXX\r\n" +
                "photoservice.1.spotid = 250\r\n" +
                "photoservice.1.publish.ids = 1,2,3,4\r\n" +
                "photoservice.2.spotid = 251\r\n" +
                "photoservice.2.publish.ids = 5,6,7,8";

            var configParser = new ConfigParser();
            var item = configParser.DeserializeObject<Mock1DeviceConfiguration>(config);

            Assert.AreEqual("MOCK-XXX", item.TargetVersion);
            Assert.AreEqual("1", item.Photoservice[0].ConfigArrayIndex);
            Assert.AreEqual("250", item.Photoservice[0].SpotId);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, item.Photoservice[0].PublishIds);
            Assert.AreEqual("2", item.Photoservice[1].ConfigArrayIndex);
            Assert.AreEqual("251", item.Photoservice[1].SpotId);
            CollectionAssert.AreEqual(new int[] { 5, 6, 7, 8 }, item.Photoservice[1].PublishIds);
        }
    }
}
