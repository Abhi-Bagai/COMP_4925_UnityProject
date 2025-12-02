# Unity Betting UI Setup - Step by Step Guide

Follow these steps to add a complete betting interface to your Craps game.

---

## Step 1: Create Betting UI Canvas (if needed)

### 1.1 Check if you have a Canvas
1. In Hierarchy, look for a `Canvas` GameObject
2. If you don't have one:
   - Right-click in Hierarchy → **UI** → **Canvas**
   - Name it: `BettingCanvas` or use your existing `GameCanvas`

### 1.2 Set Canvas Settings
1. Select your Canvas
2. In Inspector, set **Render Mode** to **Screen Space - Overlay**
3. Make sure **Canvas Scaler** component is set appropriately

---

## Step 2: Create Bet Amount Display and Controls

### 2.1 Create Bet Amount Display
1. Right-click on Canvas → **UI** → **Text - TextMeshPro**
2. Name it: `BetAmountText`
3. Position it in a visible area (top-center or side)
4. Set text to: `Bet Amount: $10`
5. Adjust font size (24-30)

### 2.2 Create Increase Bet Button
1. Right-click on Canvas → **UI** → **Button - TextMeshPro**
2. Name it: `IncreaseBetButton`
3. Position next to BetAmountText
4. Change button text to: `+`
5. Make it a small square button (about 50x50 pixels)

### 2.3 Create Decrease Bet Button
1. Right-click on Canvas → **UI** → **Button - TextMeshPro**
2. Name it: `DecreaseBetButton`
3. Position next to IncreaseBetButton
4. Change button text to: `-`
5. Make it same size as IncreaseBetButton

---

## Step 3: Create Betting UI Manager Script

### 3.1 Create the Script
1. In Project window, right-click in `Assets` folder
2. **Create** → **C# Script**
3. Name it: `BettingUI.cs`

