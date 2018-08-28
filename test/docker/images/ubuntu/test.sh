#!/bin/bash
pushd "${0%/*}" > /dev/null 

IMAGE_NAME=dotnet_packaging_ubuntu1604_test
source ../../_scripts/imagetests.sh
popd
exit $_RESULT
