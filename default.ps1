properties {
    $base_dir = resolve-path .   
    $build_dir = "$base_dir\build"
    $buildartifacts_dir = "$build_dir\bin"
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
}

task Compile -depends Init {
	$v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
	$msbuildExe = "$env:windir\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe"
	$msbuildArgs = """$sln_file"" /p:Configuration=Release;OutDir=$buildartifacts_dir\"
    $p = start-process $msbuildExe $msbuildArgs -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$build_dir\MsbuildOutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "MsBuild failed see $build_dir\MsbuildOutput.txt"
	}
}

task Test -depends Compile {
	$nunitVersion = (ls "$packages_dir\NUnit*").Name
	$nunitExe = "$packages_dir\$nunitVersion\tools\nunit-console.exe"
	$nunitArguments = "$buildartifacts_dir\NuSelfUpdate.Tests.dll /xml=$build_dir\TestResults.xml"
		
	$p = start-process $nunitExe $nunitArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$build_dir\NUnitOutput.txt"		
	Move-Item "$buildartifacts_dir\bddify.html" "$build_dir"
	Move-Item "$buildartifacts_dir\bddify.css" "$build_dir"
	
	if ($p.ExitCode -ne 0) {
		throw "Tests failed see $build_dir\NUnitOutput.txt"
	}
}

task BuildPackage -depends Test {
	$nugetVersion = (ls "$packages_dir\NuGet.CommandLine*").Name
	$nugetExe = "$packages_dir\$nugetVersion\tools\nuget.exe"
	$excludes = "-Exclude **\*NuSelfUpdate.Sample.exe -Exclude **\*NuSelfUpdate.Tests.dll"
	$nugetArguments = "pack ""$base_dir\src\NuSelfUpdate\NuSelfUpdate.csproj"" -Prop Configuration=Package -OutputDirectory ""$build_dir"" $excludes"
	
	$p = start-process $nugetExe $nugetArguments -PassThru -Wait -NoNewWindow -RedirectStandardOutput "$build_dir\nugetoutput.txt"
	
	if ($p.ExitCode -ne 0) {
		throw "NuGet pack failed see $build_dir\nugetoutput.txt"
	}
}

task PublishPackage -depends BuildPackage {
}