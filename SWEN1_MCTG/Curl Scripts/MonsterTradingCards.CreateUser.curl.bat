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

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Florian\", \"Password\":\"floriansPW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Marius\", \"Password\":\"myPassword123\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"istratorPW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"PackageBuyer\",    \"Password\":\"WillBuyAPackage\"}"
echo "Should return HTTP 201"
echo.

echo should fail:
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Florian\", \"Password\":\"hammerschmidt\"}"
echo "Should return HTTP 4xx - User already exists"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Marius\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx - User already exists"
echo. 
echo.

if %pauseFlag%==1 pause