### 3.2 Add the Code
Open `BettingUI.cs` and replace with this code:

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BettingUI : MonoBehaviour
{
    [Header("Bet Amount Display")]
    public TMP_Text betAmountText;
    public Button increaseBetButton;
    public Button decreaseBetButton;

    [Header("Bet Buttons")]
    public Button place4Button;
    public Button place5Button;
    public Button place6Button;
    public Button place8Button;
    public Button place9Button;
    public Button place10Button;
    public Button fieldButton;
    public Button anyCrapsButton;
    public Button hard4Button;
    public Button hard6Button;
    public Button hard8Button;
    public Button hard10Button;

    [Header("Active Bets Display")]
    public TMP_Text activeBetsText;
    public GameObject activeBetsPanel; // Optional panel

    private void Start()
    {
        // Connect button events
        if (increaseBetButton != null)
            increaseBetButton.onClick.AddListener(OnIncreaseBet);
        
        if (decreaseBetButton != null)
            decreaseBetButton.onClick.AddListener(OnDecreaseBet);

        // Connect bet type buttons
        ConnectBetButton(place4Button, BetType.Place4);
        ConnectBetButton(place5Button, BetType.Place5);
        ConnectBetButton(place6Button, BetType.Place6);
        ConnectBetButton(place8Button, BetType.Place8);
        ConnectBetButton(place9Button, BetType.Place9);
        ConnectBetButton(place10Button, BetType.Place10);
        ConnectBetButton(fieldButton, BetType.Field);
        ConnectBetButton(anyCrapsButton, BetType.AnyCraps);
        ConnectBetButton(hard4Button, BetType.Hard4);
        ConnectBetButton(hard6Button, BetType.Hard6);
        ConnectBetButton(hard8Button, BetType.Hard8);
        ConnectBetButton(hard10Button, BetType.Hard10);

        // Initial update
        UpdateBetAmountDisplay();
        UpdateActiveBetsDisplay();
    }

    private void Update()
    {
        // Update displays every frame (or use events for better performance)
        UpdateBetAmountDisplay();
        UpdateActiveBetsDisplay();
        UpdateButtonStates();
    }

    private void ConnectBetButton(Button button, BetType betType)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => OnPlaceBet(betType));
        }
    }

    private void OnIncreaseBet()
    {
        if (CrapsGameManager.Instance != null)
        {
            CrapsGameManager.Instance.IncreaseBetAmount();
        }
    }

    private void OnDecreaseBet()
    {
        if (CrapsGameManager.Instance != null)
        {
            CrapsGameManager.Instance.DecreaseBetAmount();
        }
    }

    private void OnPlaceBet(BetType betType)
    {
        if (CrapsGameManager.Instance != null)
        {
            bool success = CrapsGameManager.Instance.PlaceBet(betType);
            if (!success)
            {
                Debug.LogWarning($"Could not place {betType} bet. Check if bet is valid for current game state.");
            }
        }
    }

    private void UpdateBetAmountDisplay()
    {
        if (betAmountText != null && CrapsGameManager.Instance != null)
        {
            int amount = CrapsGameManager.Instance.GetCurrentBetAmount();
            betAmountText.text = $"Bet Amount: ${amount}";
        }
    }

    private void UpdateActiveBetsDisplay()
    {
        if (activeBetsText != null && CrapsGameManager.Instance != null)
        {
            var bets = CrapsGameManager.Instance.GetActiveBets();
            if (bets.Count == 0)
            {
                activeBetsText.text = "No active bets";
            }
            else
            {
                string betList = "Active Bets:\n";
                foreach (var bet in bets)
                {
                    betList += $"{bet.betType}: ${bet.amount}\n";
                }
                activeBetsText.text = betList;
            }
        }
    }

    private void UpdateButtonStates()
    {
        if (CrapsGameManager.Instance == null) return;

        bool pointIsOn = CrapsGameManager.Instance.IsPointOn;
        bool canBet = !CrapsGameManager.Instance.IsRollInProgress;

        // Place bets only available when point is on
        SetButtonEnabled(place4Button, pointIsOn && canBet);
        SetButtonEnabled(place5Button, pointIsOn && canBet);
        SetButtonEnabled(place6Button, pointIsOn && canBet);
        SetButtonEnabled(place8Button, pointIsOn && canBet);
        SetButtonEnabled(place9Button, pointIsOn && canBet);
        SetButtonEnabled(place10Button, pointIsOn && canBet);

        // One-roll bets available anytime (when not rolling)
        SetButtonEnabled(fieldButton, canBet);
        SetButtonEnabled(anyCrapsButton, canBet);
        SetButtonEnabled(hard4Button, canBet);
        SetButtonEnabled(hard6Button, canBet);
        SetButtonEnabled(hard8Button, canBet);
        SetButtonEnabled(hard10Button, canBet);
    }

    private void SetButtonEnabled(Button button, bool enabled)
    {
        if (button != null)
        {
            button.interactable = enabled;
        }
    }
}
```

---

## Step 4: Create Bet Type Buttons

### 4.1 Create Place Bet Buttons
For each place bet (4, 5, 6, 8, 9, 10):

1. Right-click on Canvas → **UI** → **Button - TextMeshPro**
2. Name it: `Place4Button` (or Place5Button, etc.)
3. Change button text to: `Place 4` (or corresponding number)
4. Position buttons in a grid or vertical list
5. Repeat for all 6 place bets

**Tip**: Group them visually - maybe in a panel labeled "Place Bets"

### 4.2 Create One-Roll Bet Buttons
1. **Field Button**: Right-click Canvas → **UI** → **Button - TextMeshPro**
   - Name: `FieldButton`
   - Text: `Field`

2. **Any Craps Button**: Right-click Canvas → **UI** → **Button - TextMeshPro**
   - Name: `AnyCrapsButton`
   - Text: `Any Craps`

3. **Hard Way Buttons**: Create 4 buttons
   - `Hard4Button` - Text: `Hard 4`
   - `Hard6Button` - Text: `Hard 6`
   - `Hard8Button` - Text: `Hard 8`
   - `Hard10Button` - Text: `Hard 10`

**Tip**: Group one-roll bets together, maybe label the section "One Roll Bets"

---

## Step 5: Create Active Bets Display

### 5.1 Create Active Bets Text
1. Right-click on Canvas → **UI** → **Text - TextMeshPro**
2. Name it: `ActiveBetsText`
3. Position it on the side or bottom of screen
4. Set text to: `Active Bets:`
5. Set alignment to **Upper Left**
6. Make it a reasonable size (maybe 200x300 pixels)

### 5.2 Optional: Create Panel Background
1. Right-click on Canvas → **UI** → **Panel**
2. Name it: `ActiveBetsPanel`
3. Position it behind ActiveBetsText
4. Make it semi-transparent
5. Set ActiveBetsText as child of panel

---

## Step 6: Set Up BettingUI Component

### 6.1 Create BettingUI GameObject
1. Right-click in Hierarchy → **Create Empty**
2. Name it: `BettingUI`
3. Select it

### 6.2 Add BettingUI Component
1. In Inspector, click **Add Component**
2. Search for: `Betting UI`
3. Click to add it

### 6.3 Assign All References
In the BettingUI component, drag and drop:

**Bet Amount Display:**
- `Bet Amount Text` → drag `BetAmountText`
- `Increase Bet Button` → drag `IncreaseBetButton`
- `Decrease Bet Button` → drag `DecreaseBetButton`

**Bet Buttons:**
- `Place 4 Button` → drag `Place4Button`
- `Place 5 Button` → drag `Place5Button`
- `Place 6 Button` → drag `Place6Button`
- `Place 8 Button` → drag `Place8Button`
- `Place 9 Button` → drag `Place9Button`
- `Place 10 Button` → drag `Place10Button`
- `Field Button` → drag `FieldButton`
- `Any Craps Button` → drag `AnyCrapsButton`
- `Hard 4 Button` → drag `Hard4Button`
- `Hard 6 Button` → drag `Hard6Button`
- `Hard 8 Button` → drag `Hard8Button`
- `Hard 10 Button` → drag `Hard10Button`

**Active Bets Display:**
- `Active Bets Text` → drag `ActiveBetsText`
- `Active Bets Panel` → drag `ActiveBetsPanel` (if you created it)

**Note**: You can leave fields empty if you don't want that button/display.

---

## Step 7: Organize Your UI Layout

### Suggested Layout:
```
Canvas
├── Top Section
│   ├── BetAmountText
│   ├── IncreaseBetButton
│   └── DecreaseBetButton
│
├── Left Section (Place Bets)
│   ├── Place4Button
│   ├── Place5Button
│   ├── Place6Button
│   ├── Place8Button
│   ├── Place9Button
│   └── Place10Button
│
├── Center Section (One-Roll Bets)
│   ├── FieldButton
│   ├── AnyCrapsButton
│   ├── Hard4Button
│   ├── Hard6Button
│   ├── Hard8Button
│   └── Hard10Button
│
└── Right Section (Active Bets)
    ├── ActiveBetsPanel (optional)
    └── ActiveBetsText
