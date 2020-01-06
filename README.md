<p align="center">
    <img src="https://raw.githubusercontent.com/Aviuz/JsonLogger/master/Design/Logo.png" alt="JsonLogger" />
</p>
## Overview
.NET Library for object logging to json files. This library provides interface to log text messages, .NET Objects and Exceptions to file in JSON format.  
THis is very basic library with some additional features.

## Logging
There are three ways for logging:
```csharp
public void Log(string title, string text, LogCategory logCategory = LogCategory.Info)
{
	(...)
}

public void Log(Exception e, string title = null, LogCategory logCategory = LogCategory.Critical)
{
	(...)
}

public void Log(object item, string title = null, LogCategory logCategory = LogCategory.Info)
{
	(...)
}
```
Additionaly there are some similiar methods that vary in some arguments but works same as methods listed above.

There are **three** LogCategories:
```csharp
public enum LogCategory
{
    Critical,
    Warning,
    Info,
}
```

## Log file
Every JLogger class is associated with one log file. Logging through JLogger class is synchronized with monitors hide behind implementation of JLogger class.  
There is possibility to transfer log file to another file (to backup or to reduce size, etc.). To do so, *TranferLogToFile(string, bool)* method must be called. This method will hold logging for duration of transfering, transfer all logs to another file and finally create new empty log file in source path.  
Also there is possibility to create automatic transfering based on log file size.
