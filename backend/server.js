const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(express.json());

// PostgreSQL connection
const pool = new Pool({
  connectionString: process.env.DATABASE_URL,
  ssl: process.env.NODE_ENV === 'production' ? { rejectUnauthorized: false } : false
});

// Initialize database table
async function initDatabase() {
  try {
    await pool.query(`
      CREATE TABLE IF NOT EXISTS accounts (
        id SERIAL PRIMARY KEY,
        account_id VARCHAR(255) UNIQUE NOT NULL,
        username VARCHAR(255) UNIQUE NOT NULL,
        current_money INTEGER DEFAULT 1000,
        total_wins INTEGER DEFAULT 0,
        total_losses INTEGER DEFAULT 0,
        current_win_streak INTEGER DEFAULT 0,
        best_win_streak INTEGER DEFAULT 0,
        total_games_played INTEGER DEFAULT 0,
        total_money_won INTEGER DEFAULT 0,
        total_money_lost INTEGER DEFAULT 0,
        total_loaned INTEGER DEFAULT 0,
        last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
      )
    `);
    console.log('Database initialized successfully');
  } catch (error) {
    console.error('Error initializing database:', error);
  }
}

// Initialize on startup
initDatabase();

// Health check endpoint
app.get('/', (req, res) => {
  res.json({ status: 'ok', message: 'Craps Game API is running!' });
});

app.get('/api/health', (req, res) => {
  res.json({ status: 'ok' });
});

// Get account by username
app.get('/api/accounts/:username', async (req, res) => {
  try {
    const { username } = req.params;
    const result = await pool.query(
      'SELECT * FROM accounts WHERE LOWER(username) = LOWER($1)',
      [username]
    );
    
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Account not found' });
    }

    const account = result.rows[0];
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
app.post('/api/accounts', async (req, res) => {
  try {
    const { accountId, username, currentMoney = 1000 } = req.body;

    if (!username) {
      return res.status(400).json({ error: 'Username is required' });
    }

    // Check if username already exists
    const existing = await pool.query(
      'SELECT * FROM accounts WHERE LOWER(username) = LOWER($1)',
      [username]
    );
    
    if (existing.rows.length > 0) {
      return res.status(409).json({ error: 'Username already exists' });
    }

    const newAccountId = accountId || generateId();
    const result = await pool.query(
      `INSERT INTO accounts (account_id, username, current_money)
       VALUES ($1, $2, $3)
       RETURNING *`,
      [newAccountId, username, currentMoney]
    );

    const account = result.rows[0];
    console.log(`Created account: ${username}`);
    
    res.status(201).json({
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
    console.error('Error creating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Update account by accountId
app.put('/api/accounts/:accountId', async (req, res) => {
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

    // Check if account exists
    const existing = await pool.query(
      'SELECT * FROM accounts WHERE account_id = $1',
      [accountId]
    );
    
    if (existing.rows.length === 0) {
      return res.status(404).json({ error: 'Account not found' });
    }

    const result = await pool.query(
      `UPDATE accounts SET
        current_money = COALESCE($1, current_money),
        total_wins = COALESCE($2, total_wins),
        total_losses = COALESCE($3, total_losses),
        current_win_streak = COALESCE($4, current_win_streak),
        best_win_streak = COALESCE($5, best_win_streak),
        total_games_played = COALESCE($6, total_games_played),
        total_money_won = COALESCE($7, total_money_won),
        total_money_lost = COALESCE($8, total_money_lost),
        total_loaned = COALESCE($9, total_loaned),
        last_updated = CURRENT_TIMESTAMP
      WHERE account_id = $10
      RETURNING *`,
      [
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
      ]
    );

    const account = result.rows[0];
    console.log(`Updated account: ${account.username}`);
    
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
    console.error('Error updating account:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Delete account (optional)
app.delete('/api/accounts/:accountId', async (req, res) => {
  try {
    const { accountId } = req.params;
    const result = await pool.query(
      'DELETE FROM accounts WHERE account_id = $1 RETURNING *',
      [accountId]
    );
    
    if (result.rows.length === 0) {
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
