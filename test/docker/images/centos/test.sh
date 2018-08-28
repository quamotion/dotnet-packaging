#!/bin/bash
pushd "${0%/*}" > /dev/null 

IMAGE_NAME=dotnet_packaging_centos_test
source ../../_scripts/imagetests.sh
popd
exit $_RESULT