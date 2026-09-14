# Unity Test & Coverage Runner (Windows / Tuanjie Engine)
# Equivalent to: mvn clean verify + jacoco:report in the Java project

param(
    [ValidateSet("EditMode", "PlayMode", "All")]
    [string]$Mode = "EditMode"
)

$ErrorActionPreference = "Stop"
$projectPath = Split-Path -Parent $PSScriptRoot
$resultsDir = Join-Path $projectPath "test-results"
$coverageDir = Join-Path $projectPath "coverage-report"

# Find Tuanjie Editor
$tuanjieHub = "${env:ProgramFiles}\Tuanjie\Hub\Editor"
if (-not (Test-Path $tuanjieHub)) {
    $tuanjieHub = "${env:ProgramFiles}\Unity\Hub\Editor"
}

$editorDir = Get-ChildItem $tuanjieHub -Directory | Where-Object { $_.Name -match "2022" } | Sort-Object Name -Descending | Select-Object -First 1
if (-not $editorDir) {
    Write-Error "Unity Editor 2022 not found in $tuanjieHub"
    exit 1
}

$unityExe = Join-Path $editorDir.FullName "Editor\Unity.exe"
Write-Host "Using Unity: $unityExe" -ForegroundColor Cyan

# Clean previous results
if (Test-Path $resultsDir) { Remove-Item $resultsDir -Recurse -Force }
New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null

$modes = if ($Mode -eq "All") { @("EditMode", "PlayMode") } else { @($Mode) }

foreach ($testMode in $modes) {
    Write-Host "`n=== Running $testMode tests ===" -ForegroundColor Yellow

    $testOutput = Join-Path $resultsDir "$testMode-results.xml"
    $coverageOutput = Join-Path $resultsDir "$testMode-coverage"

    & $unityExe -batchmode `
        -projectPath $projectPath `
        -runTests `
        -testPlatform $testMode `
        -testResults $testOutput `
        -enableCodeCoverage `
        -coverageResultsPath $coverageOutput `
        -coverageOptions "generateAdditionalMetrics;generateHtmlReport;generateBadgeReport" `
        -logFile `
        -nographics

    $exitCode = $LASTEXITCODE
    Write-Host "$testMode exit code: $exitCode"

    if (Test-Path $testOutput) {
        [xml]$xml = Get-Content $testOutput
        $total = $xml.testsuites.testsuites.testsuite.tests
        $failures = $xml.testsuites.testsuites.testsuite.failures
        Write-Host "$testMode Results: $total tests, $failures failures" -ForegroundColor $(if ($failures -eq 0) { 'Green' } else { 'Red' })
    }
}

# Generate Cobertura coverage report for Codecov
Write-Host "`n=== Generating coverage report ===" -ForegroundColor Yellow

$coverageFiles = Get-ChildItem $resultsDir -Recurse -Filter "*.xml" | Where-Object { $_.Name -match "coverage" -or $_.Name -match "Cobertura" }
if ($coverageFiles) {
    $reportsArg = ($coverageFiles.FullName -join ";")
    & reportgenerator -reports:$reportsArg -targetdir:$coverageDir -reporttypes:"Cobertura;Html" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Coverage report generated at: $coverageDir" -ForegroundColor Green
    } else {
        Write-Host "ReportGenerator not found. Install with: dotnet tool install -g dotnet-reportgenerator" -ForegroundColor Yellow
    }
} else {
    Write-Host "No coverage files found. Coverage may not be supported in this Unity version." -ForegroundColor Yellow
}

Write-Host "`nDone. Results in: $resultsDir" -ForegroundColor Cyan
