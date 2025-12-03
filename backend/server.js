const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(express.json());

// Simple JSON file database (works on free tier without persistent disk)
// Note: Data persists in memory during runtime, saved to file for local dev
const DATA_FILE = path.join(__dirname, 'accounts.json');
let accounts = {};

// Load existing data (for local development)
function loadData() {
  try {
    if (fs.existsSync(DATA_FILE)) {
      const data = fs.readFileSync(DATA_FILE, 'utf8');
      accounts = JSON.parse(data);
      console.log(`Loaded ${Object.keys(accounts).length} accounts from file`);
    }
  } catch (error) {
    console.log('No existing data file, starting fresh');
    accounts = {};
  }
}

// Save data to file (for local development)
function saveData() {
  try {
    fs.writeFileSync(DATA_FILE, JSON.stringify(accounts, null, 2));
  } catch (error) {
    console.log('Could not save to file (normal on Render free tier)');
  }
}

// Load data on startup
loadData();


// Health check endpoint
app.get('/', (req, res) => {
  res.json({ status: 'ok', message: 'Craps Game API is running!' });
});

app.get('/api/health', (req, res) => {
  res.json({ status: 'ok' });
});

// Get account by username
app.get('/api/accounts/:username', (req, res) => {
  try {
    const { username } = req.params;
    const account = accounts[username.toLowerCase()];
    
    if (!account) {
      return res.status(404).json({ error: 'Account not found' });
    }

    res.json(account);
  } catch (error) {
    console.error('Error getting account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Create new account
app.post('/api/accounts', (req, res) => {
  try {
    const { accountId, username, currentMoney = 1000 } = req.body;

    if (!username) {
      return res.status(400).json({ error: 'Username is required' });
    }

    const usernameKey = username.toLowerCase();

    // Check if username already exists
    if (accounts[usernameKey]) {
      return res.status(409).json({ error: 'Username already exists' });
    }

    const newAccount = {
      accountId: accountId || generateId(),
      username: username,
      currentMoney: currentMoney,
      totalWins: 0,
      totalLosses: 0,
      currentWinStreak: 0,
      bestWinStreak: 0,
      totalGamesPlayed: 0,
      totalMoneyWon: 0,
      totalMoneyLost: 0,
      totalLoaned: 0,
      lastUpdated: new Date().toISOString()
    };

    accounts[usernameKey] = newAccount;
    saveData();

    console.log(`Created account: ${username}`);
    res.status(201).json(newAccount);
  } catch (error) {
    console.error('Error creating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Update account by accountId
app.put('/api/accounts/:accountId', (req, res) => {
  try {
    const { accountId } = req.params;
    const updates = req.body;

    // Find account by accountId
    let foundKey = null;
    for (const [key, account] of Object.entries(accounts)) {
      if (account.accountId === accountId) {
        foundKey = key;
        break;
      }
    }

    if (!foundKey) {
      return res.status(404).json({ error: 'Account not found' });
    }

    // Update fields
    const account = accounts[foundKey];
    if (updates.currentMoney !== undefined) account.currentMoney = updates.currentMoney;
    if (updates.totalWins !== undefined) account.totalWins = updates.totalWins;
    if (updates.totalLosses !== undefined) account.totalLosses = updates.totalLosses;
    if (updates.currentWinStreak !== undefined) account.currentWinStreak = updates.currentWinStreak;
    if (updates.bestWinStreak !== undefined) account.bestWinStreak = updates.bestWinStreak;
    if (updates.totalGamesPlayed !== undefined) account.totalGamesPlayed = updates.totalGamesPlayed;
    if (updates.totalMoneyWon !== undefined) account.totalMoneyWon = updates.totalMoneyWon;
    if (updates.totalMoneyLost !== undefined) account.totalMoneyLost = updates.totalMoneyLost;
    if (updates.totalLoaned !== undefined) account.totalLoaned = updates.totalLoaned;
    account.lastUpdated = new Date().toISOString();

    saveData();

    console.log(`Updated account: ${account.username}`);
    res.json(account);
  } catch (error) {
    console.error('Error updating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Delete account (optional)
app.delete('/api/accounts/:accountId', (req, res) => {
  try {
    const { accountId } = req.params;
    
    // Find and delete account by accountId
    let foundKey = null;
    for (const [key, account] of Object.entries(accounts)) {
      if (account.accountId === accountId) {
        foundKey = key;
        break;
      }
    }

    if (!foundKey) {
      return res.status(404).json({ error: 'Account not found' });
    }

    delete accounts[foundKey];
    saveData();

    res.json({ message: 'Account deleted successfully' });
  } catch (error) {
    console.error('Error deleting account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Generate unique ID
function generateId() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

// Start server
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
  console.log(`API endpoints:`);
  console.log(`  GET  /api/accounts/:username - Get account by username`);
  console.log(`  POST /api/accounts - Create new account`);
  console.log(`  PUT  /api/accounts/:accountId - Update account`);
});

