# Account System Summary

## What Was Created

I've created a complete cloud-hosted account system for your Craps game with the following components:

### 1. **AccountData.cs**
- Serializable data structure for account information
- Stores: username, money, wins, losses, streaks, loans, etc.
- Handles JSON serialization/deserialization

### 2. **AccountService.cs**
- Manages cloud account operations
- Handles sign-in, account creation, and data synchronization
- Falls back to local storage if cloud API is unavailable
- Singleton pattern - persists across scenes

### 3. **LoginScreen.cs**
- UI controller for login/sign-in screen
- Handles username input, sign-in, and account creation
- Shows loading states and status messages
- Automatically navigates to home screen after successful login

### 4. **HomeScreen.cs**
- UI controller for home screen
- Displays account information (money, stats, loans)
- Provides navigation to game and sign-out functionality
- Auto-syncs account data when leaving screen

### 5. **AccountManager.cs** (Updated)
- Integrated with cloud account system
- Loads account data from AccountService
- Auto-syncs changes to cloud
- Maintains backward compatibility with local storage

## How It Works

### Flow:
1. **Login Screen** → User enters username → Sign in or Create Account
2. **AccountService** → Finds/creates account → Loads data
3. **AccountManager** → Receives account data → Updates local state
4. **Home Screen** → Displays account info → User can play game
5. **During Game** → AccountManager tracks changes → Auto-syncs to cloud
6. **Sign Out** → Syncs final state → Clears session

### Cloud Storage:
- Uses REST API endpoints (configurable)
- Falls back to local storage (PlayerPrefs) if API unavailable
- Accounts stored by username
- Data syncs automatically on changes

### Loan Tracking:
- Tracks total amount loaned to player
- Displayed on home screen
- Synced to cloud account

## Key Features

✅ **Sign In / Create Account** - Users can find existing accounts or create new ones
✅ **Cloud Hosted** - Account data stored in cloud (with local fallback)
✅ **Money Tracking** - Tracks current money, wins, losses
✅ **Loan Tracking** - Tracks total loans from bank
✅ **Auto Sync** - Changes automatically sync to cloud
✅ **Home Screen** - Centralized account display and navigation
✅ **Offline Support** - Works with local storage if cloud unavailable

## Next Steps

1. **Set up UI in Unity** - Follow `ACCOUNT_SETUP_GUIDE.md` to create login and home screens
2. **Configure Backend** (Optional) - Set up REST API or use local storage
3. **Test** - Create account, play game, verify data syncs

## Backend API Requirements

If you want to use a cloud backend, you'll need these endpoints:

- `GET /api/accounts/{username}` - Get account by username
- `POST /api/accounts` - Create new account (body: AccountData JSON)
- `PUT /api/accounts/{accountId}` - Update account (body: AccountData JSON)

The system will work with local storage if no backend is configured.

## Files Created

- `Assets/AccountData.cs` - Account data structure
- `Assets/AccountService.cs` - Cloud account service
- `Assets/LoginScreen.cs` - Login UI controller
- `Assets/HomeScreen.cs` - Home screen UI controller
- `Assets/AccountManager.cs` - Updated with cloud integration
- `Assets/ACCOUNT_SETUP_GUIDE.md` - Setup instructions
- `Assets/ACCOUNT_SYSTEM_SUMMARY.md` - This file

