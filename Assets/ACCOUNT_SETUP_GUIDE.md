# Account System Setup Guide

This guide will help you set up the login screen and home screen for your Craps game with cloud-hosted accounts.

## Overview

The account system includes:
- **AccountService**: Manages cloud account operations (sign in, create account, sync)
- **LoginScreen**: UI for signing in or creating accounts
- **HomeScreen**: UI for displaying account info and navigation
- **AccountData**: Serializable account data structure
- **AccountManager**: Updated to work with cloud accounts

## Step 1: Set Up AccountService

1. In Unity, create an empty GameObject in your Menu scene
2. Name it: `AccountService`
3. Add the `AccountService` component to it
4. In the Inspector, configure:
   - **API Base Url**: Set your backend API endpoint (or leave default for local storage)
   - **Use Local Storage**: Check this to use local storage as fallback

## Step 2: Set Up Login Screen

### Option A: Add Login UI to Existing Menu Scene

1. In your Menu scene, create a new Canvas (if you don't have one)
2. Create a Panel for the login screen:
   - Right-click Canvas → **UI** → **Panel**
   - Name it: `LoginPanel`
   - Set it to fill the screen

3. Create Login UI Elements:
   - **Username Input**: Right-click LoginPanel → **UI** → **Input Field - TextMeshPro**
     - Name it: `UsernameInput`
     - Set placeholder text: "Enter username"
   
   - **Sign In Button**: Right-click LoginPanel → **UI** → **Button - TextMeshPro**
     - Name it: `SignInButton`
     - Set button text: "Sign In"
   
   - **Create Account Button**: Right-click LoginPanel → **UI** → **Button - TextMeshPro**
     - Name it: `CreateAccountButton`
     - Set button text: "Create Account"
   
   - **Status Text**: Right-click LoginPanel → **UI** → **Text - TextMeshPro**
     - Name it: `StatusText`
     - Set text: "Enter your username to sign in or create a new account"
   
   - **Loading Panel** (optional): Right-click LoginPanel → **UI** → **Panel**
     - Name it: `LoadingPanel`
     - Add a TextMeshPro text child: `LoadingText`
     - Set text: "Loading..."
     - Initially disable this panel

4. Add LoginScreen Script:
   - Create an empty GameObject: `LoginScreenManager`
   - Add the `LoginScreen` component
   - In Inspector, assign:
     - **Username Input**: Drag `UsernameInput`
     - **Sign In Button**: Drag `SignInButton`
     - **Create Account Button**: Drag `CreateAccountButton`
     - **Status Text**: Drag `StatusText`
     - **Loading Panel**: Drag `LoadingPanel` (if created)
     - **Loading Text**: Drag `LoadingText` (if created)
     - **Home Scene Name**: Set to "Menu" (or your home scene name)

### Option B: Create Separate Login Scene

1. Create a new scene: **File** → **New Scene** → **Basic (Built-in)**
2. Save it as: `Assets/Scenes/Login.unity`
3. Follow steps from Option A to create the login UI
4. Add Login scene to Build Settings:
   - **File** → **Build Settings**
   - Click **Add Open Scenes**
   - Make sure Login scene is first in the list

## Step 3: Set Up Home Screen

### Update Menu Scene to Show Home Screen

1. In your Menu scene, create a Panel for home screen:
   - Right-click Canvas → **UI** → **Panel**
   - Name it: `HomePanel`
   - Initially disable LoginPanel if you added login UI to Menu scene

2. Create Account Display Elements:
   - **Username Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `UsernameText`
     - Set text: "Welcome!"
   
   - **Money Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `MoneyText`
     - Set text: "Money: $0"
   
   - **Wins Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `WinsText`
     - Set text: "Wins: 0"
   
   - **Losses Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `LossesText`
     - Set text: "Losses: 0"
   
   - **Win Rate Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `WinRateText`
     - Set text: "Win Rate: 0%"
   
   - **Win Streak Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `WinStreakText`
     - Set text: "Win Streak: 0"
   
   - **Total Loaned Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `TotalLoanedText`
     - Set text: "Total Loaned: $0"

3. Create Navigation Buttons:
   - **Play Game Button**: Right-click HomePanel → **UI** → **Button - TextMeshPro**
     - Name it: `PlayGameButton`
     - Set button text: "Play Game"
   
   - **Sign Out Button**: Right-click HomePanel → **UI** → **Button - TextMeshPro**
     - Name it: `SignOutButton`
     - Set button text: "Sign Out"
   
   - **Refresh Button** (optional): Right-click HomePanel → **UI** → **Button - TextMeshPro**
     - Name it: `RefreshButton`
     - Set button text: "Refresh"

4. Create Loan UI Elements:
   - **Request Loan Button**: Right-click HomePanel → **UI** → **Button - TextMeshPro**
     - Name it: `RequestLoanButton`
     - Set button text: "Request Loan ($500)"
   
   - **Repay Loan Button**: Right-click HomePanel → **UI** → **Button - TextMeshPro**
     - Name it: `RepayLoanButton`
     - Set button text: "Repay Loan"
   
   - **Loan Status Text**: Right-click HomePanel → **UI** → **Text - TextMeshPro**
     - Name it: `LoanStatusText`
     - Set text: "No debt. Good standing!"
   
   - **Repay Loan Panel**: Right-click HomePanel → **UI** → **Panel**
     - Name it: `RepayLoanPanel`
     - Set it to be smaller (popup style)
     - Initially disable it in Inspector
     - Add child elements:
       - **Repay Amount Input**: Right-click RepayLoanPanel → **UI** → **Input Field - TextMeshPro**
         - Name it: `RepayAmountInput`
         - Set Content Type to "Integer Number"
         - Set placeholder text: "Enter amount"
       - **Confirm Repay Button**: Right-click RepayLoanPanel → **UI** → **Button - TextMeshPro**
         - Name it: `ConfirmRepayButton`
         - Set button text: "Confirm"
       - **Cancel Repay Button**: Right-click RepayLoanPanel → **UI** → **Button - TextMeshPro**
         - Name it: `CancelRepayButton`
         - Set button text: "Cancel"

5. Add HomeScreen Script:
   - Create an empty GameObject: `HomeScreenManager`
   - Add the `HomeScreen` component
   - In Inspector, assign all UI elements:
     - **Username Text**: Drag `UsernameText`
     - **Money Text**: Drag `MoneyText`
     - **Wins Text**: Drag `WinsText`
     - **Losses Text**: Drag `LossesText`
     - **Win Rate Text**: Drag `WinRateText`
     - **Win Streak Text**: Drag `WinStreakText`
     - **Total Loaned Text**: Drag `TotalLoanedText`
     - **Play Game Button**: Drag `PlayGameButton`
     - **Sign Out Button**: Drag `SignOutButton`
     - **Refresh Button**: Drag `RefreshButton` (if created)
     - **Request Loan Button**: Drag `RequestLoanButton`
     - **Repay Loan Button**: Drag `RepayLoanButton`
     - **Repay Amount Input**: Drag `RepayAmountInput`
     - **Repay Loan Panel**: Drag `RepayLoanPanel`
     - **Confirm Repay Button**: Drag `ConfirmRepayButton`
     - **Cancel Repay Button**: Drag `CancelRepayButton`
     - **Loan Status Text**: Drag `LoanStatusText`
     - **Game Scene Name**: Set to "TableGame"

## Step 4: Update LevelManager (Optional)

If you want the menu to check for authentication:

1. Open `LevelManager.cs`
2. In `Start()` method, check if user is authenticated:
   ```csharp
   void Start()
   {
       // Check if user is authenticated
       if (AccountService.Instance == null || !AccountService.Instance.IsAuthenticated())
       {
           // Show login screen or redirect to login scene
           // You can disable HomePanel and enable LoginPanel here
       }
   }
   ```

## Step 5: Configure Backend API (Optional)

If you want to use a cloud backend:

1. Set up a REST API with these endpoints:
   - `GET /api/accounts/{username}` - Get account by username
   - `POST /api/accounts` - Create new account
   - `PUT /api/accounts/{accountId}` - Update account

2. Update `AccountService` API Base Url in Unity Inspector

3. If you don't have a backend, the system will use local storage (PlayerPrefs) as fallback

## Step 6: Test the System

1. **Test Login**:
   - Enter a username
   - Click "Create Account"
   - Should see success message and navigate to home screen

2. **Test Sign In**:
   - Enter an existing username
   - Click "Sign In"
   - Should load account data and navigate to home screen

3. **Test Home Screen**:
   - Check that account info displays correctly
   - Click "Play Game" - should load game scene
   - Play game and check that money updates
   - Return to menu - should see updated account info

4. **Test Cloud Sync**:
   - Play game and make changes
   - Sign out
   - Sign back in with same username
   - Should see updated account data

## Notes

- The system uses local storage (PlayerPrefs) as fallback if cloud API is unavailable
- Account data syncs automatically when changes are made
- Account data syncs when app is paused or loses focus (mobile)
- All account operations are logged to Unity Console for debugging

## Troubleshooting

- **Account not found**: Make sure you created the account first, or check local storage
- **Cloud sync not working**: Check API endpoint URL and network connection
- **UI not updating**: Make sure all UI elements are assigned in Inspector
- **Scene not loading**: Check scene names match in Build Settings

