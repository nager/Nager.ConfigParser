# Nager.ConfigParser
key value seperated configuration parser

A parser for c# to read `.` seperated config files

## How can I use it?

The package is available on [nuget](https://www.nuget.org/packages/Nager.ConfigParser)
```
PM> install-package Nager.ConfigParser
```



## Examples

### Simple config

***text config***
```
alarm.signal=light
alarm.signal.color=red
alarm.reset.seconds=2
```

***Read text config***
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

### Array config

***text config***
```
alarm.signal=light
alarm.signal.color=red
alarm.device.1.name=sensor1
alarm.device.1.ip=192.168.0.20
alarm.device.1.timeout=100
alarm.device.2.name=sensor1
alarm.device.2.ip=192.168.0.20
alarm.device.2.timeout=100
```

***Read text config***
```cs
var configConvert = new ConfigConvert();
var item = configConvert.DeserializeObject<ExampleConfiguration>(config);

public class ExampleConfiguration
{
    [ConfigKey("alarm.signal")]
    public string AlarmSignal { get; set; }
    [ConfigKey("alarm.signal.color")]
    public string AlarmSignalColor { get; set; }
    [ConfigArray]
    [ConfigKey("alarm.device.")]
    public AlarmDevice[] AlarmDevices { get; set; }
}

public class AlarmDevice : ConfigArrayElement
{
    [ConfigKey("name")]
    public string Name { get; set; }
    [ConfigKey("ip")]
    public string IpAddress { get; set; }
    [ConfigKey("Timeout")]
    public int Timeout { get; set; }
}
```
