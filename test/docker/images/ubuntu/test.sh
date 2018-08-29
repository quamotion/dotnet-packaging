#!/bin/bash
pushd "${0%/*}" > /dev/null 

IMAGE_NAME=dotnet_packaging_ubuntu1604_test

FDD_PACKAGE=${1##*/}
SCD_PACKAGE=${2##*/}
_IMAGENAME=dotnet_packaging_ubuntu1604_test
_MOUNTPATH=%cd%
call ..\..\_scripts\runtest.cmd %_IMAGENAME% test-cli-fdd %FDD_PACKAGE% 
if ERRORLEVEL 1 goto end
call ..\..\_scripts\runtest.cmd %_IMAGENAME% test-cli-scd %SCD_PACKAGE%

popd
exit $_RESULT
