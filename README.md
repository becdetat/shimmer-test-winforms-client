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


### Happier path

`Create-Release.ps1` can be changed as follows:

	$nugetPackages = ls "*.nupkg" | ?{ $_.Name.EndsWith(".symbols.nupkg") -eq $false }
	foreach($pkg in $nugetPackages) {
		$packageDir = Join-Path $solutionDir "packages"

So it looks for `.nupkg` files in the project root, rather than in `project\bin\Release`.

Then the project's NuGet package is created (in the PM console):

	.\.nuget\nuget.exe pack .\src\Project\Project.csproj -Properties Configuration=Release

That will create the `.nupkg` file in the project root.

`packages\shimmer.0.6.0.0-beta\tools\template.wxs` needs to be modifies per above for the dotNetFx40 dependency.

Then `Create-Release` can just be called in the PM console, which writes a setup.exe and `*.full.nupkg` and `*.diff.nupkg` packages.

Another problem is that the current NuGet package for Shimmer is a beta, which means that the version of the project also needs to be beta.

Even given the above working scenario, when running Setup.exe I still get an exception:

	TinyIoC.TinyIoCResolutionException was unhandled
	  HResult=-2146233088
	  Message=Unable to resolve type: ReactiveUI.IViewFor`1[[Shimmer.WiXUi.ViewModels.ErrorViewModel, Shimmer.WiXUi, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
	  Source=Shimmer.Core
	  StackTrace:
	       at TinyIoC.TinyIoCContainer.ResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
	       at TinyIoC.TinyIoCContainer.Resolve(Type resolveType)
	       at Shimmer.WiXUi.ViewModels.WixUiBootstrapper.<.ctor>b__0(Type type, String contract)
	       at ReactiveUI.RxApp.GetService(Type type, String key)
	       at ReactiveUI.Routing.RxRouting.ResolveView[T](T viewModel)
	       at ReactiveUI.Routing.RoutedViewHost.<.ctor>b__1(IRoutableViewModel vm)
	       at System.Reactive.AnonymousSafeObserver`1.OnNext(T value)
	       at System.Reactive.Linq.Observαble.Concat`1._.OnNext(TSource value)
	       at System.Reactive.Linq.Observαble.Select`2._.OnNext(TSource value)
	       at System.Reactive.Linq.Observαble.Where`1._.OnNext(TSource value)
	       at System.Reactive.Linq.Observαble.Merge`1._.ι.OnNext(TSource value)
	       at System.Reactive.Linq.Observαble.Select`2._.OnNext(TSource value)
	       at System.Reactive.SafeObserver`1.OnNext(TSource value)
	       at System.Reactive.ScheduledObserver`1.Run(Object state, Action`1 recurse)
	       at System.Reactive.Concurrency.Scheduler.<>c__DisplayClass11`1.<InvokeRec1>b__e(TState state1)
	       at System.Reactive.Concurrency.Scheduler.InvokeRec1[TState](IScheduler scheduler, Pair`2 pair)
	       at System.Reactive.Concurrency.DispatcherScheduler.<>c__DisplayClass1`1.<Schedule>b__0()
	       at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
	       at MS.Internal.Threading.ExceptionFilterHelper.TryCatchWhen(Object source, Delegate method, Object args, Int32 numArgs, Delegate catchHandler)
	       at System.Windows.Threading.DispatcherOperation.InvokeImpl()
	       at System.Windows.Threading.DispatcherOperation.InvokeInSecurityContext(Object state)
	       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
	       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
	       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
	       at System.Windows.Threading.DispatcherOperation.Invoke()
	       at System.Windows.Threading.Dispatcher.ProcessQueue()
	       at System.Windows.Threading.Dispatcher.WndProcHook(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
	       at MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
	       at MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)
	       at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
	       at MS.Internal.Threading.ExceptionFilterHelper.TryCatchWhen(Object source, Delegate method, Object args, Int32 numArgs, Delegate catchHandler)
	       at System.Windows.Threading.Dispatcher.LegacyInvokeImpl(DispatcherPriority priority, TimeSpan timeout, Delegate method, Object args, Int32 numArgs)
	       at MS.Win32.HwndSubclass.SubclassWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam)
	       at MS.Win32.UnsafeNativeMethods.DispatchMessage(MSG& msg)
	       at System.Windows.Threading.Dispatcher.PushFrameImpl(DispatcherFrame frame)
	       at System.Windows.Threading.Dispatcher.PushFrame(DispatcherFrame frame)
	       at System.Windows.Threading.Dispatcher.Run()
	       at System.Windows.Application.RunDispatcher(Object ignore)
	       at System.Windows.Application.RunInternal(Window window)
	       at System.Windows.Application.Run(Window window)
	       at Shimmer.WiXUi.App.Run()
	       at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
	       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
	       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
	       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
	       at System.Threading.ThreadHelper.ThreadStart()
	  InnerException: 


	