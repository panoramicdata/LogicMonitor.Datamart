$ErrorActionPreference = "Stop"

git pull

if (git status --porcelain) {
	Write-Error "Git repo is not clean. Make sure any changes have been committed."
	exit 1;
}

$nbgvVersion = nbgv get-version -v Version
$versionParts = $nbgvVersion.Split(".")
$versionString = $versionParts[0] + "." + $versionParts[1] + "." + $versionParts[2];

$env:TAG = $versionString

& .\GitRunnerBuildDockerImages.ps1 $False