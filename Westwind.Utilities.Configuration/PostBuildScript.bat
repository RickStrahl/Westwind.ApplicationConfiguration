@echo off
setlocal

:: Make sure git utilities precede windows utilities in the path
set GitDir=C:\Program Files (x86)\Git\bin
set Path=%GitDir%;%Path%

:: Replace backslashes with forward slashes in the name of the script
:: so that bash can extract the correct value for ${BASH_SOURCE[0]}
set SCRIPT_NAME=%~dpn0
set SCRIPT_NAME=%SCRIPT_NAME:\=/%

bash.exe "%SCRIPT_NAME%" %*

if ERRORLEVEL 9010 goto :EOF
if ERRORLEVEL 9009 echo ERROR: bash not found. Install Git, which includes bash
