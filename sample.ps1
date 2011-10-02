include .\sample_ext.ps1

properties {
    $base_dir = resolve-path .
    $build_dir = "$base_dir\build"
    $buildartifacts_dir = "$build_dir\bin"
	$buildoutput_dir = "$build_dir\output"
    $sln_file = "$base_dir\src\NuSelfUpdate.sln"
    $version = Get-Date-Version
    $packages_dir = "$base_dir\src\packages"
    $sample_dir = "$build_dir\runningsample"
	$samplepackage_dir = "$sample_dir\packages"
	$prepPackage_dir = "$build_dir\packaging"
	$prepPackageApp_dir = "$prepPackage_dir\app"
}

task default -depends PublishNewVersion

task PublishClean {
	remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue
	remove-item -force -recurse $prepPackage_dir -ErrorAction SilentlyContinue
	remove-item -force -recurse $buildoutput_dir -ErrorAction SilentlyContinue
}

task FullClean {
    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
}

task UpdateVersion {
    Generate-Assembly-Info `
        -file  "$base_dir\src\Sample\Properties\AssemblyInfo.cs" `
        -product "NuSelfUpdate.Sample" `
        -copyright "Copyright © Caleb Vear 2011" `
        -company "Caleb Vear" `
        -version $version
}

task ResetVersion {
	Generate-Assembly-Info `
        -file  "$base_dir\src\Sample\Properties\AssemblyInfo.cs" `
        -product "NuSelfUpdate.Sample" `
        -copyright "Copyright © Caleb Vear 2011" `
        -company "Caleb Vear" `
        -version "1.0.0.0"
}

task RunSampleInit -depends FullClean, ResetVersion {    
	new-item $build_dir -itemType directory | out-null
	new-item $buildartifacts_dir -itemType directory | out-null
	new-item $sample_dir -itemType directory | out-null
	new-item $samplepackage_dir -itemType directory | out-null
	new-item $prepPackage_dir -itemType directory | out-null
	new-item $prepPackageApp_dir -itemType directory | out-null
}

task PublishInit -depends PublishClean, UpdateVersion {    
	new-item $buildartifacts_dir -itemType directory | out-null
	new-item $prepPackage_dir -itemType directory | out-null
	new-item $prepPackageApp_dir -itemType directory | out-null
	new-item $buildoutput_dir -itemType directory | out-null
	
	if(!(test-path $samplepackage_dir -pathtype container)){
		new-item $samplepackage_dir -type directory | out-null
	}
}

task Compile {
	$v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
	$msbuildExe = "$env:windir\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe"
	$msbuildArgs = """$sln_file"" /p:Configuration=Release;OutDir=$buildartifacts_dir\"
    $p = start-process $msbuildExe $msbuildArgs -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\MsbuildOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "MsBuild failed see $buildoutput_dir\MsbuildOutput.txt"
	}
}

task PreparePackageFiles -depends Compile {
	Copy-Item "$buildartifacts_dir\Nuget.Core.dll" "$prepPackageApp_dir\Nuget.Core.dll"
	Copy-Item "$buildartifacts_dir\NuSelfUpdate.Sample.exe" "$prepPackageApp_dir\NuSelfUpdate.Sample.exe"
	Copy-Item "$buildartifacts_dir\NuSelfUpdate.dll" "$prepPackageApp_dir\NuSelfUpdate.dll"
	Copy-Item "$buildartifacts_dir\System.Reactive.dll" "$prepPackageApp_dir\System.Reactive.dll"
	
	Copy-Item "$base_dir\src\Sample\NuSelfUpdate.Sample.nuspec" "$prepPackage_dir\NuSelfUpdate.Sample.nuspec"
}

task CopyRunningSampleFiles -depends Compile {
	Copy-Item "$buildartifacts_dir\Nuget.Core.dll" "$sample_dir\Nuget.Core.dll"
	Copy-Item "$buildartifacts_dir\NuSelfUpdate.Sample.exe" "$sample_dir\NuSelfUpdate.Sample.exe"
	Copy-Item "$buildartifacts_dir\NuSelfUpdate.dll" "$sample_dir\NuSelfUpdate.dll"
	Copy-Item "$buildartifacts_dir\System.Reactive.dll" "$sample_dir\System.Reactive.dll"
}

task BuildPackage -depends PreparePackageFiles {
	$nugetVersion = (ls "$packages_dir\NuGet.CommandLine*").Name
	$nugetExe = "$packages_dir\$nugetVersion\tools\nuget.exe"
	$nugetArguments = "pack ""$prepPackage_dir\NuSelfUpdate.Sample.nuspec"" -OutputDirectory ""$prepPackage_dir"" -Version $version"
	
	$p = start-process $nugetExe $nugetArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\NuGetOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "NuGet pack failed see $buildoutput_dir\nugetoutput.txt"
	}
}

task PublishNewVersion -depends PublishInit, BuildPackage, ResetVersion {
	Move-Item "$prepPackage_dir\*.nupkg" "$samplePackage_dir"
}

task RunSample -depends RunSampleInit, CopyRunningSampleFiles {
	start-process "$sample_dir\NuSelfUpdate.Sample.exe" "-test -updatemode ""auto update"""
}