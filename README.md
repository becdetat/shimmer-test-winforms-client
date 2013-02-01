shimmer-test-winforms-client
============================

WinForms test client for Shimmer



## Notes

- application has to include Shimmer nuget package
- applciation needs a .nuspec alongside the solution
- run `.\.nuget\NuGet.exe pack .\src\ShimmerTestApplication\ShimmerTestApplication.csproj` in PM to generate a .nupkg file for the application
- the .nupkg file needs to be moved to the bin\debug folder for some reason (`Create-Release.ps1:57` uses `$buildDirectory\*.nupkg`)
- run this in PM: `Create-Release projectName` eg `Create-Release ShimmerTestApplication`
	- there is an error, initializing to normal mode, need to google this
	- then the `light` part of wix is failing: `error LGHT0103 : The system cannot find the file 'dotNetFx40_Full_x86_x64.exe'.` - I think this is why it isn't generating the installer .exe
- in `/Releases` the new full and delta packages are written and the RELEASES file is updated

