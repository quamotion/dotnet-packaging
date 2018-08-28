@echo off

pushd "%~dp0"
SET IMAGE_NAME=dotnet_packaging_debian8_test
call ..\..\_scripts\imagetests.cmd %*
popd
exit /b %ERRORLEVEL%
