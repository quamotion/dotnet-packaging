rem @echo off

pushd "%~dp0"
SET FDD_PACKAGE=%~n1%~x1
SET SCD_PACKAGE=%~n2%~x2
SET _IMAGENAME=dotnet_packaging_centos_test
SET _MOUNTPATH=%cd%
call ..\..\_scripts\runtest.cmd %_IMAGENAME% test-cli-fdd %FDD_PACKAGE% 
if ERRORLEVEL 1 goto end
call ..\..\_scripts\runtest.cmd %_IMAGENAME% test-cli-scd %SCD_PACKAGE%

:end
popd
exit /b %ERRORLEVEL%
