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
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Competitor1\", \"Password\":\"Competitor1\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"Competitor2\", \"Password\":\"Competitor2\"}"
echo "Should return HTTP 201"

REM --------------------------------------------------
echo 2) Login Users
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"Competitor1\", \"Password\":\"Competitor1\"}"
echo "should return HTTP 200 with generated token for the user"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"Competitor2\", \"Password\":\"Competitor2\"}"
echo "should return HTTP 200 with generated token for the user"

REM --------------------------------------------------
echo 3) Acquire Package
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer competitor1-mtcgToken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer competitor2-mtcgToken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 201"
echo.

REM --------------------------------------------------
echo 4) Configure Deck
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer competitor1-mtcgToken" -d "[\"Glacial Spike\", \"Brimstone\", \"Tide Knight\", \"Lancelot\"]"
echo "Should return HTTP 2xx"

curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer competitor2-mtcgToken" -d "[\"Meteor\", \"Shipeater\", \"Tsunami\", \"Burned One\"]"
echo "Should return HTTP 2xx"

REM --------------------------------------------------
echo 5) battle
curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer competitor1-mtcgToken"
curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer competitor2-mtcgToken"

REM --------------------------------------------------
echo 6) stats
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer competitor1-mtcgToken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer competitor2-mtcgToken"
echo "Should return HTTP 200 - and user stats"

REM --------------------------------------------------
echo 7) Scoreboard
curl -i -X GET http://localhost:10001/scoreboard --header "Authorization: Bearer competitor2-mtcgToken"
echo "Should return HTTP 200 - and the scoreboard"