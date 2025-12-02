# Fix Bet Amount Increase/Decrease Buttons

## Quick Checklist

### 1. Verify Button References Are Assigned
1. Select `BettingUI` GameObject in Hierarchy
2. In Inspector, check the **Betting UI (Script)** component
3. Make sure these are assigned:
   - **Bet Amount Text** → Should have a TMP_Text assigned
   - **Increase Bet Button** → Should have a Button assigned
   - **Decrease Bet Button** → Should have a Button assigned

### 2. Create Bet Amount Display Text (if missing)
1. Right-click on your Canvas → **UI** → **Text - TextMeshPro**
2. Name it: `BetAmountText`
3. Position it where you want (top of screen, side panel, etc.)
4. Set text to: `Bet Amount: $10`
5. Make it visible and readable (font size 24-30)

### 3. Verify Buttons Are Connected
1. Select your `IncreaseBetButton` GameObject
2. In Inspector, check the **Button** component
3. Under **On Click ()**, there should be an event
4. If empty:
   - Click the **+** button
   - Drag `BettingUI` GameObject into the object field
   - Select: `BettingUI` → `OnIncreaseBet()`
5. Repeat for `DecreaseBetButton` → `OnDecreaseBet()`

**OR** (if using the BettingUI script):
- Make sure `BettingUI` component has the buttons assigned in Inspector
- The script connects them automatically in `Start()`

### 4. Test
1. Click Play
2. Click the `+` button
3. Check Console for debug messages
4. Check if BetAmountText updates

## Troubleshooting

### Buttons Not Working?
- Check Console for errors
- Verify `CrapsGameManager.Instance` exists (should see it in Hierarchy)
- Check that buttons are assigned in BettingUI component

### Text Not Updating?
- Verify `betAmountText` is assigned in BettingUI component
- Check Console for warnings about missing references
- Make sure the text GameObject is active

### Still Not Working?
- Check Console for debug messages when clicking buttons
- Verify the buttons are actually clickable (not blocked by other UI)
- Make sure EventSystem exists in scene

