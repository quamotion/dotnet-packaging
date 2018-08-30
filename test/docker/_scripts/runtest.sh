#!/bin/bash
echo Running $_TESTNAME
_OUTFILE=$(mktemp)
docker run -v $_MOUNTPATH:/tests -e AZBRIDGE_TEST_CXNSTRING="$_CXNSTRING" --rm $IMAGE_NAME:latest bash /tests/$_TESTNAME.sh > $_OUTFILE
if [ -f $_MOUNTPATH/$_TESTNAME.reference.txt ]; then
    diff -w $_MOUNTPATH/$_TESTNAME.reference.txt $_OUTFILE > /dev/null 2>&1
    _RESULT=$?
    if [ $_RESULT -eq 0 ]; then 
        echo OK
    else
        diff -w $_MOUNTPATH/$_TESTNAME.reference.txt $_OUTFILE
    fi
else
    cp $_OUTFILE $_MOUNTPATH/$_TESTNAME.reference.txt > /dev/null 2>&1
    _RESULT=0
fi 
rm $_OUTFILE
echo $_TESTNAME done.
