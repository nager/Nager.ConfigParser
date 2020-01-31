using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.ConfigParser.UnitTest.Model;
using System.IO;

namespace Nager.ConfigParser.UnitTest
{
    [TestClass]
    public class DeserializeUnitTest
    {
        [TestMethod]
        [DeploymentItem(@"Config1.txt")]
        public void FullTextFileTest()
        {
            var config = File.ReadAllText("Config1.txt");

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            Assert.AreEqual("250", item.SystemId);
            Assert.AreEqual("1", item.Photoservice[0].ConfigArrayIndex);
            Assert.AreEqual("http://127.0.0.1:8080/", item.Photoservice[0].PhotoUrl);
        }

        [TestMethod]
        public void CommentTest1()
        {
            var config = "#system.id=12\r\nsystem.id=14";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            Assert.AreEqual("14", item.SystemId);
        }

        [TestMethod]
        public void UseLatestValueTest1()
        {
            var config = "system.id=12\r\nsystem.id=14";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            Assert.AreEqual("14", item.SystemId);
        }

        [TestMethod]
        public void IntArrayTest1()
        {
            var config = "night.hours=18,19,20,21,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 20, 21, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void IntArrayTest2()
        {
            var config = "night.hours=18,19,,,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void IntArrayTest3()
        {
            var config = "night.hours=18,19,a,22,23,0,1,2,3,4,5,6,7";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[] { 18, 19, 22, 23, 0, 1, 2, 3, 4, 5, 6, 7 }, item.NightHours);
        }

        [TestMethod]
        public void IntArrayTest4()
        {
            var config = "night.hours=";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            CollectionAssert.AreEqual(new int[0], item.NightHours);
        }

        [TestMethod]
        public void ConfigArrayTest1()
        {
            var config = "targetversion=MOCK-XXX\r\n" +
                "photoservice.1.spotid=250\r\n" +
                "photoservice.1.publish.ids=1,2,3,4\r\n" +
                "photoservice.2.spotid=251\r\n" +
                "photoservice.2.publish.ids=5,6,7,8";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<DeviceConfiguration>(config);

            Assert.AreEqual("MOCK-XXX", item.TargetVersion);
            Assert.AreEqual("1", item.Photoservice[0].ConfigArrayIndex);
            Assert.AreEqual("250", item.Photoservice[0].SpotId);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, item.Photoservice[0].PublishIds);
            Assert.AreEqual("2", item.Photoservice[1].ConfigArrayIndex);
            Assert.AreEqual("251", item.Photoservice[1].SpotId);
            CollectionAssert.AreEqual(new int[] { 5, 6, 7, 8 }, item.Photoservice[1].PublishIds);
        }

        [TestMethod]
        public void AlarmSystemTest1()
        {
            var config = "active=true\r\n" +
                "name=House1\r\n" +
                "webhook=http://securitycompany1.com/alarm/\r\n" +
                "activesensorids=11.2,20.3,30.4,104";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsTrue(item.Active);
            Assert.AreEqual("House1", item.Name);
            Assert.AreEqual("http://securitycompany1.com/alarm/", item.Webhook);
            CollectionAssert.AreEqual(new double[] { 11.2, 20.3, 30.4, 104 }, item.ActiveSensorIds);
        }

        [TestMethod]
        public void AlarmSystemTest2()
        {
            var config = "active = false\r\n" +
                "name = House1\r\n" +
                "webhook = http://securitycompany1.com/alarm/\r\n" +
                "activesensorids = 11.21,20.311,30.4,104";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsFalse(item.Active);
            Assert.AreEqual("House1", item.Name);
            Assert.AreEqual("http://securitycompany1.com/alarm/", item.Webhook);
            CollectionAssert.AreEqual(new double[] { 11.21, 20.311, 30.4, 104 }, item.ActiveSensorIds);
        }

        [TestMethod]
        public void AlarmSystemTest3()
        {
            var config = "active = false\r\n" +
                "name = House1\r\n" +
                "webhook = http://securitycompany1.com/alarm/\r\n" +
                "activesensorids =";

            var configParser = new ConfigConvert();
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsFalse(item.Active);
            Assert.AreEqual("House1", item.Name);
            Assert.AreEqual("http://securitycompany1.com/alarm/", item.Webhook);
            CollectionAssert.AreEqual(new double[0], item.ActiveSensorIds);
        }

        [TestMethod]
        public void CustomSplitCharTest()
        {
            var config = "active:false\r\n" +
                "name:House1\r\n" +
                "activesensorids:3,2";

            var configParser = new ConfigConvert(new ConfigConvertConfig { KeyValueDelimiter = ':' });
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsFalse(item.Active);
            Assert.AreEqual("House1", item.Name);
            CollectionAssert.AreEqual(new double[] { 3, 2 }, item.ActiveSensorIds);
        }

        [TestMethod]
        public void CustomValueDelimiterTest()
        {
            var config = "active=false\r\n" +
                "activesensorids=3|2|1";

            var configParser = new ConfigConvert(new ConfigConvertConfig { ValueDelimiter = '|'});
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsFalse(item.Active);
            CollectionAssert.AreEqual(new double[] { 3, 2, 1 }, item.ActiveSensorIds);
        }

        [TestMethod]
        public void CustomConfigDelimiterWithCustomValueDelimiterTest()
        {
            var config = "active=false,activesensorids=3|2|1";

            var configParser = new ConfigConvert(new ConfigConvertConfig { ConfigDelimiter = new char[] { ',' }, ValueDelimiter = '|' });
            var item = configParser.DeserializeObject<AlarmSystemConfiguration>(config);

            Assert.IsFalse(item.Active);
            CollectionAssert.AreEqual(new double[] { 3, 2, 1 }, item.ActiveSensorIds);
        }

        [TestMethod]
        public void ObjectArrayTest1()
        {
            var config = "#comment1\r\n#comment2\r\naccount.1.enable = 1\r\naccount.1.label = Front\r\naccount.1.display_name = Front";

            var configParser = new ConfigConvert(new ConfigConvertConfig());
            var item = configParser.DeserializeObject<AccountCollection>(config);

            item.Should().BeEquivalentTo(new AccountCollection
            {
                Accounts = new Account[]
                {
                    new Account
                    {
                         ConfigArrayIndex = "1",
                         Enable = 1,
                         Label= "Front",
                         DisplayName = "Front"
                    }
                }
            });
        }
    }
}
