# Unity Setup Walkthrough - Step by Step

Follow these steps in order to set up your Craps game with the account system.

---

## Step 1: Set Up AccountManager

### 1.1 Create the GameObject
1. In Unity Hierarchy, right-click → **Create Empty**
2. Name it: `AccountManager`
3. Make sure it's in your **TableGame** scene (or whichever scene you're using)

### 1.2 Add the Component
1. Select the `AccountManager` GameObject
2. In Inspector, click **Add Component**
3. Search for: `Account Manager`
4. Click to add it

### 1.3 Configure Settings
1. In the AccountManager component:
   - **Starting Money**: Set to `1000` (or your preferred starting amount)
2. **Done!** The AccountManager will persist across scenes automatically

---

## Step 2: Set Up AccountUI (Money & Stats Display)

### 2.1 Create UI Canvas (if you don't have one)
1. Right-click in Hierarchy → **UI** → **Canvas**
2. Name it: `GameCanvas`
3. If prompted, also create an **EventSystem** (Unity does this automatically)

### 2.2 Create Money Display Text
1. Right-click on `GameCanvas` → **UI** → **Text - TextMeshPro**
2. If prompted, click **Import TMP Essentials** (first time only)
3. Name it: `MoneyText`
4. In Inspector:
   - **Text**: Change to `Money: $1,000`
   - **Font Size**: Set to `24` or `30` (adjust as needed)
   - **Alignment**: Center or Left (your preference)
   - Position it in top-left or top-right corner

### 2.3 Create Statistics Text Elements
Create these text elements (same process as above):

1. **WinsText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Wins: 0`
   - Position below MoneyText

2. **LossesText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Losses: 0`
   - Position below WinsText

3. **WinRateText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Win Rate: 0.0%`
   - Position below LossesText

4. **WinStreakText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Win Streak: 0`
   - Position below WinRateText

