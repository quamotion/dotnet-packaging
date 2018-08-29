#!/bin/bash

pushd "${0%/*}" > /dev/null 

if [ ! -d "packages" ]; then mkdir packages; fi
cp "$1" packages/ > /dev/null
cp "$2" packages/ > /dev/null

if [ -z `docker images "dotnet_packaging_centos_test" -q` ]; then
  docker build -f Dockerfile . --tag dotnet_packaging_centos_test
fi
popd