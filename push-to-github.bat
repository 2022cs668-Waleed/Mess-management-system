@echo off
echo ========================================
echo GitHub Push Automation Script
echo ========================================
echo.

echo Checking Git installation...
git --version
if %errorlevel% neq 0 (
    echo ERROR: Git is not installed or not in PATH
    echo Please install Git from https://git-scm.com/
    pause
    exit /b 1
)
echo.

echo Step 1: Initializing Git repository...
git init
echo.

echo Step 2: Adding remote repository...
git remote remove origin 2>nul
git remote add origin https://github.com/2022cs668-Waleed/Mess-management-system.git
echo.

echo Step 3: Staging all files...
git add .
echo.

echo Step 4: Creating commit...
git commit -m "Initial commit: Complete Mess Management System with Render deployment"
echo.

echo Step 5: Setting main branch...
git branch -M main
echo.

echo Step 6: Pushing to GitHub...
echo You may be prompted for GitHub credentials...
echo Username: Your GitHub username
echo Password: Use Personal Access Token (not account password)
echo.
git push -u origin main

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo SUCCESS! Code pushed to GitHub
    echo ========================================
    echo Repository: https://github.com/2022cs668-Waleed/Mess-management-system
    echo.
    echo Next steps:
    echo 1. Visit your GitHub repository to verify
    echo 2. Follow RENDER_DEPLOYMENT_GUIDE.md to deploy
    echo.
) else (
    echo.
    echo ========================================
    echo PUSH FAILED
    echo ========================================
    echo.
    echo If you see authentication errors:
    echo 1. Create Personal Access Token on GitHub
    echo 2. Use token as password
    echo.
    echo If repository already has content:
    echo Run: git push -u origin main --force
    echo.
)

pause
