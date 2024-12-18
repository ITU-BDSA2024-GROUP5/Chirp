start cmd.exe /k dotnet run --project ../src/Chirp.Web
powershell -executionpolicy bypass -File ".\PlaywrightTests\Bin\Debug\net8.0\playwright.ps1" install
cd .. 
call dotnet test 
PAUSE
REM powershell -executionpolicy bypass -File "./test\PlaywrightTests\Bin\Debug\net8.0\playwright.ps1" uninstall 