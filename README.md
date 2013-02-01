shimmer-test-winforms-client
============================

WinForms test client for Shimmer



## Notes

- application has to include Shimmer nuget package
- applciation needs a .nuspec alongside the solution
- run `.\.nuget\NuGet.exe pack .\src\ShimmerTestApplication\ShimmerTestApplication.csproj` in PM to generate a .nupkg file for the application
- the .nupkg file needs to be moved to the bin\debug folder for some reason (`Create-Release.ps1:57` uses `$buildDirectory\*.nupkg`)
	- **One step**: `.\.nuget\NuGet.exe pack .\src\ShimmerTestApplication\ShimmerTestApplication.csproj -Build -Properties Configuration=Release -OutputDirectory .\src\ShimmerTestApplication`
- run this in PM: `Create-Release projectName` eg `Create-Release ShimmerTestApplication`
	- there is an error, initializing to normal mode, need to google this
	- then the `light` part of wix is failing: `error LGHT0103 : The system cannot find the file 'dotNetFx40_Full_x86_x64.exe'.` - I think this is why it isn't generating the installer .exe <http://stackoverflow.com/questions/10430728/wix-cannot-find-file-when-trying-to-bundle-dotnet-framework-dependancy> looking good
	- changing `packages/shimmer.0.6.0.0-beta/tools/template.wxs` to this works:

  <PackageGroup Id="Netfx4Full">
      <ExePackage Id="Netfx4Full" Cache="no" Compressed="no" PerMachine="no" Permanent="yes" Vital="yes" Name="dotNetFx40_Full_x86_x64.exe"
                  DownloadUrl="http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"
                  DetectCondition="Netfx4FullVersion AND (NOT VersionNT64 OR Netfx4x64FullVersion)">
        <RemotePayload
          Description="Microsoft .NET Framework 4 Setup"
          Hash="58DA3D74DB353AAD03588CBB5CEA8234166D8B99"
          ProductName="Microsoft .NET Framework 4"
          Size="50449456"
          Version="4.0.30319.1"
        />
      </ExePackage>
  </PackageGroup>

- in `/Releases` the new full and delta packages are written and the RELEASES file is updated
- `Create-Release.ps1` - this line `rm "$buildDirectory\template.wixobj"` may not be needed, ObjectNotFound - except that it is there??
