@echo off

pushd "%~dp0"
docker build -f Dockerfile . --tag dotnet_packaging_fedora_test
popd