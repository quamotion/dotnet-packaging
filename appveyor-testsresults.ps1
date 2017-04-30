# upload results to AppVeyor
$wc = New-Object 'System.Net.WebClient'
$wc.UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", (Join-Path $env:APPVEYOR_BUILD_FOLDER .\Packaging.Targets.Tests\TestResults\testresults.trx))