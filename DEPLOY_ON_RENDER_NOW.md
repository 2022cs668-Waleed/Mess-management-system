# ?? RENDER DEPLOYMENT - STEP BY STEP

## ? Prerequisites Complete
- ? Code pushed to GitHub
- ? Render account created with GitHub

---

## ?? DEPLOYMENT STEPS

### Step 1: Create PostgreSQL Database

1. **Login to Render Dashboard**
   - Go to: https://dashboard.render.com/

2. **Create Database**
   - Click: "New +" button (top right)
   - Select: "PostgreSQL"
   
3. **Configure Database**
   - **Name**: `mess-db`
   - **Database**: `mess_management_db`
   - **User**: `mess_user`
   - **Region**: Select closest to you (e.g., Singapore, Frankfurt)
   - **Plan**: **Free**
   
4. **Create Database**
   - Click: "Create Database"
   - Wait 2-3 minutes for creation
   
5. **Copy Connection Details**
   - Click on your database name
   - Scroll down to "Connections"
   - Copy the **"External Database URL"**
   - Save it somewhere - you'll need it!
   - It looks like: `postgres://mess_user:xxxxx@xxxxx.render.com/mess_management_db`

---

### Step 2: Create Web Service

1. **Create New Web Service**
   - Click: "New +" button
   - Select: "Web Service"
   
2. **Connect Repository**
   - Click: "Connect account" (if needed)
   - Find and select: `2022cs668-Waleed/Mess-management-system`
   - Click: "Connect"

3. **Configure Web Service**
   
   **Basic Settings:**
   - **Name**: `mess-management-system` (or any name you want)
   - **Region**: Same as database (e.g., Singapore)
   - **Branch**: `main`
   - **Runtime**: Select **"Docker"**
   
   **Build & Deploy:**
   - Render will auto-detect your Dockerfile
   - Leave everything as default
   
   **Instance Type:**
   - Select: **"Free"**

4. **Add Environment Variables**
   
   Click "Advanced" ? Scroll to "Environment Variables"
   
   Add these 3 variables:
   
   **Variable 1:**
   - **Key**: `ASPNETCORE_ENVIRONMENT`
   - **Value**: `Production`
   
   **Variable 2:**
   - **Key**: `DATABASE_URL`
   - **Value**: Paste the External Database URL you copied earlier
   
   **Variable 3:**
   - **Key**: `ASPNETCORE_URLS`
   - **Value**: `http://0.0.0.0:$PORT`

5. **Create Web Service**
   - Scroll down and click: "Create Web Service"

---

### Step 3: Wait for Deployment

Render will now:
1. ? Clone your repository
2. ? Build Docker image (5-7 minutes)
3. ? Run database migrations
4. ? Start your application

**Progress indicators:**
- "Building..." - Creating Docker image
- "Deploying..." - Starting application
- "Live" - ? Your app is running!

**Total time**: 8-12 minutes for first deployment

---

### Step 4: Access Your Application

1. **Get Your URL**
   - On the Web Service page, you'll see your URL
   - It will be: `https://mess-management-system-xxxx.onrender.com`
   - (or the name you chose)

2. **Open Your App**
   - Click the URL or copy and open in browser
   - **First load may take 15-30 seconds** (free tier cold start)

3. **Login**
   - Email: `admin@gmail.com`
   - Password: `Admin@123`

---

## ?? SUCCESS! Your App is Live!

You should now see:
- ? Login page loads
- ? Can login with admin credentials
- ? Dashboard appears
- ? All features working

---

## ?? MONITOR YOUR DEPLOYMENT

### Check Build Logs
- Go to: Web Service ? "Logs" tab
- Watch real-time deployment progress
- Look for: "Application started" message

### Check Database
- Go to: Database ? "Info" tab
- Verify it's "Available"

### View Metrics
- Web Service ? "Metrics" tab
- Monitor CPU, Memory, Request rate

---

