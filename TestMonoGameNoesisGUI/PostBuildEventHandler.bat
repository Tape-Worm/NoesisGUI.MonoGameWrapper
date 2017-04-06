@ECHO OFF

REM All arguments should be passed between double quotes
REM %1="$(ProjectDir)"
REM %2="$(TargetDir)"
REM %3="$(PlatformName)"

ECHO Copying native Noesis %3 dlls to output...

IF %3 EQU "x86" (
	XCOPY /y /d "%1..\NoesisSDK\Bin\windows_x86\*" %2
	IF ERRORLEVEL 1 (
	  ECHO Failed to copy Noesis 32-bit native DLL 
	  EXIT 100
	) 
) 

IF %3 EQU "x64" (
	XCOPY /y /d "%1..\NoesisSDK\Bin\windows_x86_64\*" %2
	IF ERRORLEVEL 1 (
	  ECHO Failed to copy Noesis 64-bit native DLL 
	  EXIT 100
	) 
) 
