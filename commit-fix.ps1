# Quick commit and push for render.yaml fix
Write-Host "Committing render.yaml fix..." -ForegroundColor Cyan

git add render.yaml
git commit -m "Fix render.yaml: Remove startCommand and buildCommand for Docker runtime"
git push origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "SUCCESS! Changes pushed to GitHub" -ForegroundColor Green
    Write-Host "Render will automatically detect and redeploy" -ForegroundColor Yellow
} else {
    Write-Host "Push failed. Please check your credentials." -ForegroundColor Red
}

Read-Host "Press Enter to exit"
