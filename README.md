# Nager.ConfigParser
dot seperated configuration parser

A parser for c# to read `.` seperated config files

**Example text config**
```
alarm.signal=light
alarm.signal.color=red
alarm.reset.seconds=2
```

```cs

var configConvert = new ConfigConvert();
var item = configConvert.DeserializeObject<ExampleConfiguration>(config);

public class ExampleConfiguration
{
    [ConfigKey("alarm.signal")]
    public string AlarmSignal { get; set; }
    [ConfigKey("alarm.signal.color")]
    public string AlarmSignalColor { get; set; }
    [ConfigKey("alarm.reset.seconds")]
    public int AlarmResetSeconds { get; set; }
}
```
