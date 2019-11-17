nuget pack package.nuspec

nuget push "*.nupkg" -Source "GitHub"

del "*.nupkg"

@echo off
pause