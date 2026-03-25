@echo off
echo Building ChatBlocker...
dotnet build -c Release
echo Build complete!
echo Executable location: bin\Release\net6.0-windows\ChatBlocker.exe
pause
