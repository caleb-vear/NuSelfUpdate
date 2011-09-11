function Get-Date-Version
{
    $now = [System.DateTime]::UtcNow
    $timeSinceMidnight = $now - $now.Date
    $revision = ($now - $now.Date).TotalSeconds / 2
    
	# I realise this isn't semantic versioning, but it makes the script simple :)
    return $now.ToString("yyyy.MM.dd.") + $revision.ToString("0")
}

function Generate-Assembly-Info
{
    param(
        [string]$product,
        [string]$copyright,
        [string]$company,
        [string]$version,
        [string]$file = $(throw "file is a required parameter.")
    )
    
    $asmInfo = "using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"
	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	$asmInfo  | Out-File -FilePath $file -Encoding UTF8
}