@echo off

pushd "%~dp0"
SET IMAGE_NAME=dotnet_packaging_centos_test
call ..\..\_scripts\imagetests.cmd %*
popd
exit /b %ERRORLEVEL%
