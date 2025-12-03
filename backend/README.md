# Craps Game Backend API

Simple Node.js backend for the Craps game account system.

## Deploy to Render

### Option 1: Using render.yaml (Blueprint)

1. Push this `backend` folder to a GitHub repository
2. Go to [render.com](https://render.com) and sign up/login
3. Click **New** → **Blueprint**
4. Connect your GitHub repo
5. Render will auto-detect `render.yaml` and deploy

### Option 2: Manual Setup

1. Go to [render.com](https://render.com) and sign up/login
2. Click **New** → **Web Service**
3. Connect your GitHub repo
4. Configure:
   - **Name**: `craps-game-api`
   - **Region**: Oregon (or closest to you)
   - **Branch**: `main`
   - **Root Directory**: `backend` (if backend is in a subfolder)
   - **Runtime**: Node
   - **Build Command**: `npm install`
   - **Start Command**: `npm start`
   - **Plan**: Free

5. Add a **Disk** (for database persistence):
   - Click **Add Disk**
   - **Name**: `data`
   - **Mount Path**: `/data`
   - **Size**: 1 GB

6. Click **Create Web Service**

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Health check |
| GET | `/api/health` | Health check |
| GET | `/api/accounts/:username` | Get account by username |
| POST | `/api/accounts` | Create new account |
| PUT | `/api/accounts/:accountId` | Update account |
| DELETE | `/api/accounts/:accountId` | Delete account |

## Example Requests

### Create Account
```bash
curl -X POST https://your-app.onrender.com/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"username": "player1", "currentMoney": 1000}'
```

### Get Account
```bash
curl https://your-app.onrender.com/api/accounts/player1
```

### Update Account
```bash
curl -X PUT https://your-app.onrender.com/api/accounts/abc-123 \
  -H "Content-Type: application/json" \
  -d '{"currentMoney": 500, "totalWins": 5}'
```

## Unity Integration

After deploying, update your `AccountService.cs`:

```csharp
[SerializeField] private string apiBaseUrl = "https://your-app.onrender.com/api";
```

Replace `your-app` with your actual Render app name.

## Local Development

```bash
cd backend
npm install
npm start
```

Server runs on http://localhost:3000

## Notes

- Free tier on Render sleeps after 15 minutes of inactivity
- First request after sleep takes ~30 seconds to wake up
- Database is stored on persistent disk (survives restarts)

