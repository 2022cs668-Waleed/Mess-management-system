# ?? RENDER DEPLOYMENT GUIDE

## ? Configuration Complete!

Your application is now configured for deployment on Render with the following changes:

### ?? Changes Made:

1. **Added PostgreSQL Support** (`2022-CS-668.csproj`)
   - Added Npgsql.EntityFrameworkCore.PostgreSQL package

2. **Updated Program.cs**
   - Auto-detects environment (Development/Production)
   - Uses SQL Server locally, PostgreSQL on Render
   - Auto-runs migrations on startup in production
   - Configures secure cookies for HTTPS in production

3. **Created Deployment Files**
   - `render.yaml` - Render service configuration
   - `Dockerfile` - Container configuration
   - `.dockerignore` - Files to exclude from Docker build
   - `appsettings.Production.json` - Production settings

---

## ?? DEPLOYMENT STEPS

### Step 1: Prepare Your Code

```bash
# Install PostgreSQL package
dotnet restore

# Build to verify everything works
dotnet build

# Commit all changes to Git
git add .
git commit -m "Configure for Render deployment"
git push origin main
```

---

### Step 2: Create Render Account

1. Go to [render.com](https://render.com)
2. Sign up with GitHub
3. Authorize Render to access your repositories

---

### Step 3: Deploy on Render

#### Option A: Using render.yaml (Recommended)

1. **Create New Blueprint Instance**
   - Go to Dashboard ? New ? Blueprint Instance
   - Connect your GitHub repository
   - Render will detect `render.yaml`
   - Click "Apply"

2. **Render will automatically create:**
   - PostgreSQL database (mess-db)
   - Web service (mess-management-system)
   - Environment variables
   - DATABASE_URL connection

#### Option B: Manual Setup

1. **Create PostgreSQL Database**
   - Dashboard ? New ? PostgreSQL
   - Name: `mess-db`
   - Plan: Free
   - Click "Create Database"
   - Copy the "External Database URL"

2. **Create Web Service**
   - Dashboard ? New ? Web Service
   - Connect your GitHub repository
   - Settings:
     - **Name**: `mess-management-system`
     - **Runtime**: Docker
     - **Branch**: main
     - **Plan**: Free
   
3. **Add Environment Variables**
   ```
   ASPNETCORE_ENVIRONMENT = Production
   DATABASE_URL = <paste your PostgreSQL URL>
   ASPNETCORE_URLS = http://0.0.0.0:$PORT
   ```

4. **Deploy**
   - Click "Create Web Service"
   - Render will build and deploy automatically

---

### Step 4: Wait for Deployment

Render will:
1. ? Clone your repository
2. ? Build Docker image
3. ? Run database migrations
4. ? Seed initial data
5. ? Start your application

**Time**: 5-10 minutes for first deployment

---

## ?? DEFAULT LOGIN CREDENTIALS

After deployment, use these credentials:

**Admin:**
- Email: `admin@gmail.com`
- Password: `Admin@123`

**Students:**
- Email: `student1@gmail.com` / Password: `Student@123`
- Email: `student2@gmail.com` / Password: `Student@123`
- Email: `student3@gmail.com` / Password: `Student@123`

---

## ?? ACCESS YOUR APP

Your app will be available at:
```
https://mess-management-system.onrender.com
```

Or your custom URL from Render dashboard.

---

## ?? TROUBLESHOOTING

### Issue: Build Fails

**Solution**: Check Render logs
```bash
# In Render dashboard ? Logs tab
# Look for build errors
```

### Issue: Database Connection Error

**Solution**: Verify DATABASE_URL
1. Go to Database ? External Database URL
2. Copy the full URL
3. Update in Web Service ? Environment Variables

### Issue: Migrations Not Running

**Solution**: Manual migration
```bash
# In Render Shell (Dashboard ? Shell tab)
dotnet ef database update
```

### Issue: 502 Bad Gateway

**Solution**: Check startup logs
- Application might be starting up (wait 1-2 min)
- Check if PORT is correctly set in ASPNETCORE_URLS

---

## ?? MONITORING

### View Logs
- Dashboard ? Your Service ? Logs
- Real-time application logs
- Error tracking

### Check Metrics
- Dashboard ? Your Service ? Metrics
- CPU/Memory usage
- Request rate

### Database Status
- Dashboard ? Database ? Info
- Connection details
- Backup status

---

## ?? UPDATES & REDEPLOYMENT

### Automatic Deployment
```bash
# Any push to main branch triggers redeployment
git add .
git commit -m "Your changes"
git push origin main
```

### Manual Deployment
- Dashboard ? Your Service ? Manual Deploy
- Click "Deploy latest commit"

---

## ?? PRICING

**Free Tier Includes:**
- ? Web Service: 750 hours/month
- ? PostgreSQL: 90 days retention
- ? Auto-sleep after 15 min inactivity
- ? SSL certificate included
- ? Custom domain support

**Note**: Free services sleep after inactivity, first request may be slow (15-30 sec).

---

## ?? POST-DEPLOYMENT CHECKLIST

After deployment, verify:

- [ ] Application loads at Render URL
- [ ] Login page appears
- [ ] Can login with admin credentials
- [ ] Dashboard loads
- [ ] Database is seeded (menu items exist)
- [ ] Can mark attendance
- [ ] Can generate bills
- [ ] All features work correctly

---

## ?? IMPORTANT NOTES

1. **Database Persistence**
   - Free PostgreSQL keeps data for 90 days
   - Upgrade to paid plan for permanent storage

2. **Auto-Sleep**
   - Free services sleep after 15 min inactivity
   - First request after sleep takes 15-30 seconds

3. **Environment**
   - Application runs in Production mode
   - Debug info disabled
   - HTTPS enforced
   - Secure cookies enabled

4. **Logs**
   - Render keeps logs for 7 days on free tier
   - Check regularly for errors

---

## ?? USEFUL LINKS

- **Render Docs**: https://render.com/docs
- **.NET on Render**: https://render.com/docs/deploy-dotnet
- **PostgreSQL Guide**: https://render.com/docs/databases

---

## ? DEPLOYMENT READY

Your application is now configured and ready for deployment on Render!

Follow the steps above to deploy your Mess Management System to the cloud.

**Estimated Time**: 15-20 minutes total

---

**Need Help?**
- Check Render documentation
- Review application logs in Render dashboard
- Verify environment variables are set correctly
