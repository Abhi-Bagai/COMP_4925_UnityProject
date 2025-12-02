using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CrapsGameManager : MonoBehaviour
{
    public static CrapsGameManager Instance;

    [Header("UI")]
    public TMP_Text gameStateText; // Optional: display game state
    public TMP_Text pointText;     // Optional: display point value
    public TMP_Text resultText;    // Win/lose message
    public GameObject resultPanel; // Panel to show win/lose result

    [Header("Betting")]
    [SerializeField] private int defaultBetAmount = 10;
    [SerializeField] private int minBetAmount = 5;
    [SerializeField] private int maxBetAmount = 1000;
    [SerializeField] private int betIncrement = 5; // Amount to increase/decrease bet by
    
    private int currentBetAmount = 10; // Current bet amount selector
    private int passLineBet = 0; // Pass line bet (required for comeout)
    private bool passLineBetPlaced = false;
    
    // Active bets list
    private List<ActiveBet> activeBets = new List<ActiveBet>();

    [Header("Effects")]
    [SerializeField] private ParticleSystem winParticles;
    [SerializeField] private ParticleSystem loseParticles;

    [Header("Payout Settings")]
    [SerializeField] private float comeoutPayoutMultiplier = 2.0f; // 2:1 payout for natural wins (7 or 11) - returns bet + profit
    [SerializeField] private float pointPayoutMultiplier = 2.0f; // 2:1 payout for making the point - returns bet + profit

    // Game state
    private bool pointIsOn = false;  // Point starts as off
    private int pointValue = 0;      // The point number (4, 5, 6, 8, 9, or 10)
    private bool isComeoutRoll = true; // First roll is always comeout
    private bool rollInProgress = false; // Track if dice are currently rolling
    private bool canRoll = true;     // Whether player can roll (one roll per turn)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Initialize game state - point is off, ready for comeout roll
        ResetGameState();
        
        // Hide result panel initially
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        if (resultText != null)
        {
            resultText.text = "";
        }

        // Subscribe to out of money event
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnOutOfMoney += HandleOutOfMoney;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnOutOfMoney -= HandleOutOfMoney;
        }
    }

    /// <summary>
    /// Resets the game to initial state (point off, ready for comeout roll)
    /// </summary>
    public void ResetGameState()
    {
        pointIsOn = false;
        pointValue = 0;
        isComeoutRoll = true;
        rollInProgress = false;
        canRoll = true;
        passLineBetPlaced = false;
        passLineBet = 0;
        
        // Clear all bets (one-roll bets are cleared, place bets persist until won/lost)
        ClearOneRollBets();
        
        HideResult();
        UpdateUI();
    }

    /// <summary>
    /// Called by DiceShooter to check if rolling is allowed
    /// </summary>
    public bool CanRoll()
    {
        // Check if pass line bet is placed (only required for comeout roll)
        if (isComeoutRoll && !passLineBetPlaced)
        {
            // Auto-place default bet if player has enough money
            if (AccountManager.Instance != null && AccountManager.Instance.HasEnoughMoney(defaultBetAmount))
            {
                PlacePassLineBet(defaultBetAmount);
            }
            else
            {
                return false; // Can't roll without a pass line bet
            }
        }
        return canRoll && !rollInProgress;
    }

    #region Bet Amount Management

    /// <summary>
    /// Increases the current bet amount
    /// </summary>
    public void IncreaseBetAmount()
    {
        currentBetAmount = Mathf.Min(currentBetAmount + betIncrement, maxBetAmount);
        UpdateUI();
    }

    /// <summary>
    /// Decreases the current bet amount
    /// </summary>
    public void DecreaseBetAmount()
    {
        currentBetAmount = Mathf.Max(currentBetAmount - betIncrement, minBetAmount);
        UpdateUI();
    }

    /// <summary>
    /// Gets the current bet amount
    /// </summary>
    public int GetCurrentBetAmount() => currentBetAmount;

    #endregion

    #region Bet Placement

    /// <summary>
    /// Places a pass line bet (required for comeout roll)
    /// </summary>
    public bool PlacePassLineBet(int amount)
    {
        if (AccountManager.Instance == null)
        {
            Debug.LogWarning("AccountManager not found. Betting disabled.");
            return false;
        }

        if (passLineBetPlaced)
        {
            Debug.LogWarning("Pass line bet already placed for this round.");
            return false;
        }

        if (!isComeoutRoll)
        {
            Debug.LogWarning("Can only place pass line bet on comeout roll.");
            return false;
        }

        if (AccountManager.Instance.RemoveMoney(amount))
        {
            passLineBet = amount;
            passLineBetPlaced = true;
            Debug.Log($"Pass line bet placed: ${amount}");
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Places a bet using the current bet amount
    /// </summary>
    public bool PlaceBet(BetType betType)
    {
        return PlaceBet(betType, currentBetAmount);
    }

    /// <summary>
    /// Places a bet of specified type and amount
    /// </summary>
    public bool PlaceBet(BetType betType, int amount)
    {
        if (AccountManager.Instance == null)
        {
            Debug.LogWarning("AccountManager not found. Betting disabled.");
            return false;
        }

        // Check if bet is valid for current game state
        if (!IsBetValid(betType))
        {
            Debug.LogWarning($"Bet type {betType} is not valid in current game state.");
            return false;
        }

        // Check if bet already exists (for place bets)
        if (IsPlaceBet(betType) && HasActiveBet(betType))
        {
            Debug.LogWarning($"Bet on {betType} already exists. Increase bet instead.");
            return false;
        }

        if (AccountManager.Instance.RemoveMoney(amount))
        {
            int targetNumber = GetBetTargetNumber(betType);
            activeBets.Add(new ActiveBet(betType, amount, targetNumber));
            Debug.Log($"Bet placed: {betType} - ${amount}");
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Increases an existing place bet
    /// </summary>
    public bool IncreaseBet(BetType betType, int additionalAmount)
    {
        if (AccountManager.Instance == null) return false;

        ActiveBet bet = activeBets.FirstOrDefault(b => b.betType == betType);
        if (bet == null)
        {
            Debug.LogWarning($"No active bet found for {betType}");
            return false;
        }

        if (AccountManager.Instance.RemoveMoney(additionalAmount))
        {
            bet.amount += additionalAmount;
            Debug.Log($"Bet increased: {betType} - Total: ${bet.amount}");
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a bet type is valid for current game state
    /// </summary>
    private bool IsBetValid(BetType betType)
    {
        // Pass line bet only on comeout
        if (betType == BetType.PassLine)
        {
            return isComeoutRoll && !passLineBetPlaced;
        }

        // Place bets only when point is on
        if (IsPlaceBet(betType))
        {
            return pointIsOn;
        }

        // One-roll bets can be placed anytime
        return true;
    }

    /// <summary>
    /// Checks if bet type is a place bet
    /// </summary>
    private bool IsPlaceBet(BetType betType)
    {
        return betType == BetType.Place4 || betType == BetType.Place5 || 
               betType == BetType.Place6 || betType == BetType.Place8 || 
               betType == BetType.Place9 || betType == BetType.Place10;
    }

    /// <summary>
    /// Checks if there's an active bet of this type
    /// </summary>
    private bool HasActiveBet(BetType betType)
    {
        return activeBets.Any(b => b.betType == betType);
    }

    /// <summary>
    /// Gets the target number for a bet type
    /// </summary>
    private int GetBetTargetNumber(BetType betType)
    {
        switch (betType)
        {
            case BetType.Place4: return 4;
            case BetType.Place5: return 5;
            case BetType.Place6: return 6;
            case BetType.Place8: return 8;
            case BetType.Place9: return 9;
            case BetType.Place10: return 10;
            case BetType.Hard4: return 4;
            case BetType.Hard6: return 6;
            case BetType.Hard8: return 8;
            case BetType.Hard10: return 10;
            default: return 0;
        }
    }

    /// <summary>
    /// Clears one-roll bets (field, any craps, hard ways)
    /// </summary>
    private void ClearOneRollBets()
    {
        activeBets.RemoveAll(b => 
            b.betType == BetType.Field || 
            b.betType == BetType.AnyCraps || 
            b.betType == BetType.Hard4 || 
            b.betType == BetType.Hard6 || 
            b.betType == BetType.Hard8 || 
            b.betType == BetType.Hard10);
    }

    #endregion

    /// <summary>
    /// Called by DiceShooter when dice are shot - marks roll as in progress
    /// </summary>
    public void OnRollStarted()
    {
        rollInProgress = true;
        canRoll = false; // Prevent rolling again until this roll is complete
        UpdateUI();
    }

    /// <summary>
    /// Called by DiceManager when dice settle - processes the roll result
    /// </summary>
    public void OnRollCompleted(int total, List<int> diceValues = null)
    {
        if (!rollInProgress)
        {
            Debug.LogWarning("OnRollCompleted called but no roll was in progress");
            return;
        }

        rollInProgress = false;

        // Process all bets
        ProcessAllBets(total, diceValues);

        // Process pass line bet
        if (isComeoutRoll)
        {
            ProcessComeoutRoll(total);
        }
        else
        {
            ProcessPointRoll(total);
        }

        UpdateUI();
    }

    /// <summary>
    /// Processes a comeout roll (when point is off)
    /// </summary>
    private void ProcessComeoutRoll(int total)
    {
        Debug.Log($"Comeout Roll: {total}");

        if (total == 7 || total == 11)
        {
            // Natural - win immediately (2:1 payout - bet + profit)
            int payout = CalculatePayout(passLineBet, isPointWin: false);
            HandleWin($"Natural! {total}", payout, isPassLine: true);
        }
        else if (total == 2 || total == 3 || total == 12)
        {
            // Craps - lose immediately
            HandleLoss($"Craps! {total}", isPassLine: true);
        }
        else
        {
            // Point is established (4, 5, 6, 8, 9, or 10)
            pointIsOn = true;
            pointValue = total;
            isComeoutRoll = false;
            canRoll = true; // Ready for next roll
            Debug.Log($"Point established: {pointValue}");
        }
    }

    /// <summary>
    /// Processes a point roll (when point is on)
    /// </summary>
    private void ProcessPointRoll(int total)
    {
        Debug.Log($"Point Roll: {total} (Point: {pointValue})");

        if (total == pointValue)
        {
            // Made the point - win (2:1 payout - bet + profit)
            int payout = CalculatePayout(passLineBet, isPointWin: true);
            HandleWin($"Made the point! {total}", payout, isPassLine: true);
        }
        else if (total == 7)
        {
            // Seven out - lose pass line bet and all place bets
            HandleLoss($"Seven out! {total}", isPassLine: true);
            LoseAllPlaceBets();
        }
        else
        {
            // Continue rolling (any other number)
            Debug.Log($"Roll again. Rolled: {total}");
            
            // Show side bet results if any (even if pass line continues)
            if (currentSideBetWins.Count > 0 || currentSideBetLosses.Count > 0)
            {
                ShowSideBetResults(total);
            }
            
            canRoll = true; // Ready for next roll
        }
    }

    // Store side bet results for display
    private List<string> currentSideBetWins = new List<string>();
    private List<string> currentSideBetLosses = new List<string>();
    private int currentSideBetPayout = 0;

    /// <summary>
    /// Processes all active bets (place bets, field, one-roll bets)
    /// </summary>
    private void ProcessAllBets(int total, List<int> diceValues)
    {
        if (diceValues == null || diceValues.Count < 2)
        {
            Debug.LogWarning("Dice values not provided, cannot process hard way bets");
            diceValues = new List<int>(); // Use empty list as fallback
        }

        // Clear previous results
        currentSideBetWins.Clear();
        currentSideBetLosses.Clear();
        currentSideBetPayout = 0;

        bool isHardWay = diceValues.Count >= 2 && diceValues[0] == diceValues[1];

        // Process each active bet
        List<ActiveBet> betsToRemove = new List<ActiveBet>();

        foreach (ActiveBet bet in activeBets)
        {
            int payout = ProcessBet(bet, total, isHardWay, diceValues);
            
            if (payout > 0)
            {
                // Bet won
                currentSideBetPayout += payout;
                currentSideBetWins.Add($"{bet.betType}: ${payout}");
                
                // Remove one-roll bets after processing
                if (IsOneRollBet(bet.betType))
                {
                    betsToRemove.Add(bet);
                }
            }
            else if (payout < 0)
            {
                // Bet lost
                currentSideBetLosses.Add($"{bet.betType}: Lost ${bet.amount}");
                
                // Remove lost bets
                if (IsOneRollBet(bet.betType) || (IsPlaceBet(bet.betType) && total == 7))
                {
                    betsToRemove.Add(bet);
                }
            }
        }

        // Remove processed bets
        foreach (var bet in betsToRemove)
        {
            activeBets.Remove(bet);
        }

        // Add payouts to account
        if (currentSideBetPayout > 0 && AccountManager.Instance != null)
        {
            AccountManager.Instance.AddMoney(currentSideBetPayout);
            Debug.Log($"Total side bet payout: ${currentSideBetPayout}");
        }

        // Log results
        if (currentSideBetWins.Count > 0)
        {
            Debug.Log($"Side bets won: {string.Join(", ", currentSideBetWins)}");
        }
        if (currentSideBetLosses.Count > 0)
        {
            Debug.Log($"Side bets lost: {string.Join(", ", currentSideBetLosses)}");
        }
    }

    /// <summary>
    /// Processes a single bet and returns payout (positive = win, negative = loss, 0 = no change)
    /// </summary>
    private int ProcessBet(ActiveBet bet, int total, bool isHardWay, List<int> diceValues)
    {
        switch (bet.betType)
        {
            // Place bets - win if number rolled before 7
            case BetType.Place4:
                if (total == 4) return CalculatePlaceBetPayout(bet.amount, 4);
                if (total == 7) return -bet.amount; // Lost
                return 0; // No change, bet continues

            case BetType.Place5:
                if (total == 5) return CalculatePlaceBetPayout(bet.amount, 5);
                if (total == 7) return -bet.amount;
                return 0;

            case BetType.Place6:
                if (total == 6) return CalculatePlaceBetPayout(bet.amount, 6);
                if (total == 7) return -bet.amount;
                return 0;

            case BetType.Place8:
                if (total == 8) return CalculatePlaceBetPayout(bet.amount, 8);
                if (total == 7) return -bet.amount;
                return 0;

            case BetType.Place9:
                if (total == 9) return CalculatePlaceBetPayout(bet.amount, 9);
                if (total == 7) return -bet.amount;
                return 0;

            case BetType.Place10:
                if (total == 10) return CalculatePlaceBetPayout(bet.amount, 10);
                if (total == 7) return -bet.amount;
                return 0;

            // Field bet - one roll, wins on 2,3,4,9,10,11,12
            case BetType.Field:
                if (total == 2 || total == 12) return CalculateFieldBetPayout(bet.amount, total); // 2:1 for 2 or 12
                if (total == 3 || total == 4 || total == 9 || total == 10 || total == 11) return bet.amount; // 1:1 for others
                return -bet.amount; // Lost on 5,6,7,8

            // Any craps - one roll, wins on 2, 3, or 12
            case BetType.AnyCraps:
                if (total == 2 || total == 3 || total == 12) return CalculateAnyCrapsPayout(bet.amount);
                return -bet.amount; // Lost

            // Hard ways - one roll, must be exact pair (both dice same)
            case BetType.Hard4:
                if (total == 4 && isHardWay && diceValues.Count >= 2 && diceValues[0] == 2 && diceValues[1] == 2) 
                    return CalculateHardWayPayout(bet.amount, 4);
                if (total == 4 || total == 7) return -bet.amount; // Lost if easy way (1+3 or 3+1) or 7
                return 0; // No change if other number

            case BetType.Hard6:
                if (total == 6 && isHardWay && diceValues.Count >= 2 && diceValues[0] == 3 && diceValues[1] == 3) 
                    return CalculateHardWayPayout(bet.amount, 6);
                if (total == 6 || total == 7) return -bet.amount; // Lost if easy way (1+5, 2+4, 4+2, 5+1) or 7
                return 0;

            case BetType.Hard8:
                if (total == 8 && isHardWay && diceValues.Count >= 2 && diceValues[0] == 4 && diceValues[1] == 4) 
                    return CalculateHardWayPayout(bet.amount, 8);
                if (total == 8 || total == 7) return -bet.amount; // Lost if easy way (2+6, 3+5, 5+3, 6+2) or 7
                return 0;

            case BetType.Hard10:
                if (total == 10 && isHardWay && diceValues.Count >= 2 && diceValues[0] == 5 && diceValues[1] == 5) 
                    return CalculateHardWayPayout(bet.amount, 10);
                if (total == 10 || total == 7) return -bet.amount; // Lost if easy way (4+6, 6+4) or 7
                return 0;

            default:
                return 0;
        }
    }

    /// <summary>
    /// Checks if bet is a one-roll bet
    /// </summary>
    private bool IsOneRollBet(BetType betType)
    {
        return betType == BetType.Field || betType == BetType.AnyCraps ||
               betType == BetType.Hard4 || betType == BetType.Hard6 ||
               betType == BetType.Hard8 || betType == BetType.Hard10;
    }

    /// <summary>
    /// Loses all place bets (called on seven out)
    /// </summary>
    private void LoseAllPlaceBets()
    {
        foreach (var bet in activeBets.Where(b => IsPlaceBet(b.betType)).ToList())
        {
            if (AccountManager.Instance != null)
            {
                AccountManager.Instance.RecordLoss(bet.amount);
            }
            // Add to loss messages for display
            currentSideBetLosses.Add($"{bet.betType}: Lost ${bet.amount}");
        }
        activeBets.RemoveAll(b => IsPlaceBet(b.betType));
    }

    /// <summary>
    /// Calculates payout based on bet amount and game rules.
    /// Returns total amount to add to account (bet + profit).
    /// Since the bet is removed when placed, this pays back the bet plus profit.
    /// </summary>
    /// <param name="bet">The bet amount that was already removed from account</param>
    /// <param name="isPointWin">True if this is a point win, false if comeout win (both use 2:1)</param>
    private int CalculatePayout(int bet, bool isPointWin)
    {
        float multiplier = isPointWin ? pointPayoutMultiplier : comeoutPayoutMultiplier;
        // Multiplier of 2.0 means: bet $10 -> get $20 back (your $10 bet + $10 profit)
        return Mathf.RoundToInt(bet * multiplier);
    }

    /// <summary>
    /// Calculates place bet payout with correct odds
    /// Returns total amount to add to account (bet + profit).
    /// Since the bet is removed when placed, this pays back the bet plus profit.
    /// 
    /// Standard craps place bet payouts:
    /// 4 & 10: 9:5 odds - bet $5, win $9 profit, total = $14 (multiplier = 14/5 = 2.8)
    /// 5 & 9: 7:5 odds - bet $5, win $7 profit, total = $12 (multiplier = 12/5 = 2.4)
    /// 6 & 8: 7:6 odds - bet $6, win $7 profit, total = $13 (multiplier = 13/6 = 2.167)
    /// 
    /// Formula: multiplier = (bet + profit) / bet = (odds numerator + odds denominator) / odds denominator
    /// </summary>
    private int CalculatePlaceBetPayout(int bet, int number)
    {
        float multiplier;
        switch (number)
        {
            case 4:
            case 10:
                // 9:5 odds: bet $5, win $9 profit, total = $14
                // Multiplier = (5 + 9) / 5 = 14/5 = 2.8
                // For any bet: total = bet * 2.8
                multiplier = 14f / 5f; // 9:5 odds = 2.8 (correct)
                break;
            case 5:
            case 9:
                // 7:5 odds: bet $5, win $7 profit, total = $12
                // Multiplier = (5 + 7) / 5 = 12/5 = 2.4
                // For any bet: total = bet * 2.4
                multiplier = 12f / 5f; // 7:5 odds = 2.4 (correct)
                break;
            case 6:
            case 8:
                // 7:6 odds: bet $6, win $7 profit, total = $13
                // Multiplier = (6 + 7) / 6 = 13/6 = 2.1667
                // For any bet: total = bet * 2.1667
                multiplier = 13f / 6f; // 7:6 odds = 2.1667 (correct)
                break;
            default:
                multiplier = 2.0f; // Fallback
                break;
        }
        return Mathf.RoundToInt(bet * multiplier);
    }

    /// <summary>
    /// Calculates field bet payout
    /// 2 or 12: 2:1 - bet $10, win $20 total
    /// 3, 4, 9, 10, 11: 1:1 - bet $10, win $10 total (just bet back)
    /// </summary>
    private int CalculateFieldBetPayout(int bet, int total)
    {
        if (total == 2 || total == 12)
        {
            return bet * 2; // 2:1 payout
        }
        return bet; // 1:1 payout (just bet back)
    }

    /// <summary>
    /// Calculates any craps payout (7:1 odds)
    /// Bet $10, win $70 total ($10 bet + $60 profit)
    /// </summary>
    private int CalculateAnyCrapsPayout(int bet)
    {
        return bet * 8; // 7:1 payout + bet back = 8x total
    }

    /// <summary>
    /// Calculates hard way payout
    /// Hard 4 & 10: 7:1 - bet $10, win $70 total
    /// Hard 6 & 8: 9:1 - bet $10, win $90 total
    /// </summary>
    private int CalculateHardWayPayout(int bet, int number)
    {
        if (number == 4 || number == 10)
        {
            return bet * 8; // 7:1 payout + bet back
        }
        else if (number == 6 || number == 8)
        {
            return bet * 10; // 9:1 payout + bet back
        }
        return bet * 2; // Fallback
    }

    /// <summary>
    /// Handles a win outcome
    /// </summary>
    private void HandleWin(string message, int payout, bool isPassLine = false)
    {
        Debug.Log($"{message} - You Win! Payout: ${payout}");
        
        // Update account
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.RecordWin(payout);
        }

        // Build comprehensive win message
        string winMessage = message + "\nYou Win!";
        string detailMessage = "";
        int totalPayout = payout;

        // Add pass line bet info
        if (isPassLine && payout > 0)
        {
            detailMessage = $"Pass Line: ${payout}";
            totalPayout = payout;
        }

        // Add side bet wins
        if (currentSideBetWins.Count > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += "\n";
            }
            detailMessage += string.Join("\n", currentSideBetWins);
            totalPayout += currentSideBetPayout;
        }

        // Add side bet losses (if any)
        if (currentSideBetLosses.Count > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += "\n";
            }
            detailMessage += string.Join("\n", currentSideBetLosses);
        }

        // Show total payout if there are multiple bets
        if (isPassLine && currentSideBetWins.Count > 0)
        {
            detailMessage += $"\n\nTotal Payout: ${totalPayout}";
        }
        else if (currentSideBetWins.Count > 0 && !isPassLine)
        {
            detailMessage += $"\n\nTotal Payout: ${currentSideBetPayout}";
        }
        else if (string.IsNullOrEmpty(detailMessage))
        {
            detailMessage = $"Payout: ${payout}";
        }

        // Show win UI
        ShowResult(winMessage, detailMessage, Color.green);
        
        // Play win particles
        if (winParticles != null)
        {
            winParticles.Play();
        }
        
        // Play win sound
        AudioPlayer audioPlayer = FindAnyObjectByType<AudioPlayer>();
        if (audioPlayer != null)
        {
            audioPlayer.PlayWinSound();
        }

        // Reset for next round after delay (only if pass line bet)
        if (isPassLine)
        {
            StartCoroutine(ResetAfterDelay(3f));
        }
    }

    /// <summary>
    /// Handles a loss outcome
    /// </summary>
    private void HandleLoss(string message, bool isPassLine = false)
    {
        int lostAmount = isPassLine ? passLineBet : 0;
        Debug.Log($"{message} - You Lose! Lost: ${lostAmount}");
        
        // Update account
        if (AccountManager.Instance != null && lostAmount > 0)
        {
            AccountManager.Instance.RecordLoss(lostAmount);
        }

        // Build comprehensive loss message
        string loseMessage = message + "\nYou Lose!";
        string detailMessage = "";
        int totalLost = lostAmount;

        // Add pass line bet loss
        if (isPassLine && lostAmount > 0)
        {
            detailMessage = $"Pass Line: Lost ${lostAmount}";
        }

        // Add side bet losses
        if (currentSideBetLosses.Count > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += "\n";
            }
            detailMessage += string.Join("\n", currentSideBetLosses);
            
            // Calculate total lost from side bets
            foreach (var lossMsg in currentSideBetLosses)
            {
                // Extract amount from "BetType: Lost $X"
                if (lossMsg.Contains("Lost $"))
                {
                    string amountStr = lossMsg.Substring(lossMsg.IndexOf("Lost $") + 6);
                    if (int.TryParse(amountStr, out int amount))
                    {
                        totalLost += amount;
                    }
                }
            }
        }

        // Add side bet wins (if any - some bets might win while pass line loses)
        if (currentSideBetWins.Count > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += "\n";
            }
            detailMessage += "Won:\n" + string.Join("\n", currentSideBetWins);
        }

        // Show total lost if there are multiple bets
        if (totalLost > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += $"\n\nTotal Lost: ${totalLost}";
            }
            else
            {
                detailMessage = $"Lost: ${totalLost}";
            }
        }
        else if (string.IsNullOrEmpty(detailMessage))
        {
            detailMessage = "No bets lost";
        }

        // Show lose UI
        ShowResult(loseMessage, detailMessage, Color.red);
        
        // Play lose particles
        if (loseParticles != null)
        {
            loseParticles.Play();
        }
        
        // Play lose sound
        AudioPlayer audioPlayer = FindAnyObjectByType<AudioPlayer>();
        if (audioPlayer != null)
        {
            audioPlayer.PlayLoseSound();
        }

        // Reset for next round after delay (only if pass line bet)
        if (isPassLine)
        {
            StartCoroutine(ResetAfterDelay(3f));
        }
    }

    /// <summary>
    /// Shows side bet results when pass line bet continues
    /// </summary>
    private void ShowSideBetResults(int total)
    {
        string mainMessage = $"Rolled: {total}";
        string detailMessage = "";
        
        if (currentSideBetWins.Count > 0)
        {
            detailMessage = "Won:\n" + string.Join("\n", currentSideBetWins);
            if (currentSideBetPayout > 0)
            {
                detailMessage += $"\n\nTotal Payout: ${currentSideBetPayout}";
            }
        }
        
        if (currentSideBetLosses.Count > 0)
        {
            if (!string.IsNullOrEmpty(detailMessage))
            {
                detailMessage += "\n\n";
            }
            detailMessage += "Lost:\n" + string.Join("\n", currentSideBetLosses);
        }
        
        if (!string.IsNullOrEmpty(detailMessage))
        {
            Color messageColor = currentSideBetWins.Count > 0 ? Color.green : Color.yellow;
            ShowResult(mainMessage, detailMessage, messageColor);
            
            // Auto-hide after 2 seconds
            StartCoroutine(HideResultAfterDelay(2f));
        }
    }

    /// <summary>
    /// Hides result panel after a delay
    /// </summary>
    private IEnumerator HideResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideResult();
    }

    /// <summary>
    /// Shows result panel with message
    /// </summary>
    private void ShowResult(string mainMessage, string detailMessage, Color color)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = $"{mainMessage}\n{detailMessage}";
            resultText.color = color;
        }
    }

    /// <summary>
    /// Hides result panel
    /// </summary>
    private void HideResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    /// <summary>
    /// Resets game state after a delay
    /// </summary>
    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetGameState();
    }

    /// <summary>
    /// Updates UI elements if they are assigned
    /// </summary>
    private void UpdateUI()
    {
        if (gameStateText != null)
        {
            if (rollInProgress)
            {
                gameStateText.text = "Rolling...";
            }
            else if (isComeoutRoll)
            {
                gameStateText.text = "Comeout Roll";
            }
            else
            {
                gameStateText.text = "Point Roll";
            }
        }

        if (pointText != null)
        {
            if (pointIsOn)
            {
                pointText.text = $"Point: {pointValue}";
            }
            else
            {
                pointText.text = "Point: Off";
            }
        }
    }

    /// <summary>
    /// Handles when player runs out of money
    /// </summary>
    private void HandleOutOfMoney()
    {
        Debug.Log("Player ran out of money! Giving loan and returning to menu...");
        
        // Give loan
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.GiveLoan();
        }

        // Show message
        ShowResult("Out of Money!", $"Casino loaned you $500\nTotal loaned: ${AccountManager.Instance?.GetTotalLoaned() ?? 0}", Color.yellow);

        // Return to menu after delay
        StartCoroutine(ReturnToMenuAfterDelay(3f));
    }

    /// <summary>
    /// Returns to menu scene after a delay
    /// </summary>
    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }

    // Public getters for UI or other systems
    public bool IsPointOn => pointIsOn;
    public int PointValue => pointValue;
    public bool IsComeoutRoll => isComeoutRoll;
    public bool IsRollInProgress => rollInProgress;
    public List<ActiveBet> GetActiveBets() => new List<ActiveBet>(activeBets);
    public int GetPassLineBet() => passLineBet;
}

