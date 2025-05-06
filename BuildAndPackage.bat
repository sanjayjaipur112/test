@echo off
echo Building WASender and Downloader...

REM Set paths
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
set SOLUTION_PATH=WASender.sln
set DOWNLOADER_PATH=WaSenderDownloader\WaSenderDownloader.csproj
set OUTPUT_DIR=Release
set ZIP_FILE=WaSender.zip

REM Create output directory if it doesn't exist
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

REM Build WASender in Release mode
echo Building WASender...
%MSBUILD% %SOLUTION_PATH% /p:Configuration=Release /t:Rebuild

REM Build Downloader in Release mode
echo Building WaSender Downloader...
%MSBUILD% %DOWNLOADER_PATH% /p:Configuration=Release /t:Rebuild

REM Copy necessary files to output directory
echo Copying files to output directory...
xcopy /E /Y WASender\bin\Release\* %OUTPUT_DIR%\

REM Create ZIP file
echo Creating ZIP file...
powershell -command "Compress-Archive -Path '%OUTPUT_DIR%\*' -DestinationPath '%OUTPUT_DIR%\%ZIP_FILE%' -Force"

REM Copy downloader to output directory
echo Copying downloader to output directory...
copy WaSenderDownloader\bin\Release\WaSenderDownloader.exe %OUTPUT_DIR%\

echo Build and packaging complete!
echo Output files are in the %OUTPUT_DIR% directory.
pause


