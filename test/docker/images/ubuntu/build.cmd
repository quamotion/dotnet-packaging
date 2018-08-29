@echo off

pushd "%~dp0"

if not exist packages mkdir packages
copy /y "%1" packages > NUL
copy /y "%2" packages > NUL
docker build -f Dockerfile . --tag dotnet_packaging_ubuntu1604_test 

popd