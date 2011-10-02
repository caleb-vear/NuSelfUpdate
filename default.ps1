properties {
    $base_dir = resolve-path .   
    $build_dir = "$base_dir\build"
    $buildartifacts_dir = "$build_dir\bin"
	$buildoutput_dir = "$build_dir\output"
    $sln_file = "$base_dir\src\NuSelfUpdate.sln"
    $packages_dir = "$base_dir\src\packages"
}

task default -depends BuildPackage

task FullClean {
    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
}

task Init -depends FullClean {
	new-item $build_dir -itemType directory | out-null
	new-item $buildartifacts_dir -itemType directory | out-null
	new-item $buildoutput_dir -itemType directory | out-null
}

task Compile -depends Init {
	$v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
	$msbuildExe = "$env:windir\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe"
	$msbuildArgs = """$sln_file"" /p:Configuration=Release;OutDir=$buildartifacts_dir\"
    $p = start-process $msbuildExe $msbuildArgs -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\MsbuildOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "MsBuild failed see $buildoutput_dir\MsbuildOutput.txt"
	}
}

task Test -depends Compile {
	$nunitVersion = (ls "$packages_dir\NUnit*").Name
	$nunitExe = "$packages_dir\$nunitVersion\tools\nunit-console.exe"
	$nunitArguments = "$buildartifacts_dir\NuSelfUpdate.Tests.dll /xml=$buildoutput_dir\TestResults.xml"
		
	$p = start-process $nunitExe $nunitArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\NUnitOutput.txt"		
	Move-Item "$buildartifacts_dir\bddify.html" "$buildoutput_dir"
	Move-Item "$buildartifacts_dir\bddify.css" "$buildoutput_dir"
	
	if ($p.ExitCode -ne 0) {
		throw "Tests failed see $buildoutput_dir\NUnitOutput.txt"
	}
}

task BuildPackage -depends Test {
	$nugetVersion = (ls "$packages_dir\NuGet.CommandLine*").Name
	$nugetExe = "$packages_dir\$nugetVersion\tools\nuget.exe"
	$excludes = "-Exclude **\*NuSelfUpdate.Sample.exe -Exclude **\*NuSelfUpdate.Tests.dll"
	$nugetArguments = "pack ""$base_dir\src\NuSelfUpdate\NuSelfUpdate.csproj"" -Prop Configuration=Package -OutputDirectory ""$build_dir"" $excludes"
	
	$p = start-process $nugetExe $nugetArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\NuGetPackOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "NuGet pack failed see $buildoutput_dir\NuGetPackOutput.txt"
	}
}

task PublishPackage -depends BuildPackage {
	$nugetVersion = (ls "$packages_dir\NuGet.CommandLine*").Name
	$nugetExe = "$packages_dir\$nugetVersion\tools\nuget.exe"
	$package = (ls "$build_dir\NuSelfUpdate*nupkg").Name
	$nugetArguments = "push $build_dir\$package"
	
	$p = start-process $nugetExe $nugetArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$buildoutput_dir\NuGetPushOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "NuGet push failed see $buildoutput_dir\NuGetPushOutput.txt"
	}
}