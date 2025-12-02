using UnityEngine;
using TMPro;

/// <summary>
/// UI component that displays account information (money, stats, etc.)
/// Updates automatically when account data changes.
/// </summary>
public class AccountUI : MonoBehaviour
{
    [Header("Money Display")]
    public TMP_Text moneyText;
    
    [Header("Statistics Display")]
    public TMP_Text winsText;
    public TMP_Text lossesText;
    public TMP_Text winRateText;
    public TMP_Text winStreakText;
    public TMP_Text bestStreakText;
    public TMP_Text gamesPlayedText;
    public TMP_Text totalLoanedText; // Optional: display total loaned amount

    private void Start()
    {
        // Subscribe to account events
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            AccountManager.Instance.OnStatsChanged += UpdateStatsDisplay;
            AccountManager.Instance.OnWinStreakChanged += UpdateStreakDisplay;
            
            // Initial update
            UpdateAllDisplays();
        }
        else
        {
            Debug.LogWarning("AccountManager not found. AccountUI will not update.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            AccountManager.Instance.OnStatsChanged -= UpdateStatsDisplay;
            AccountManager.Instance.OnWinStreakChanged -= UpdateStreakDisplay;
        }
    }

    /// <summary>
    /// Updates all UI displays at once
    /// </summary>
    private void UpdateAllDisplays()
    {
        UpdateMoneyDisplay(AccountManager.Instance.GetMoney());
        UpdateStatsDisplay(AccountManager.Instance.GetTotalWins(), AccountManager.Instance.GetTotalLosses());
        UpdateStreakDisplay(AccountManager.Instance.GetCurrentWinStreak());
        
        // Update total loaned display
        if (totalLoanedText != null)
        {
            int totalLoaned = AccountManager.Instance.GetTotalLoaned();
            totalLoanedText.text = $"Total Loaned: ${totalLoaned:N0}";
        }
    }

    /// <summary>
    /// Updates money display
    /// </summary>
    private void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${money:N0}";
        }
    }

    /// <summary>
    /// Updates statistics display
    /// </summary>
    private void UpdateStatsDisplay(int wins, int losses)
    {
        if (winsText != null)
        {
            winsText.text = $"Wins: {wins}";
        }

        if (lossesText != null)
        {
            lossesText.text = $"Losses: {losses}";
        }

        if (winRateText != null)
        {
            float winRate = AccountManager.Instance.GetWinRate();
            winRateText.text = $"Win Rate: {winRate:F1}%";
        }

        if (gamesPlayedText != null)
        {
            int gamesPlayed = AccountManager.Instance.GetTotalGamesPlayed();
            gamesPlayedText.text = $"Games: {gamesPlayed}";
        }

        if (totalLoanedText != null)
        {
            int totalLoaned = AccountManager.Instance.GetTotalLoaned();
            totalLoanedText.text = $"Total Loaned: ${totalLoaned:N0}";
        }
    }

    /// <summary>
    /// Updates win streak display
    /// </summary>
    private void UpdateStreakDisplay(int currentStreak)
    {
        if (winStreakText != null)
        {
            winStreakText.text = $"Win Streak: {currentStreak}";
        }

        if (bestStreakText != null)
        {
            int bestStreak = AccountManager.Instance.GetBestWinStreak();
            bestStreakText.text = $"Best Streak: {bestStreak}";
        }
    }
}

