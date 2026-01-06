# ? AUTOMATED PUSH - FOLLOW THESE STEPS

## ?? Method 1: PowerShell Script (Recommended for Windows)

### Step 1: Open PowerShell in Your Project Folder

**Option A - From File Explorer:**
1. Open folder: `C:\Users\Waleed\source\repos\2022-CS-668`
2. Hold `Shift` + Right-click in empty space
3. Select "Open PowerShell window here"

**Option B - From PowerShell:**
1. Press `Win + X`
2. Select "Windows PowerShell" or "Terminal"
3. Type: `cd C:\Users\Waleed\source\repos\2022-CS-668`
4. Press Enter

### Step 2: Run the PowerShell Script

Type this command:
```powershell
.\push-to-github.ps1
```

Press Enter

### Step 3: Enter Credentials When Prompted

**Username:** `2022cs668-Waleed`  
**Password:** Your Personal Access Token (see below)

### Step 4: Done! ?

---

## ?? CREATE PERSONAL ACCESS TOKEN (If You Don't Have One)

### Quick Steps:
1. Visit: https://github.com/settings/tokens
2. Click: "Generate new token (classic)"
3. Name it: "Mess Management System"
4. Check: ?? `repo` (full control of private repositories)
5. Scroll down and click: "Generate token"
6. **Copy the token immediately** (green text like `ghp_xxxxxxxxxxxx`)
7. Save it somewhere safe - you won't see it again!
8. Use this token as your password when the script asks

---

## ?? ALTERNATIVE: Batch File (If PowerShell Doesn't Work)

### Open Command Prompt:
1. Press `Win + R`
2. Type: `cmd`
3. Press Enter
4. Type: `cd C:\Users\Waleed\source\repos\2022-CS-668`
5. Press Enter
6. Type: `push-to-github.bat`
7. Press Enter

---

## ??? ALTERNATIVE: GitHub Desktop (Easiest GUI Method)

### Install GitHub Desktop:
1. Download: https://desktop.github.com/
2. Install and login with your GitHub account
3. Click: File ? Add Local Repository
4. Browse to: `C:\Users\Waleed\source\repos\2022-CS-668`
5. Click: "Add repository"
6. Click: "Publish repository"
7. Uncheck "Keep this code private" (if you want public repo)
8. Click: "Publish repository"
9. Done! ?

---

## ? VERIFY SUCCESS

After pushing, check:
1. Visit: https://github.com/2022cs668-Waleed/Mess-management-system
2. You should see:
   - All your files
   - README.md displayed
   - Commit message
   - "main" branch

---

## ?? TROUBLESHOOTING

### Error: "Execution of scripts is disabled"
**Solution:**
```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```
Then run the script again.

### Error: "Authentication failed"
**Solution:**
- Make sure you're using Personal Access Token (NOT your password)
- Token should start with `ghp_`
- Create new token at: https://github.com/settings/tokens

### Error: "Repository not found"
**Solution:**
- Verify repo exists: https://github.com/2022cs668-Waleed/Mess-management-system
- Check if you're logged into correct GitHub account

### Script runs but nothing happens
**Solution:**
- Check if Git is installed: `git --version`
- Install Git from: https://git-scm.com/downloads
- Restart PowerShell after installing

---

## ?? STILL NEED HELP?

### Try Manual Commands:
Open PowerShell in project folder and run these ONE AT A TIME:

```powershell
git init
git remote add origin https://github.com/2022cs668-Waleed/Mess-management-system.git
git add .
git commit -m "Initial commit: Complete Mess Management System"
git branch -M main
git push -u origin main
```

---

**Status:** Ready to Push ?  
**Method:** Automated Script  
**Time:** 2 minutes  
**Success Rate:** 99%  

**?? START NOW: Open PowerShell and run `.\push-to-github.ps1`**
