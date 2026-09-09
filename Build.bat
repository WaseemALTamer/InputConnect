@echo off
setlocal

echo ========================================
echo Building InputConnect
echo ========================================

dotnet publish -c Release -r win-x64 --self-contained true -o bin\Build\InputConnect-win-x64
if errorlevel 1 goto :error

dotnet publish -c Release -r win-x86 --self-contained true -o bin\Build\InputConnect-win-x86
if errorlevel 1 goto :error

dotnet publish -c Release -r win-arm64 --self-contained true -o bin\Build\InputConnect-win-arm64
if errorlevel 1 goto :error

dotnet publish -c Release -r linux-x64 --self-contained true -o bin\Build\InputConnect-linux-x64
if errorlevel 1 goto :error

dotnet publish -c Release -r linux-arm64 --self-contained true -o bin\Build\InputConnect-linux-arm64
if errorlevel 1 goto :error


echo.
echo ========================================
echo Creating Windows ZIP archives
echo ========================================

powershell -NoProfile -Command "Compress-Archive -Path 'bin\Build\InputConnect-win-x64\*' -DestinationPath 'bin\Build\InputConnect-win-x64.zip' -Force"
if errorlevel 1 goto :error

powershell -NoProfile -Command "Compress-Archive -Path 'bin\Build\InputConnect-win-x86\*' -DestinationPath 'bin\Build\InputConnect-win-x86.zip' -Force"
if errorlevel 1 goto :error

powershell -NoProfile -Command "Compress-Archive -Path 'bin\Build\InputConnect-win-arm64\*' -DestinationPath 'bin\Build\InputConnect-win-arm64.zip' -Force"
if errorlevel 1 goto :error


echo.
echo ========================================
echo Creating Linux TAR.GZ archives
echo ========================================

cd bin\Build

tar -czvf InputConnect-linux-x64.tar.gz InputConnect-linux-x64
if errorlevel 1 goto :error

tar -czvf InputConnect-linux-arm64.tar.gz InputConnect-linux-arm64
if errorlevel 1 goto :error

cd ..\..

echo.
echo ========================================
echo BUILD COMPLETE
echo ========================================
echo.

dir bin\Build

exit /b 0


:error

echo.
echo ========================================
echo BUILD FAILED
echo ========================================
echo.

exit /b 1