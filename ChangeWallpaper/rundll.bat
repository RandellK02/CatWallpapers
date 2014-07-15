@echo off
::%SystemRoot%\System32\RUNDLL32.EXE user32.dll,UpdatePerUserSystemParameters

IF EXIST %USERPROFILE%\refresh.ps1 (
	PowerShell.exe -WindowStyle Hidden -NoProfile -Nologo -NonInteractive -ExecutionPolicy Bypass -Command "& '%USERPROFILE%\refresh.ps1'"
)
ELSE (
	PowerShell.exe -WindowStyle Hidden -NoProfile -Nologo -NonInteractive -ExecutionPolicy Bypass -Command "& 'C:\Windows\System32\refresh.ps1'"
)