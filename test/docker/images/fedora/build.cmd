@echo off

pushd "%~dp0"

if not exist packages mkdir packages
copy /y "%1" packages > NUL
copy /y "%2" packages > NUL

for /f %%i in ('docker images dotnet_packaging_fedora_test -q') do set _IMAGE_ID=%%i
if "%_IMAGE_ID%"=="" (
   docker build -f Dockerfile . --tag dotnet_packaging_fedora_test
)
popd