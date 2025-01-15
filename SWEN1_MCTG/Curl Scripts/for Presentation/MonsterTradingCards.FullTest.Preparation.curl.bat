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
echo 1) Create Users (Registration)
REM Create User
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUser1\", \"Password\":\"FullTestUser1PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUser2\", \"Password\":\"FullTestUser2PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUser3\",    \"Password\":\"FullTestUser3PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUser4\",    \"Password\":\"FullTestUser4PW\"}"
echo "Should return HTTP 201"
echo.