```

### Layout Tips:
- Use **Layout Groups** (Horizontal/Vertical) to organize buttons
- Use **Content Size Fitter** to auto-size panels
- Use **Grid Layout Group** for button grids
- Anchor UI elements appropriately for different screen sizes

---

## Step 8: Test Your Betting UI

### 8.1 Test Bet Amount Adjustment
1. Click Play in Unity
2. Click `+` button - bet amount should increase
3. Click `-` button - bet amount should decrease
4. Check that BetAmountText updates

### 8.2 Test Place Bets
1. Start a game (roll dice to establish point)
2. Once point is on, place bets should be enabled
3. Click a place bet button (e.g., Place 6)
4. Check Active Bets display shows your bet
5. Check that money decreased

### 8.3 Test One-Roll Bets
1. Click Field or Any Craps button
2. Check Active Bets display
3. Roll dice
4. Check if bet won/lost and was removed

### 8.4 Test Button States
1. During comeout roll - place bets should be disabled
2. When point is on - place bets should be enabled
3. While rolling - all bets should be disabled

---

## Step 9: Polish and Enhancements

### 9.1 Add Visual Feedback
- Change button color when disabled (gray)
- Highlight active bets
- Show bet amounts on buttons
- Add tooltips explaining each bet

### 9.2 Add Sound Effects
- Play sound when bet is placed
- Play sound when bet amount changes
- Play sound when bet wins/loses

### 9.3 Improve Active Bets Display
- Show bet type, amount, and status
- Color-code winning bets (green) vs losing bets (red)
- Add icons for different bet types

### 9.4 Add Bet History
- Show recent bets
- Show win/loss history
- Display total winnings from side bets

---

## Troubleshooting

### Buttons Not Working?
- Check that BettingUI component has all button references assigned
- Check Console for errors
- Verify CrapsGameManager exists in scene

### Bets Not Showing?
- Check ActiveBetsText reference is assigned
- Verify CrapsGameManager.GetActiveBets() is working
- Check that bets are actually being placed (money decreases)

### Buttons Always Disabled?
- Check UpdateButtonStates() logic
- Verify CrapsGameManager.Instance exists
- Check game state (point on/off, rolling status)

### Money Not Decreasing?
- Verify AccountManager exists in scene
- Check that player has enough money
- Check Console for warnings about insufficient funds

---

## Quick Reference

### BettingUI Methods Called:
- `CrapsGameManager.Instance.IncreaseBetAmount()`
- `CrapsGameManager.Instance.DecreaseBetAmount()`
- `CrapsGameManager.Instance.PlaceBet(BetType)`
- `CrapsGameManager.Instance.GetCurrentBetAmount()`
- `CrapsGameManager.Instance.GetActiveBets()`
- `CrapsGameManager.Instance.IsPointOn`
- `CrapsGameManager.Instance.IsRollInProgress`

### Bet Types Available:
- `BetType.Place4`, `Place5`, `Place6`, `Place8`, `Place9`, `Place10`
- `BetType.Field`
- `BetType.AnyCraps`
- `BetType.Hard4`, `Hard6`, `Hard8`, `Hard10`

---

## Next Steps

Once your betting UI is working:
1. Add visual polish (colors, animations)
2. Add sound effects
3. Create a betting tutorial/help panel
4. Add bet history display
5. Create a betting statistics panel

Your betting system is now ready to use! 🎲

