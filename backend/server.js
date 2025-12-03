const express = require('express');
const cors = require('cors');
const Database = require('better-sqlite3');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(express.json());

// Initialize SQLite database
// Use /data directory on Render (persistent disk) or local directory
const dbPath = process.env.NODE_ENV === 'production' 
  ? '/data/accounts.db' 
  : path.join(__dirname, 'accounts.db');
const db = new Database(dbPath);
console.log(`Database path: ${dbPath}`);

// Create accounts table if it doesn't exist
db.exec(`
  CREATE TABLE IF NOT EXISTS accounts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id TEXT UNIQUE NOT NULL,
    username TEXT UNIQUE NOT NULL,
    current_money INTEGER DEFAULT 1000,
    total_wins INTEGER DEFAULT 0,
    total_losses INTEGER DEFAULT 0,
    current_win_streak INTEGER DEFAULT 0,
    best_win_streak INTEGER DEFAULT 0,
    total_games_played INTEGER DEFAULT 0,
    total_money_won INTEGER DEFAULT 0,
    total_money_lost INTEGER DEFAULT 0,
    total_loaned INTEGER DEFAULT 0,
    last_updated TEXT DEFAULT CURRENT_TIMESTAMP,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP
  )
`);

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
    const account = db.prepare('SELECT * FROM accounts WHERE username = ?').get(username);
    
    if (!account) {
      return res.status(404).json({ error: 'Account not found' });
    }

    // Convert to camelCase for Unity
    res.json({
      accountId: account.account_id,
      username: account.username,
      currentMoney: account.current_money,
      totalWins: account.total_wins,
      totalLosses: account.total_losses,
      currentWinStreak: account.current_win_streak,
      bestWinStreak: account.best_win_streak,
      totalGamesPlayed: account.total_games_played,
      totalMoneyWon: account.total_money_won,
      totalMoneyLost: account.total_money_lost,
      totalLoaned: account.total_loaned,
      lastUpdated: account.last_updated
    });
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

    // Check if username already exists
    const existing = db.prepare('SELECT * FROM accounts WHERE username = ?').get(username);
    if (existing) {
      return res.status(409).json({ error: 'Username already exists' });
    }

    const stmt = db.prepare(`
      INSERT INTO accounts (account_id, username, current_money)
      VALUES (?, ?, ?)
    `);

    const result = stmt.run(accountId || generateId(), username, currentMoney);

    const newAccount = db.prepare('SELECT * FROM accounts WHERE id = ?').get(result.lastInsertRowid);

    res.status(201).json({
      accountId: newAccount.account_id,
      username: newAccount.username,
      currentMoney: newAccount.current_money,
      totalWins: newAccount.total_wins,
      totalLosses: newAccount.total_losses,
      currentWinStreak: newAccount.current_win_streak,
      bestWinStreak: newAccount.best_win_streak,
      totalGamesPlayed: newAccount.total_games_played,
      totalMoneyWon: newAccount.total_money_won,
      totalMoneyLost: newAccount.total_money_lost,
      totalLoaned: newAccount.total_loaned,
      lastUpdated: newAccount.last_updated
    });
  } catch (error) {
    console.error('Error creating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Update account by accountId
app.put('/api/accounts/:accountId', (req, res) => {
  try {
    const { accountId } = req.params;
    const {
      currentMoney,
      totalWins,
      totalLosses,
      currentWinStreak,
      bestWinStreak,
      totalGamesPlayed,
      totalMoneyWon,
      totalMoneyLost,
      totalLoaned
    } = req.body;

    const existing = db.prepare('SELECT * FROM accounts WHERE account_id = ?').get(accountId);
    if (!existing) {
      return res.status(404).json({ error: 'Account not found' });
    }

    const stmt = db.prepare(`
      UPDATE accounts SET
        current_money = COALESCE(?, current_money),
        total_wins = COALESCE(?, total_wins),
        total_losses = COALESCE(?, total_losses),
        current_win_streak = COALESCE(?, current_win_streak),
        best_win_streak = COALESCE(?, best_win_streak),
        total_games_played = COALESCE(?, total_games_played),
        total_money_won = COALESCE(?, total_money_won),
        total_money_lost = COALESCE(?, total_money_lost),
        total_loaned = COALESCE(?, total_loaned),
        last_updated = CURRENT_TIMESTAMP
      WHERE account_id = ?
    `);

    stmt.run(
      currentMoney,
      totalWins,
      totalLosses,
      currentWinStreak,
      bestWinStreak,
      totalGamesPlayed,
      totalMoneyWon,
      totalMoneyLost,
      totalLoaned,
      accountId
    );

    const updated = db.prepare('SELECT * FROM accounts WHERE account_id = ?').get(accountId);

    res.json({
      accountId: updated.account_id,
      username: updated.username,
      currentMoney: updated.current_money,
      totalWins: updated.total_wins,
      totalLosses: updated.total_losses,
      currentWinStreak: updated.current_win_streak,
      bestWinStreak: updated.best_win_streak,
      totalGamesPlayed: updated.total_games_played,
      totalMoneyWon: updated.total_money_won,
      totalMoneyLost: updated.total_money_lost,
      totalLoaned: updated.total_loaned,
      lastUpdated: updated.last_updated
    });
  } catch (error) {
    console.error('Error updating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Delete account (optional)
app.delete('/api/accounts/:accountId', (req, res) => {
  try {
    const { accountId } = req.params;
    const result = db.prepare('DELETE FROM accounts WHERE account_id = ?').run(accountId);
    
    if (result.changes === 0) {
      return res.status(404).json({ error: 'Account not found' });
    }

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

