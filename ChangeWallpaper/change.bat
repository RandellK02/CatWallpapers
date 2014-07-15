@echo off
reg add "HKCU\Control Panel\Desktop" /v Wallpaper /f /t REG_SZ /d C:\Users\Randell\Pictures\IMAG0046.jpg
RUNDLL32.EXE user32.dll,UpdatePerUserSystemParameters
pause