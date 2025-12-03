using UnityEngine;
using System;

/// <summary>
/// Manages player account data including money, statistics, and progress.
/// Uses PlayerPrefs for persistence across sessions.
/// </summary>
public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance;

    [Header("Starting Values")]
    [SerializeField] private int startingMoney = 1000;
    
    [Header("Loan Settings")]
    [SerializeField] private int loanAmount = 500; // Amount given to player when requesting loan
    [SerializeField] private int loanDebt = 525; // Amount added to debt (includes 5% interest)
    [SerializeField] private int loanThreshold = 100; // Must be below this to request loan

    // Account data
    private int currentMoney;
    private int totalWins;
    private int totalLosses;
    private int currentWinStreak;
    private int bestWinStreak;
    private int totalGamesPlayed;
    private int totalMoneyWon;
    private int totalMoneyLost;
    private int totalLoaned; // Total amount loaned to player

    // Events for UI updates
    public event Action<int> OnMoneyChanged;
    public event Action<int, int> OnStatsChanged; // wins, losses
    public event Action<int> OnWinStreakChanged;
    public event Action<int> OnLoanChanged; // Fired when loan amount changes
    public event Action OnOutOfMoney; // Fired when money reaches 0

    // PlayerPrefs keys
    private const string MONEY_KEY = "PlayerMoney";
    private const string WINS_KEY = "PlayerWins";
    private const string LOSSES_KEY = "PlayerLosses";
    private const string WIN_STREAK_KEY = "CurrentWinStreak";
    private const string BEST_STREAK_KEY = "BestWinStreak";
    private const string GAMES_PLAYED_KEY = "TotalGamesPlayed";
    private const string MONEY_WON_KEY = "TotalMoneyWon";
    private const string MONEY_LOST_KEY = "TotalMoneyLost";
    private const string TOTAL_LOANED_KEY = "TotalLoaned";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAccountData();
    }

    /// <summary>
    /// Loads account data from PlayerPrefs or initializes with default values
    /// </summary>
    private void LoadAccountData()
    {
        currentMoney = PlayerPrefs.GetInt(MONEY_KEY, startingMoney);
        totalWins = PlayerPrefs.GetInt(WINS_KEY, 0);
        totalLosses = PlayerPrefs.GetInt(LOSSES_KEY, 0);
        currentWinStreak = PlayerPrefs.GetInt(WIN_STREAK_KEY, 0);
        bestWinStreak = PlayerPrefs.GetInt(BEST_STREAK_KEY, 0);
        totalGamesPlayed = PlayerPrefs.GetInt(GAMES_PLAYED_KEY, 0);
        totalMoneyWon = PlayerPrefs.GetInt(MONEY_WON_KEY, 0);
        totalMoneyLost = PlayerPrefs.GetInt(MONEY_LOST_KEY, 0);
        totalLoaned = PlayerPrefs.GetInt(TOTAL_LOANED_KEY, 0);
    }

    /// <summary>
    /// Saves account data to PlayerPrefs
    /// </summary>
    private void SaveAccountData()
    {
        PlayerPrefs.SetInt(MONEY_KEY, currentMoney);
        PlayerPrefs.SetInt(WINS_KEY, totalWins);
        PlayerPrefs.SetInt(LOSSES_KEY, totalLosses);
        PlayerPrefs.SetInt(WIN_STREAK_KEY, currentWinStreak);
        PlayerPrefs.SetInt(BEST_STREAK_KEY, bestWinStreak);
        PlayerPrefs.SetInt(GAMES_PLAYED_KEY, totalGamesPlayed);
        PlayerPrefs.SetInt(MONEY_WON_KEY, totalMoneyWon);
        PlayerPrefs.SetInt(MONEY_LOST_KEY, totalMoneyLost);
        PlayerPrefs.SetInt(TOTAL_LOANED_KEY, totalLoaned);
        PlayerPrefs.Save();

        // Sync to cloud if AccountService is available
        SyncToCloud();
    }

    /// <summary>
    /// Loads account data from AccountData (cloud account)
    /// </summary>
    public void LoadAccountData(AccountData accountData)
    {
        if (accountData == null) return;

        currentMoney = accountData.currentMoney;
        totalWins = accountData.totalWins;
        totalLosses = accountData.totalLosses;
        currentWinStreak = accountData.currentWinStreak;
        bestWinStreak = accountData.bestWinStreak;
        totalGamesPlayed = accountData.totalGamesPlayed;
        totalMoneyWon = accountData.totalMoneyWon;
        totalMoneyLost = accountData.totalMoneyLost;
        totalLoaned = accountData.totalLoaned;

        // Also save to PlayerPrefs as backup
        SaveAccountData();

        // Notify UI
        OnMoneyChanged?.Invoke(currentMoney);
        OnStatsChanged?.Invoke(totalWins, totalLosses);
        OnWinStreakChanged?.Invoke(currentWinStreak);
    }

    /// <summary>
    /// Syncs current account data to cloud via AccountService
    /// </summary>
    private void SyncToCloud()
    {
        if (AccountService.Instance != null && AccountService.Instance.IsAuthenticated())
        {
            AccountData account = AccountService.Instance.GetCurrentAccount();
            if (account != null)
            {
                // Update account data with current values
                account.currentMoney = currentMoney;
                account.totalWins = totalWins;
                account.totalLosses = totalLosses;
                account.currentWinStreak = currentWinStreak;
                account.bestWinStreak = bestWinStreak;
                account.totalGamesPlayed = totalGamesPlayed;
                account.totalMoneyWon = totalMoneyWon;
                account.totalMoneyLost = totalMoneyLost;
                account.totalLoaned = totalLoaned;

                // Sync to cloud (async, don't wait)
                AccountService.Instance.SyncAccount(account);
            }
        }
    }

    #region Money Management

    /// <summary>
    /// Gets the current money amount
    /// </summary>
    public int GetMoney() => currentMoney;

    /// <summary>
    /// Adds money to the account
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot add negative money. Use RemoveMoney instead.");
            return;
        }
        currentMoney += amount;
        totalMoneyWon += amount;
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
    }

    /// <summary>
    /// Removes money from the account
    /// </summary>
    public bool RemoveMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot remove negative money. Use AddMoney instead.");
            return false;
        }
        if (currentMoney < amount)
        {
            Debug.LogWarning($"Insufficient funds. Have: {currentMoney}, Need: {amount}");
            return false;
        }
        currentMoney -= amount;
        totalMoneyLost += amount;
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
        
        // Check if player ran out of money
        if (currentMoney <= 0)
        {
            OnOutOfMoney?.Invoke();
        }
        
        return true;
    }

    /// <summary>
    /// Checks if player has enough money
    /// </summary>
    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }

    /// <summary>
    /// Resets money to starting amount (for testing or new game)
    /// </summary>
    public void ResetMoney()
    {
        currentMoney = startingMoney;
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
    }

    #endregion

    #region Statistics Management

    /// <summary>
    /// Records a win and updates statistics
    /// </summary>
    public void RecordWin(int moneyWon = 0)
    {
        totalWins++;
        totalGamesPlayed++;
        currentWinStreak++;
        
        if (currentWinStreak > bestWinStreak)
        {
            bestWinStreak = currentWinStreak;
        }

        if (moneyWon > 0)
        {
            AddMoney(moneyWon);
        }

        SaveAccountData();
        OnStatsChanged?.Invoke(totalWins, totalLosses);
        OnWinStreakChanged?.Invoke(currentWinStreak);
    }

    /// <summary>
    /// Records a loss and updates statistics
    /// </summary>
    public void RecordLoss(int moneyLost = 0)
    {
        totalLosses++;
        totalGamesPlayed++;
        currentWinStreak = 0; // Reset win streak on loss

        if (moneyLost > 0)
        {
            RemoveMoney(moneyLost);
        }

        SaveAccountData();
        OnStatsChanged?.Invoke(totalWins, totalLosses);
        OnWinStreakChanged?.Invoke(currentWinStreak);
    }

    /// <summary>
    /// Gets win rate as a percentage
    /// </summary>
    public float GetWinRate()
    {
        if (totalGamesPlayed == 0) return 0f;
        return (float)totalWins / totalGamesPlayed * 100f;
    }

    #endregion

    #region Getters

    public int GetTotalWins() => totalWins;
    public int GetTotalLosses() => totalLosses;
    public int GetCurrentWinStreak() => currentWinStreak;
    public int GetBestWinStreak() => bestWinStreak;
    public int GetTotalGamesPlayed() => totalGamesPlayed;
    public int GetTotalMoneyWon() => totalMoneyWon;
    public int GetTotalMoneyLost() => totalMoneyLost;
    public int GetTotalLoaned() => totalLoaned;

    #endregion

    #region Loan Management

    /// <summary>
    /// Gives the player a loan when they run out of money (auto-loan during game)
    /// </summary>
    public void GiveLoan()
    {
        currentMoney += loanAmount;
        totalLoaned += loanDebt; // Add debt with interest
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
        OnLoanChanged?.Invoke(totalLoaned);
        Debug.Log($"Casino loaned ${loanAmount}. Debt added: ${loanDebt}. Total debt: ${totalLoaned}");
    }

    /// <summary>
    /// Requests a loan manually (only works if money is below threshold)
    /// Player receives $500, but $525 is added to their debt (5% interest)
    /// </summary>
    public bool RequestLoan()
    {
        if (currentMoney >= loanThreshold)
        {
            Debug.LogWarning($"Cannot request loan. You have ${currentMoney}, need less than ${loanThreshold}.");
            return false;
        }

        currentMoney += loanAmount; // Give player $500
        totalLoaned += loanDebt; // Add $525 to debt (5% interest)
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
        OnLoanChanged?.Invoke(totalLoaned);
        Debug.Log($"Loan requested. Received: ${loanAmount}. Debt added: ${loanDebt}. Total debt: ${totalLoaned}");
        return true;
    }

    /// <summary>
    /// Repays a portion of the loan
    /// </summary>
    /// <param name="amount">Amount to repay</param>
    /// <returns>True if repayment was successful</returns>
    public bool RepayLoan(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Repayment amount must be positive.");
            return false;
        }

        if (totalLoaned <= 0)
        {
            Debug.LogWarning("No loan to repay.");
            return false;
        }

        if (currentMoney < amount)
        {
            Debug.LogWarning($"Insufficient funds. Have: ${currentMoney}, trying to repay: ${amount}");
            return false;
        }

        // Cap repayment at total loan amount
        int actualRepayment = Mathf.Min(amount, totalLoaned);
        
        currentMoney -= actualRepayment;
        totalLoaned -= actualRepayment;
        
        SaveAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
        OnLoanChanged?.Invoke(totalLoaned);
        Debug.Log($"Loan repaid: ${actualRepayment}. Remaining debt: ${totalLoaned}");
        return true;
    }

    /// <summary>
    /// Checks if player can request a loan (money below threshold)
    /// </summary>
    public bool CanRequestLoan()
    {
        return currentMoney < loanThreshold;
    }

    /// <summary>
    /// Checks if player has any outstanding loan
    /// </summary>
    public bool HasLoan()
    {
        return totalLoaned > 0;
    }

    /// <summary>
    /// Checks if player is out of money
    /// </summary>
    public bool IsOutOfMoney()
    {
        return currentMoney <= 0;
    }

    /// <summary>
    /// Gets the loan threshold (minimum money to request loan)
    /// </summary>
    public int GetLoanThreshold()
    {
        return loanThreshold;
    }

    #endregion

    #region Reset Functions

    /// <summary>
    /// Resets all account data (use with caution!)
    /// </summary>
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        LoadAccountData();
        OnMoneyChanged?.Invoke(currentMoney);
        OnStatsChanged?.Invoke(totalWins, totalLosses);
        OnWinStreakChanged?.Invoke(currentWinStreak);
    }

    /// <summary>
    /// Resets statistics but keeps money
    /// </summary>
    public void ResetStatistics()
    {
        totalWins = 0;
        totalLosses = 0;
        currentWinStreak = 0;
        bestWinStreak = 0;
        totalGamesPlayed = 0;
        totalMoneyWon = 0;
        totalMoneyLost = 0;
        SaveAccountData();
        OnStatsChanged?.Invoke(totalWins, totalLosses);
        OnWinStreakChanged?.Invoke(currentWinStreak);
    }

    #endregion
}

