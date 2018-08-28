#!/bin/bash

pushd "${0%/*}" > /dev/null 
docker build -f Dockerfile . --tag dotnet_packaging_fedora_test
popd