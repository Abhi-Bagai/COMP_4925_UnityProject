# Craps Betting System Guide

## Overview
The game now supports multiple bet types with correct odds payouts. You can place bets on various numbers and adjust your bet amounts.

---

## Bet Types Available

### 1. **Pass Line Bet** (Required for Comeout Roll)
- **When**: Only on comeout roll
- **Payout**: 2:1 (bet $10, win $20 total)
- **Wins**: Natural (7 or 11) on comeout, or making the point
- **Loses**: Craps (2, 3, 12) on comeout, or seven out (7) after point established

### 2. **Place Bets** (Available when point is on)
Place bets win if your number is rolled before a 7.

- **Place 4**: Pays 9:5 (1.8:1) - Bet $10, win $18 total
- **Place 5**: Pays 7:5 (1.4:1) - Bet $10, win $14 total
- **Place 6**: Pays 7:6 (1.167:1) - Bet $12, win $14 total
- **Place 8**: Pays 7:6 (1.167:1) - Bet $12, win $14 total
- **Place 9**: Pays 7:5 (1.4:1) - Bet $10, win $14 total
- **Place 10**: Pays 9:5 (1.8:1) - Bet $10, win $18 total

**Note**: Place bets remain active until won or lost (on 7).

### 3. **Field Bet** (One Roll)
- **When**: Anytime
- **Wins**: 2, 3, 4, 9, 10, 11, 12
- **Payout**: 
  - 2 or 12: 2:1 (bet $10, win $20 total)
  - 3, 4, 9, 10, 11: 1:1 (bet $10, win $10 total - just bet back)
- **Loses**: 5, 6, 7, 8

### 4. **Any Craps** (One Roll)
- **When**: Anytime
- **Wins**: 2, 3, or 12
- **Payout**: 7:1 (bet $10, win $70 total)
- **Loses**: Any other number

### 5. **Hard Ways** (One Roll)
Hard ways win only if the number is rolled as a pair (both dice show same value).

- **Hard 4** (2+2): Pays 7:1 (bet $10, win $70 total)
- **Hard 6** (3+3): Pays 9:1 (bet $10, win $90 total)
- **Hard 8** (4+4): Pays 9:1 (bet $10, win $90 total)
- **Hard 10** (5+5): Pays 7:1 (bet $10, win $70 total)

**Loses**: If number is rolled the "easy way" (different dice values) or if 7 is rolled.

---

## Bet Amount Management

### Adjusting Bet Amount
- **Increase Bet**: Call `CrapsGameManager.Instance.IncreaseBetAmount()`
- **Decrease Bet**: Call `CrapsGameManager.Instance.DecreaseBetAmount()`
- **Get Current Amount**: `CrapsGameManager.Instance.GetCurrentBetAmount()`

### Bet Amount Settings
- **Default**: $10
- **Minimum**: $5 (configurable)
- **Maximum**: $1000 (configurable)
- **Increment**: $5 (configurable)

---

## Placing Bets

### Pass Line Bet (Required)
```csharp
// Automatically placed on comeout roll, or manually:
CrapsGameManager.Instance.PlacePassLineBet(10);
```

### Other Bets
```csharp
// Place bet using current bet amount
CrapsGameManager.Instance.PlaceBet(BetType.Place6);

// Place bet with specific amount
CrapsGameManager.Instance.PlaceBet(BetType.Field, 20);

// Increase existing place bet
CrapsGameManager.Instance.IncreaseBet(BetType.Place6, 10);
```

### Available Bet Types
- `BetType.PassLine` - Pass line bet
- `BetType.Place4`, `Place5`, `Place6`, `Place8`, `Place9`, `Place10` - Place bets
- `BetType.Field` - Field bet
- `BetType.AnyCraps` - Any craps bet
- `BetType.Hard4`, `Hard6`, `Hard8`, `Hard10` - Hard way bets

---

## How Bets Are Processed

1. **Pass Line Bet**: Processed first, determines game flow
2. **Place Bets**: Checked each roll, win if number rolled, lose on 7
3. **One-Roll Bets**: Processed immediately, removed after one roll
   - Field, Any Craps, Hard Ways

### Example Roll Flow
1. Player places Pass Line bet ($10) and Place 6 bet ($12)
2. Dice roll: 6
   - Pass Line: No change (point not established yet)
   - Place 6: **WINS** - Pays $14 (7:6 odds)
3. Next roll: Point established (8)
4. Next roll: 6
   - Pass Line: No change (waiting for point or 7)
   - Place 6: Already won, bet removed
5. Next roll: 7
   - Pass Line: **LOSES** - Seven out
   - All place bets: **LOSE** - Seven out

---

## Payout Summary

| Bet Type | Odds | Example ($10 bet) |
|----------|------|-------------------|
| Pass Line | 2:1 | Win $20 total |
| Place 4/10 | 9:5 (1.8:1) | Win $18 total |
| Place 5/9 | 7:5 (1.4:1) | Win $14 total |
| Place 6/8 | 7:6 (1.167:1) | Win $14 total (bet $12) |
| Field (2/12) | 2:1 | Win $20 total |
| Field (others) | 1:1 | Win $10 total |
| Any Craps | 7:1 | Win $70 total |
| Hard 4/10 | 7:1 | Win $70 total |
| Hard 6/8 | 9:1 | Win $90 total |

**Note**: All payouts include your original bet back. So "win $20 total" means you get your $10 bet back plus $10 profit.

---

## UI Integration

### Getting Active Bets
```csharp
List<ActiveBet> activeBets = CrapsGameManager.Instance.GetActiveBets();
foreach (var bet in activeBets)
{
    Debug.Log($"{bet.betType}: ${bet.amount}");
}
```

### Checking Game State
```csharp
bool canPlaceBets = CrapsGameManager.Instance.IsPointOn;
int currentBetAmount = CrapsGameManager.Instance.GetCurrentBetAmount();
```

---

## Code Examples

### Create Betting UI Buttons
```csharp
// Button for Place 6 bet
public void OnPlace6ButtonClick()
{
    CrapsGameManager.Instance.PlaceBet(BetType.Place6);
}

// Button to increase bet amount
public void OnIncreaseBetButtonClick()
{
    CrapsGameManager.Instance.IncreaseBetAmount();
}

// Button to decrease bet amount
public void OnDecreaseBetButtonClick()
{
    CrapsGameManager.Instance.DecreaseBetAmount();
}
```

### Display Active Bets
```csharp
void UpdateBetDisplay()
{
    var bets = CrapsGameManager.Instance.GetActiveBets();
    foreach (var bet in bets)
    {
        // Display bet info in UI
        Debug.Log($"{bet.betType}: ${bet.amount}");
    }
}
```

---

## Important Notes

1. **Pass Line Bet**: Required to start a round. Automatically placed if you have enough money.
2. **Place Bets**: Can only be placed when point is on. Remain active until won or lost.
3. **One-Roll Bets**: Automatically removed after one roll (win or lose).
4. **Seven Out**: Loses all place bets and pass line bet.
5. **Hard Ways**: Only win if rolled as exact pair. Lose if rolled "easy way" or on 7.

---

## Next Steps for UI

To create a betting UI, you'll need:
1. Buttons for each bet type
2. Bet amount adjustment buttons (+/-)
3. Display of active bets
4. Display of current bet amount
5. Validation to prevent invalid bets

The system is ready - just connect UI buttons to the methods above!

