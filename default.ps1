include .\psake_ext.ps1

properties {
    $base_dir = resolve-path .
    $lib_dir = "$base_dir\Dependencies"
    $build_dir = "$base_dir\build"
    $buildartifacts_dir = "$build_dir\bin"
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

task RunSampleInit -depends FullClean, UpdateVersion {    
	new-item $build_dir -itemType directory
	new-item $buildartifacts_dir -itemType directory
	new-item $sample_dir -itemType directory
	new-item $samplepackage_dir -itemType directory
	new-item $prepPackage_dir -itemType directory
	new-item $prepPackageApp_dir -itemType directory
}

task PublishInit -depends PublishClean, UpdateVersion {    
	new-item $buildartifacts_dir -itemType directory
	new-item $prepPackage_dir -itemType directory
	new-item $prepPackageApp_dir -itemType directory
	
	if(!(test-path $samplepackage_dir -pathtype container)){
		new-item $samplepackage_dir -type directory
	}
}

task Compile {
	$v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    exec "$env:windir\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" """$sln_file"" /p:Configuration=Release;OutDir=$buildartifacts_dir\"
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
	
	start-process $nugetExe $nugetArguments -Wait -NoNewWindow
}

task PublishNewVersion -depends PublishInit, BuildPackage {
	Move-Item "$prepPackage_dir\*.nupkg" "$samplePackage_dir"
}

task RunSample -depends RunSampleInit, CopyRunningSampleFiles {
	start-process "$sample_dir\NuSelfUpdate.Sample.exe"
}