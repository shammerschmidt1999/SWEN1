@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo Syntax: $1 [pause]
echo - pause: optional, if set, the script will pause after each block
echo NOTE: Add users to DB before running this script (manually add Tokens to DB)
echo Creation Script is found in MonsterTradingCards.FullTest.Preparation.curl.bat
echo.

set "pauseFlag=0"
for %%a in (%*) do (
    if /I "%%a"=="pause" (
        set "pauseFlag=1"
    )
)

REM --------------------------------------------------
echo 1) stats
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser4-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 2) scoreboard
curl -i -X GET http://localhost:10001/scoreboard --header "Authorization: Bearer fulltestuser2-mtcgtoken"
echo "Should return HTTP 200 - and the scoreboard"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 3) show all acquired cards FullTestUser1
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 204 - and a list of all cards"
echo should fail (no token)
curl -i -X GET http://localhost:10001/cards 
echo "Should return HTTP 4xx - Unauthorized"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 4) show all acquired cards FullTestUser3
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser3-mtcgtoken"
echo "Should return HTTP 204 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause

echo 5) acquire packages FullTestUser1
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser1-mtcgtoken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 201"
echo acquire packages FullTestUser2
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser2-mtcgtoken" -d "{\"packageType\": \"Basic\"}"
echo "Should return HTTP 201"
echo acquire packages FullTestUser3
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser3-mtcgtoken" -d "{\"packageType\": \"Premium\"}"
echo "Should return HTTP 201"
echo acquire packages FullTestUser4
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser4-mtcgtoken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 201"
echo FullTestUser1 should fail (no money):
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser1-mtcgtoken" -d "{\"packageType\": \"Legendary\"}"
echo "Should return HTTP 4xx - Not enough money"
echo.
echo.

if %pauseFlag%==1 pause

echo 6) show all acquired cards FullTestUser1 - FullTestUser4
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser2-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser3-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer fulltestuser4-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
echo.

echo 7) show unconfigured deck
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 204 - and a empty-list"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser3-mtcgtoken"
echo "Should return HTTP 204 - and a empty-list"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 8) configure deck
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser1-mtcgtoken" -d "[\"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\"]"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser2-mtcgtoken" -d "[\"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\"]"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser2-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser3-mtcgtoken" -d "[\"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\"]"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser3-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer fulltestuser4-mtcgtoken" -d "[\"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\", \"PLACEHOLDER\"]"
echo "Should return HTTP 400 - and tell user that his deck is already full"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer fulltestuser4-mtcgtoken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 9) battle
curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer fulltestuser1-mtcgtoken"
curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer fulltestuser3-mtcgtoken"

curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer fulltestuser2-mtcgtoken"
curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer fulltestuser4-mtcgtoken"

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 10) stats
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser1-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser2-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser3-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer fulltestuser4-mtcgtoken"
echo "Should return HTTP 200 - and user stats"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 11) scoreboard
curl -i -X GET http://localhost:10001/scoreboard --header "Authorization: Bearer fulltestuser2-mtcgtoken"
echo "Should return HTTP 200 - and the scoreboard"
echo.
echo.
