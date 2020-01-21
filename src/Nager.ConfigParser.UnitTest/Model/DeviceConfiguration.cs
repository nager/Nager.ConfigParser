namespace Nager.ConfigParser.UnitTest.Model
{
    public class DeviceConfiguration
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
}
