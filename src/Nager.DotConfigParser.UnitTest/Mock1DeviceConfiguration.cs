namespace Nager.DotConfigParser.UnitTest
{
    public class Mock1DeviceConfiguration
    {
        public string TargetVersion { get; set; }
        [ConfigKey("cardreader.url")]
        public string CardReaderVersion { get; set; }
        [ConfigKey("system.id")]
        public string SystemId { get; set; }
        [ConfigKey("photo.delay")]
        public int PhotoDelay { get; set; }
        [ConfigArray]
        [ConfigKey("photoservice.")]
        public Photoservice[] Photoservice { get; set; }
        [ConfigKey("night.hours")]
        public int[] NightHours { get; set; }
    }

    public class Photoservice : ConfigArrayElement
    {
        public string SpotId { get; set; }
        [ConfigKey("photo.url")]
        public string PhotoUrl { get; set; }
        [ConfigKey("motion.timeout")]
        public string MotionTimeout { get; set; }
        [ConfigKey("publish.ids")]
        public int[] PublishIds { get; set; }
    }
}
