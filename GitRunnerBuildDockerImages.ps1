param ([boolean]$autoPush = $True)
$ErrorActionPreference = "Stop"

function ThrowOnBuildFailure {
	if (-not $?) {
		throw 'A failure occurred during the docker build'
	}
}

$env:DOCKER_BUILDKIT = 1

# Get the git tag which is the same as the built version based on nerdbank gitversioning
$versionString = $env:TAG

$repositories = @("pdl-harbor-test.panoramicdata.com/library", "pdl-harbor-prod.panoramicdata.com/library", "panoramicdata")

Write-Output "Building LogicMonitor.Datamart.Cli ${versionString}..."
& docker build -f LogicMonitor.Datamart.Cli/Dockerfile -t logicmonitor-datamart:${versionString} .
ThrowOnBuildFailure

foreach ($repo in $repositories) {
	docker tag logicmonitor-datamart:${versionString} $repo/logicmonitor-datamart:${versionString}
}

if ($autoPush -ne $True) {
	$title = "Push images"
	$message = "Do you want to push the images for version '${versionString}'?"
	$yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Push the images"
	$no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "No thanks"
	$options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no)
	$result = $host.ui.PromptForChoice($title, $message, $options, 1)
	switch ($result) {
		0 { Write-Output "Proceeding to push images..." }
		1 { Write-Output "All done."; exit 1; }
	}
}

foreach ($repo in $repositories) {
	Write-Output "*** Pushing ${repo}/logicmonitor-datamart:${versionString} ***"
	& docker push ${repo}/logicmonitor-datamart:${versionString}
}
