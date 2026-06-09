@echo off
REM Quick launcher for football data scraping scripts

:menu
cls
echo ============================================================
echo          Football Data Scraper - Quick Launcher
echo ============================================================
echo.
echo 1. Fetch Premier League data (multiple seasons)
echo 2. Fetch specific league/season (custom)
echo 3. Fetch World Cup 2022 squads
echo 4. Fetch Champions League fixtures
echo 5. Edit configuration and run
echo 6. Exit
echo.
set /p choice="Enter your choice (1-6): "

if "%choice%"=="1" goto premier_league
if "%choice%"=="2" goto custom
if "%choice%"=="3" goto worldcup
if "%choice%"=="4" goto champions_league
if "%choice%"=="5" goto config
if "%choice%"=="6" goto end
goto menu

:premier_league
cls
echo Running Premier League multi-season fetcher...
python fetch_premier_league_multi_season.py
pause
goto menu

:custom
cls
echo.
echo Common League IDs:
echo   39 = Premier League
echo  140 = La Liga
echo  135 = Serie A
echo   78 = Bundesliga
echo   61 = Ligue 1
echo    2 = Champions League
echo    1 = World Cup
echo.
set /p league_id="Enter League ID: "
set /p season="Enter Season (2022-2024): "
set /p data_type="Enter data type (squad/fixtures): "
set /p output="Enter output filename: "

python fetch_data_generic.py --type %data_type% --league %league_id% --season %season% --output "%output%"
pause
goto menu

:worldcup
cls
echo Fetching World Cup 2022 squads...
if not exist "..\..\data\world-cup" mkdir "..\..\data\world-cup"
python fetch_data_generic.py --type squad --league 1 --season 2022 --output "..\..\data\world-cup\squads_2022.csv" --tournament --sleep 5
pause
goto menu

:champions_league
cls
echo Fetching Champions League 2023/24 fixtures...
if not exist "..\..\data\champions-league" mkdir "..\..\data\champions-league"
python fetch_data_generic.py --type fixtures --league 2 --season 2023 --output "..\..\data\champions-league\fixtures_2023-24.csv"
pause
goto menu

:config
cls
echo Opening fetch_data_config.py...
echo Edit the configuration at the top of the file and save.
echo.
pause
notepad fetch_data_config.py
echo.
set /p run="Run the script now? (y/n): "
if /i "%run%"=="y" (
    python fetch_data_config.py
    pause
)
goto menu

:end
echo.
echo Exiting...
exit
