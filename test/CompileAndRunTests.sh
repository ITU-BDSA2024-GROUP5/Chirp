#!/bin/bash

dotnet run --project ../src/Chirp.Web &
dotnet_pid=$!

pwsh -ExecutionPolicy Bypass -File "./PlaywrightTests/Bin/Debug/net8.0/playwright.ps1" install-deps
pwsh -ExecutionPolicy Bypass -File "./PlaywrightTests/Bin/Debug/net8.0/playwright.ps1" install

cd ..

dotnet test

#pwsh -ExecutionPolicy Bypass -File "./test/PlaywrightTests/Bin/Debug/net8.0/playwright.ps1" uninstall

read -p "Press Any key to continue"
kill -9 $(lsof -t -i tcp:5177)
