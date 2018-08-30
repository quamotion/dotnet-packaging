set _IMAGE_ID=
for /f %%i in ('docker images %IMAGE_NAME% -q') do set _IMAGE_ID=%%i
if "%_IMAGE_ID%"=="" exit /b
docker rmi -f %IMAGE_NAME%:latest