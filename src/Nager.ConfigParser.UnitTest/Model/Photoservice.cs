namespace Nager.ConfigParser.UnitTest.Model
{
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
