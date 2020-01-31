namespace Nager.ConfigParser.UnitTest.Model
{
    public class Account : ConfigArrayElement
    {
        public int Enable { get; set; }
        public string Label { get; set; }
        [ConfigKey("display_name")]
        public string DisplayName { get; set; }
    }
}