5. **BestStreakText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Best Streak: 0`
   - Position below WinStreakText

6. **GamesPlayedText** - Right-click Canvas → UI → Text - TextMeshPro
   - Text: `Games: 0`
   - Position below BestStreakText

**Tip**: You can organize these in a vertical layout or position them however looks good!

### 2.4 Create AccountUI Component
1. Right-click in Hierarchy → **Create Empty**
2. Name it: `AccountUI`
3. Select `AccountUI`
4. In Inspector, click **Add Component**
5. Search for: `Account UI`
6. Click to add it

### 2.5 Assign Text References
1. Select `AccountUI` GameObject
2. In the AccountUI component, you'll see fields for each text element
3. **Drag and drop** each TextMeshPro text from Hierarchy into the corresponding field:
   - Drag `MoneyText` → **Money Text** field
   - Drag `WinsText` → **Wins Text** field
   - Drag `LossesText` → **Losses Text** field
   - Drag `WinRateText` → **Win Rate Text** field
   - Drag `WinStreakText` → **Win Streak Text** field
   - Drag `BestStreakText` → **Best Streak Text** field
   - Drag `GamesPlayedText` → **Games Played Text** field

**Note**: You can leave some fields empty if you don't want to display that stat.

---

## Step 3: Set Up Result Panel (Win/Lose Messages)

### 3.1 Create Result Panel
1. Right-click on `GameCanvas` → **UI** → **Panel**
2. Name it: `ResultPanel`
3. In Inspector:
   - **Image Component**: 
     - Set **Color** to semi-transparent (e.g., black with alpha 200)
     - Or add a background image
   - **Rect Transform**: 
     - Set **Anchor Presets** to stretch-stretch (hold Alt while clicking)
     - This makes it cover the whole screen

### 3.2 Create Result Text
1. Right-click on `ResultPanel` → **UI** → **Text - TextMeshPro**
2. Name it: `ResultText`
3. In Inspector:
   - **Text**: `You Win!` (placeholder)
   - **Font Size**: `48` or larger (for visibility)
   - **Alignment**: Center (both horizontal and vertical)
   - **Color**: White or bright color
   - **Rect Transform**: Center it in the panel

### 3.3 Hide Panel Initially
1. Select `ResultPanel` in Hierarchy
2. In Inspector, **uncheck** the checkbox next to the GameObject name
   - This hides it initially (code will show it when needed)

---

## Step 4: Update CrapsGameManager

### 4.1 Find Your CrapsGameManager
1. In Hierarchy, find the GameObject that has the `Craps Game Manager` component
   - It might be named "CrapsGameManager" or "GameManager" or similar
2. Select it

### 4.2 Assign UI References
In the Craps Game Manager component, assign:

1. **Game State Text**: 
   - Drag your existing game state text (if you have one)
   - Or create a new TextMeshPro text for this

2. **Point Text**: 
   - Drag your existing point text (if you have one)
   - Or create a new TextMeshPro text for this

3. **Result Text**: 
   - Drag `ResultText` (the one inside ResultPanel)

4. **Result Panel**: 
   - Drag `ResultPanel` GameObject

### 4.3 Configure Betting Settings
In Craps Game Manager component:

1. **Default Bet Amount**: Set to `10` (or your preferred amount)
2. **Pass Line Payout Multiplier**: Set to `1` (for 1:1 payout)

### 4.4 Assign Particle Systems (Optional but Recommended)
1. Create two Particle Systems:
   - Right-click in Hierarchy → **Effects** → **Particle System**
   - Name one: `WinParticles`
   - Name another: `LoseParticles`

2. Configure WinParticles:
   - Select `WinParticles`
   - In Particle System component:
     - **Start Color**: Green or Gold
     - **Start Lifetime**: 2-3 seconds
     - **Start Speed**: 5-10
     - **Start Size**: 0.5-1.0
     - **Max Particles**: 100-200
     - **Shape**: Sphere or Cone
   - **Uncheck** "Play On Awake" (we'll trigger it manually)

3. Configure LoseParticles:
   - Select `LoseParticles`
   - Similar settings but:
     - **Start Color**: Red or Dark
     - **Max Particles**: 50-100 (less dramatic)

4. Assign to CrapsGameManager:
   - Select your CrapsGameManager GameObject
   - Drag `WinParticles` → **Win Particles** field
   - Drag `LoseParticles` → **Lose Particles** field

---

## Step 5: Update AudioPlayer

### 5.1 Find AudioPlayer GameObject
1. In Hierarchy, find the GameObject with `Audio Player` component
   - Might be named "AudioPlayer" or "AudioManager"

### 5.2 Assign Sound Clips
1. Select the AudioPlayer GameObject
2. In Inspector, find the Audio Player component
3. You should see fields for:
   - **Win Sound**: (new field)
   - **Lose Sound**: (new field)
   - Existing dice roll sounds

4. **Assign Audio Clips**:
   - In your `Assets/Ausio` folder (or wherever your audio files are)
   - Drag an audio file → **Win Sound** field
   - Drag an audio file → **Lose Sound** field
   - If you don't have win/lose sounds yet, you can leave these empty (they're optional)

---

## Step 6: Verify Setup

### 6.1 Check All Components
Make sure these GameObjects exist and have components:

- ✅ `AccountManager` - Has AccountManager component
- ✅ `AccountUI` - Has AccountUI component with text references assigned
- ✅ `ResultPanel` - Has ResultText child, initially hidden
- ✅ `CrapsGameManager` GameObject - Has Craps Game Manager component with all references
- ✅ `AudioPlayer` GameObject - Has Audio Player component

### 6.2 Test in Play Mode
1. Click **Play** button in Unity
2. Roll the dice
3. Check that:
   - Money decreases when you roll (bet is placed)
   - Win/lose message appears
   - Statistics update
   - Sounds play (if assigned)
   - Particles play (if assigned)

---

## Step 7: Organize Your Hierarchy (Optional)

Organize your GameObjects for easier management:

```
TableGame Scene
├── AccountManager
├── AccountUI
├── GameCanvas
│   ├── MoneyText
│   ├── WinsText
│   ├── LossesText
│   ├── WinRateText
│   ├── WinStreakText
│   ├── BestStreakText
│   ├── GamesPlayedText
│   └── ResultPanel
│       └── ResultText
├── CrapsGameManager (or whatever it's named)
├── DiceShooter
├── DiceManager
├── WinParticles
├── LoseParticles
├── AudioPlayer
└── ... (other game objects)
```

---

## Troubleshooting

### AccountUI not updating?
- Check that AccountManager exists in the scene
- Verify all text references are assigned in AccountUI component
- Check Console for errors

### Result panel not showing?
- Make sure ResultPanel and ResultText are assigned in CrapsGameManager
- Check that ResultPanel is a child of Canvas
- Verify the panel is initially hidden (unchecked)

### Money not changing?
- Check that AccountManager exists
- Verify CrapsGameManager has Default Bet Amount set
- Check Console for errors about insufficient funds

### Sounds not playing?
- Verify AudioPlayer GameObject exists
- Check that audio clips are assigned (can be empty, but no errors)
- Make sure AudioPlayer is not destroyed

---

## Quick Reference Checklist

- [ ] AccountManager created and configured
- [ ] AccountUI created with all text references assigned
- [ ] ResultPanel created with ResultText, initially hidden
- [ ] CrapsGameManager has all UI references assigned
- [ ] CrapsGameManager has betting settings configured
- [ ] Particle systems created and assigned (optional)
- [ ] AudioPlayer has win/lose sounds assigned (optional)
- [ ] Tested in Play mode - everything works!

---

**You're all set!** The game should now track money, display statistics, and show win/lose feedback.

