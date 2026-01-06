# PowerShell script to push to GitHub
# Run this in PowerShell: .\push-to-github.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub Push Automation Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Git is installed
Write-Host "Checking Git installation..." -ForegroundColor Yellow
try {
    $gitVersion = git --version
    Write-Host $gitVersion -ForegroundColor Green
} catch {
    Write-Host "ERROR: Git is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Git from https://git-scm.com/" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host ""

# Initialize Git repository
Write-Host "Step 1: Initializing Git repository..." -ForegroundColor Yellow
git init
Write-Host ""

# Remove existing remote (if any) and add new one
Write-Host "Step 2: Setting up remote repository..." -ForegroundColor Yellow
git remote remove origin 2>$null
git remote add origin https://github.com/2022cs668-Waleed/Mess-management-system.git
Write-Host "Remote added: https://github.com/2022cs668-Waleed/Mess-management-system.git" -ForegroundColor Green
Write-Host ""

# Stage all files
Write-Host "Step 3: Staging all files..." -ForegroundColor Yellow
git add .
Write-Host "All files staged" -ForegroundColor Green
Write-Host ""

# Create commit
Write-Host "Step 4: Creating commit..." -ForegroundColor Yellow
git commit -m "Initial commit: Complete Mess Management System with Render deployment"
Write-Host ""

# Set main branch
Write-Host "Step 5: Setting main branch..." -ForegroundColor Yellow
git branch -M main
Write-Host "Branch set to main" -ForegroundColor Green
Write-Host ""

# Push to GitHub
Write-Host "Step 6: Pushing to GitHub..." -ForegroundColor Yellow
Write-Host ""
Write-Host "You will be prompted for GitHub credentials:" -ForegroundColor Cyan
Write-Host "Username: 2022cs668-Waleed" -ForegroundColor White
Write-Host "Password: Use your Personal Access Token (NOT your GitHub password)" -ForegroundColor White
Write-Host ""
Write-Host "If you don't have a Personal Access Token:" -ForegroundColor Yellow
Write-Host "1. Go to: https://github.com/settings/tokens" -ForegroundColor White
Write-Host "2. Click 'Generate new token (classic)'" -ForegroundColor White
Write-Host "3. Select 'repo' scope" -ForegroundColor White
Write-Host "4. Copy the generated token" -ForegroundColor White
Write-Host "5. Use it as your password below" -ForegroundColor White
Write-Host ""

git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCCESS! Code pushed to GitHub" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Repository: https://github.com/2022cs668-Waleed/Mess-management-system" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Visit your GitHub repository to verify" -ForegroundColor White
    Write-Host "2. Follow RENDER_DEPLOYMENT_GUIDE.md to deploy" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "PUSH FAILED" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues and solutions:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "If authentication failed:" -ForegroundColor Cyan
    Write-Host "- Create a Personal Access Token on GitHub" -ForegroundColor White
    Write-Host "- Use the token as your password" -ForegroundColor White
    Write-Host ""
    Write-Host "If repository already has content:" -ForegroundColor Cyan
    Write-Host "- Run: git push -u origin main --force" -ForegroundColor White
    Write-Host ""
    Write-Host "If you need to try again:" -ForegroundColor Cyan
    Write-Host "- Just run this script again: .\push-to-github.ps1" -ForegroundColor White
    Write-Host ""
}

Read-Host "Press Enter to exit"
