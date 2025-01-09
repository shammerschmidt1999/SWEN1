@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo Syntax: $1 [pause]
echo - pause: optional, if set, the script will pause after each block
echo.

set "pauseFlag=0"
for %%a in (%*) do (
    if /I "%%a"=="pause" (
        set "pauseFlag=1"
    )
)

REM --------------------------------------------------
echo 4) acquire packages kienboec
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer packageUser-mtcgToken" -d "{\"packageType\": \"Basic\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer packageUser-mtcgToken" -d "{\"packageType\": \"Basic\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer packageUser-mtcgToken" -d "{\"packageType\": \"Premium\"}"
echo "Should return HTTP 201"
echo.
echo should fail (no money):
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer packageUser-mtcgToken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 4xx - Not enough money"
echo.
echo.