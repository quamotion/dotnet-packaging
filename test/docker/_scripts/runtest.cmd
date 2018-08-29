@echo off

set _IMAGENAME=%1
set _TESTNAME=%2
set _PACKAGENAME=%3


echo Running %_TESTNAME%
set _OUTFILE=%temp%\tests-%_TESTNAME%.output.txt
docker run -v %_MOUNTPATH%:/tests --rm %_IMAGENAME%:latest bash -c "/tests/%_TESTNAME%.sh %_PACKAGENAME%" > %_OUTFILE%

IF EXIST %_MOUNTPATH%\%_TESTNAME%.reference.txt ( 
    comp /M %_MOUNTPATH%\%_TESTNAME%.reference.txt %_OUTFILE% > NUL
    if errorlevel 1 (
        type %_OUTFILE%
        set _RESULT=1
    ) else (
        echo OK 
        set _RESULT=0
    ) 
) ELSE (
    set _RESULT=0
    copy /y %_OUTFILE% %_MOUNTPATH%\%_TESTNAME%.reference.txt
)
del %_OUTFILE%
exit /b %_RESULT%