## ?? TROUBLESHOOTING

### Issue: Build Failed

**Check Logs:**
- Web Service ? Logs tab
- Look for error messages

**Common Solutions:**
- Verify Dockerfile is correct
- Check if all packages are restored
- Ensure .csproj file is valid

### Issue: "Application Error" or 502 Bad Gateway

**Solutions:**
1. Check Environment Variables are set correctly
2. Verify DATABASE_URL is correct
3. Wait 1-2 minutes (app might be starting)
4. Check Logs for errors

**Verify Environment Variables:**
- Web Service ? Environment
- Make sure all 3 variables are set:
  - ASPNETCORE_ENVIRONMENT = Production
  - DATABASE_URL = postgres://...
  - ASPNETCORE_URLS = http://0.0.0.0:$PORT

### Issue: Database Connection Error

**Solutions:**
1. Verify DATABASE_URL is correct
2. Copy it from Database ? Info ? External Database URL
3. Update in Web Service ? Environment
4. Click "Manual Deploy" to restart

### Issue: 404 Not Found

**Solution:**
- Your app is running but routes not found
- Check if migrations ran: Web Service ? Shell
- Run: `dotnet ef database update`

---

## ?? REDEPLOY (After Making Changes)

When you make changes and push to GitHub:

1. **Automatic Redeployment:**
   - Render auto-detects push to main branch
   - Automatically rebuilds and deploys
   - Takes 5-8 minutes

2. **Manual Redeployment:**
   - Web Service ? "Manual Deploy"
   - Click "Deploy latest commit"

---

## ?? FREE TIER LIMITATIONS

- ? 750 hours/month of runtime
- ? PostgreSQL (90 days data retention)
- ? Auto-sleep after 15 minutes of inactivity
- ? First request after sleep: 15-30 seconds to wake up
- ? Free SSL certificate
- ? Custom domain support

**To Prevent Sleep:** Upgrade to paid plan ($7/month)

---

## ?? POST-DEPLOYMENT CHECKLIST

After successful deployment:

- [ ] App loads at Render URL
- [ ] Login page appears
- [ ] Can login with admin@gmail.com / Admin@123
- [ ] Dashboard loads
- [ ] Menu management works
- [ ] Can create menu items
- [ ] Can mark attendance
- [ ] Can generate bills
- [ ] Students can login
- [ ] Students can view menu
- [ ] Students can view bills

---

## ?? QUICK REFERENCE

### Your URLs:
- **GitHub**: https://github.com/2022cs668-Waleed/Mess-management-system
- **Render Dashboard**: https://dashboard.render.com/
- **Your App**: https://your-app-name.onrender.com (get from Render)

### Default Credentials:
**Admin:**
- Email: admin@gmail.com
- Password: Admin@123

**Students:**
- student1@gmail.com / Student@123
- student2@gmail.com / Student@123
- student3@gmail.com / Student@123

### Important Settings:
- Runtime: Docker
- Branch: main
- Region: Same for DB and Web Service
- Plan: Free

---

## ?? NEED HELP?

### Check These First:
1. Logs tab - for error messages
2. Events tab - for deployment history
3. Environment tab - verify all 3 variables set

### Common Commands in Shell:
```bash
# Check if app is running
ps aux

# View environment variables
printenv

# Run migrations manually
dotnet ef database update
```

---

## ?? CONGRATULATIONS!

Your Mess Management System is now:
- ? Live on the internet
- ? Accessible from anywhere
- ? Using PostgreSQL database
- ? Running in Docker container
- ? Secured with HTTPS

**Share your app URL with others!**

---

**Next Steps:**
1. Complete the deployment steps above
2. Test all features
3. Share with your users
4. Monitor logs for any issues

**Estimated Time**: 15-20 minutes total  
**Difficulty**: Easy ?  
**Success Rate**: 95%+  

---

**?? START NOW: Go to https://dashboard.render.com/ and follow Step 1!**
