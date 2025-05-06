@echo off
echo Building WASender Standalone Executable...

REM Set paths
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
set SOLUTION_PATH=WASender.sln
set STANDALONE_DIR=Standalone
set ILREPACK_PATH=packages\ILRepack.2.0.18\tools\ILRepack.exe

REM Create output directory if it doesn't exist
if not exist %STANDALONE_DIR% mkdir %STANDALONE_DIR%

REM Build WASender in Release mode
echo Building WASender...
%MSBUILD% %SOLUTION_PATH% /p:Configuration=Release /t:Rebuild

REM Install ILRepack if not present
if not exist %ILREPACK_PATH% (
    echo Installing ILRepack...
    nuget install ILRepack -Version 2.0.18 -OutputDirectory packages
)

REM Merge assemblies
echo Merging assemblies...
%ILREPACK_PATH% /target:winexe /targetplatform:v4 /out:%STANDALONE_DIR%\WASender.exe ^
    WASender\bin\Release\WASender.exe ^
    WASender\bin\Release\AeroWizard.dll ^
    WASender\bin\Release\BouncyCastle.Crypto.dll ^
    WASender\bin\Release\EntityFramework.dll ^
    WASender\bin\Release\EntityFramework.SqlServer.dll ^
    WASender\bin\Release\EPPlus.dll ^
    WASender\bin\Release\Google.Generative.AI.dll ^
    WASender\bin\Release\MaterialSkin.dll ^
    WASender\bin\Release\Microsoft.Web.WebView2.Core.dll ^
    WASender\bin\Release\Microsoft.Web.WebView2.WinForms.dll ^
    WASender\bin\Release\Newtonsoft.Json.dll ^
    WASender\bin\Release\ProjectCommon.dll ^
    WASender\bin\Release\RestSharp.dll ^
    WASender\bin\Release\System.IO.Compression.ZipFile.dll ^
    WASender\bin\Release\WebDriver.dll

REM Copy additional required files
echo Copying additional files...
copy WASender\bin\Release\WASender.exe.config %STANDALONE_DIR%\WASender.exe.config
copy WASender\bin\Release\LogoWA.ico %STANDALONE_DIR%\LogoWA.ico
copy WASender\bin\Release\chromedriver.exe %STANDALONE_DIR%\chromedriver.exe
copy WASender\bin\Release\*.wav %STANDALONE_DIR%\
copy WASender\bin\Release\*.txt %STANDALONE_DIR%\
mkdir %STANDALONE_DIR%\Resources
xcopy /E /Y WASender\bin\Release\Resources\* %STANDALONE_DIR%\Resources\
mkdir %STANDALONE_DIR%\runtimes
xcopy /E /Y WASender\bin\Release\runtimes\* %STANDALONE_DIR%\runtimes\

REM Create ZIP file
echo Creating ZIP file...
powershell -command "Compress-Archive -Path '%STANDALONE_DIR%\*' -DestinationPath '%STANDALONE_DIR%\WASender-Standalone.zip' -Force"

echo Standalone executable build complete!
echo Output files are in the %STANDALONE_DIR% directory.
pause

