# Changelog for JsonLogger

## 2.x for .NET Standard
2.0.0
- changed serializer to `System.Text.Json` library, which is much faster
- moved to library to .NET Standard

## 1.x for .NET Framework 4
1.0.8
- fixed bug with empty directiories
- fixed bug with cross-thread read access denied
1.0.7
- added failover behaviour for logging unserializable objects
1.0.6
- fixed object logging for primitives
- fixed object logging for arrays
1.0.5
- added transfering logs with optional automatic size-based transfering
1.0.4
- fixed issue with reading unsaved changes
- improved compatibility with logging objects
1.0.3
- improved appending logs synchronized aspect
- improved reading logs, now reading log reads saved and un-saved records
- lowered .NET requirements to 4.5
1.0.2
- changed name of methods
- fixed object logging
- added monitors to methods accessings files
1.0.1
- fixed time format
- improved unit tests
1.0.0
- first version of JsonLogger