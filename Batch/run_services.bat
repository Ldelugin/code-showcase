@ECHO OFF
SET /A Total_Instances=%~1
SET Service_Name=%~2

IF "%Total_Instances%"=="0" GOTO dont_run
CALL :start_service %Total_Instances% %Service_Name%
EXIT /B %ERRORLEVEL%

:start_service
SET /A Max=%~1-1
ECHO "This will start a total of %Total_Instances% %Service_Name% service(s)!"
FOR /L %%y IN (0, 1, %Max%) DO start /MIN "%~2" "C:\-- redacted --\%~2\-- redacted --%~2.exe"
EXIT /B 0

:dont_run
ECHO "Given %Total_Instances% should be higher then 0."