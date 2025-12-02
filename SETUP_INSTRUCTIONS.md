# Craps Game - Setup Instructions

## ✅ Completed Features

### Account System
- **AccountManager.cs**: Tracks money, wins, losses, win streaks, and statistics
- Persists data using PlayerPrefs (survives game restarts)
- Events system for UI updates

### Game Integration
- **Betting System**: Automatic betting on comeout rolls
- **Payout System**: 1:1 payout for pass line bets
- **Win/Lose Tracking**: Automatically records outcomes to account

### UI Components
- **AccountUI.cs**: Displays money, wins, losses, win rate, streaks
- **Win/Lose Messages**: Visual feedback with result panels
- **Game State Display**: Shows current game state and point

### Audio
- Win sound effects
- Lose sound effects
- Dice roll sounds (already implemented)

### Visual Effects
- Particle effects for win/lose outcomes
- Result panels with color-coded messages

### Bug Fixes
- Fixed GameManager.cs variable scope bug
- Removed unused NewMonoBehaviourScript.cs

---

## 🎮 Unity Setup Steps

### 1. AccountManager Setup
1. Create an empty GameObject in your scene (name it "AccountManager")
2. Add the `AccountManager` component to it
3. Set the `Starting Money` value (default: 1000)
4. The AccountManager will persist across scenes automatically

### 2. CrapsGameManager Setup
1. Find your existing CrapsGameManager GameObject
2. In the Inspector, assign the following:
   - **UI Elements**:
     - `Game State Text`: TMP_Text for game state
     - `Point Text`: TMP_Text for point value
     - `Result Text`: TMP_Text for win/lose messages
     - `Result Panel`: GameObject panel to show/hide results
   - **Betting**:
     - `Default Bet Amount`: Amount to bet automatically (default: 10)
     - `Pass Line Payout Multiplier`: Payout multiplier (default: 1.0 for 1:1)
   - **Effects**:
     - `Win Particles`: ParticleSystem for win effects
     - `Lose Particles`: ParticleSystem for lose effects

### 3. AccountUI Setup
1. Create a Canvas or use existing UI Canvas
2. Create UI Text elements (TMP_Text) for:
   - Money display
   - Wins count
   - Losses count
   - Win rate
   - Current win streak
   - Best win streak
   - Games played
3. Create an empty GameObject (name it "AccountUI")
4. Add the `AccountUI` component to it
5. Assign all the TMP_Text references in the Inspector

### 4. AudioPlayer Setup
1. Find your AudioPlayer GameObject
2. In the Inspector, assign:
   - `Win Sound`: AudioClip for win sound
   - `Lose Sound`: AudioClip for lose sound
   - (Existing dice roll sounds should already be assigned)

### 5. Result Panel Setup
1. Create a UI Panel GameObject (name it "ResultPanel")
2. Add a TMP_Text child for the result message
3. Assign the panel and text to CrapsGameManager's `Result Panel` and `Result Text`
4. Style the panel as desired (background, borders, etc.)
5. Initially set the panel to inactive (it will be shown/hidden by code)

### 6. Particle Effects Setup
1. Create two ParticleSystem GameObjects:
   - "WinParticles" - for win celebrations
   - "LoseParticles" - for lose effects
2. Configure particles as desired (sparks, confetti, etc.)
3. Assign them to CrapsGameManager's `Win Particles` and `Lose Particles` fields

---

## 🎯 How It Works

### Betting Flow
1. Player starts a comeout roll
2. System automatically places default bet (if player has enough money)
3. Dice are rolled
4. Outcome is determined:
   - **Win**: Money added, win recorded, particles/sounds play
   - **Loss**: Money deducted, loss recorded, particles/sounds play
5. Game resets after 3 seconds

### Account Persistence
- All account data saves automatically using PlayerPrefs
- Data persists between game sessions
- To reset: Use `AccountManager.Instance.ResetAllData()` in code or delete PlayerPrefs

### UI Updates
- AccountUI automatically updates when account data changes
- Uses event system for efficient updates
- No manual refresh needed

---

## 🔧 Customization Options

### Change Starting Money
- Edit `AccountManager` component: `Starting Money` field

### Change Bet Amount
- Edit `CrapsGameManager` component: `Default Bet Amount` field
- Or implement custom betting UI

### Change Payout Rate
- Edit `CrapsGameManager` component: `Pass Line Payout Multiplier`
- 1.0 = 1:1 payout (bet $10, win $10)
- 2.0 = 2:1 payout (bet $10, win $20)

### Add Custom Betting UI
- Create UI buttons for different bet amounts
- Call `CrapsGameManager.Instance.PlaceBet(amount)` when clicked
- Check `AccountManager.Instance.HasEnoughMoney(amount)` first

---

## 📝 Testing Checklist

- [ ] AccountManager persists money between scenes
- [ ] Money decreases when bet is placed
- [ ] Money increases on win
- [ ] Statistics update correctly (wins/losses)
- [ ] Win streak tracks correctly
- [ ] UI updates automatically
- [ ] Win/lose sounds play
- [ ] Particle effects trigger
- [ ] Result panel shows/hides correctly
- [ ] Game resets after win/lose

---

## 🐛 Troubleshooting

### AccountManager not found
- Make sure AccountManager GameObject exists in scene
- Check that it's not destroyed

### UI not updating
- Verify AccountUI component has all TMP_Text references assigned
- Check that AccountManager exists and is initialized

### No sound effects
- Verify AudioPlayer has win/lose sound clips assigned
- Check AudioPlayer GameObject exists in scene

### Particles not playing
- Verify ParticleSystem references are assigned in CrapsGameManager
- Check that particles are configured properly

### Betting not working
- Ensure player has enough money (check AccountManager)
- Verify CrapsGameManager is set up correctly

---

## 🚀 Next Steps (Optional Enhancements)

1. **Custom Betting UI**: Let players choose bet amounts
2. **Settings Menu**: Volume controls, graphics settings
3. **Statistics Screen**: Detailed stats view
4. **Achievements**: Unlock achievements for milestones
5. **Save/Load System**: Multiple save slots
6. **Tutorial**: On-screen instructions for new players

