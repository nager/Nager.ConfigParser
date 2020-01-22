namespace Nager.ConfigParser.UnitTest.Model
{
    public class AlarmSystemConfiguration
    {
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Webhook { get; set; }
        public double[] ActiveSensorIds { get; set; }
    }
}
