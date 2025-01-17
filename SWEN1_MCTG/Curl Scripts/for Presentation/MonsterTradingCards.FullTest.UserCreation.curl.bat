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
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation1\", \"Password\":\"FullTestUserCreation1PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation2\", \"Password\":\"FullTestUserCreation2PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation3\",    \"Password\":\"FullTestUserCreation3PW\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation4\",    \"Password\":\"FullTestUserreation4PW\"}"
echo "Should return HTTP 201"
echo.

echo should fail:
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation1\", \"Password\":\"FullTestUserCreation1PW\"}"
echo "Should return HTTP 4xx - User already exists"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation3\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx - User already exists"
echo. 
echo.


echo 2) Login Users
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation1\", \"Password\":\"FullTestUserCreation1PW\"}"
echo "should return HTTP 200 with generated token for the user"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation2\", \"Password\":\"FullTestUserCreation2PW\"}"
echo "should return HTTP 200 with generated token for the user"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation3\",    \"Password\":\"FullTestUserCreation3PW\"}"
echo "should return HTTP 200 with generated token for the user"
echo.

if %pauseFlag%==1 pause

echo should fail:
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"FullTestUserCreation4\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx - Login failed"
echo.
echo.

if %pauseFlag%==1 pause

echo 3) edit user data
echo.
curl -i -X GET http://localhost:10001/users/ --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION1"
echo "Should return HTTP 200 - and current user data"
echo.
curl -i -X GET http://localhost:10001/users/ --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION2"
echo "Should return HTTP 200 - and current user data"
echo.
curl -i -X PUT http://localhost:10001/users/ --header "Content-Type: application/json" --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION1" -d "{\"Username\": \"FullTestUser1CreationNewName\",\"Password\": \"FullTestUser1CreationNewPassword\",\"Coins\": {\"Bronze\": 10,\"Diamond\": 5}}"
echo "Should return HTTP 2xx"
echo.
curl -i -X PUT http://localhost:10001/users/ --header "Content-Type: application/json" --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION2" -d "{\"Username\": \"FullTestUser2CreationNewName\",\"Password\": \"FullTestUser2CreationNewName\",\"Coins\": {\"Gold\": 1,\"Diamond\": 0}}"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/users/ --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION1"
echo "Should return HTTP 200 - and new user data"
echo.
curl -i -X GET http://localhost:10001/users/ --header "Authorization: Bearer ENTER TOKEN FROM DB FOR FULLTESTUSERCREATION2"
echo "Should return HTTP 200 - and new user data"
echo.
echo.