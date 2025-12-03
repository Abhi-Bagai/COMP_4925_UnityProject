using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI controller for the home screen
/// Displays account information and provides navigation
/// </summary>
public class HomeScreen : MonoBehaviour
{
    [Header("Account Display")]
    public TMP_Text usernameText;
    public TMP_Text moneyText;
    public TMP_Text winsText;
    public TMP_Text lossesText;
    public TMP_Text winRateText;
    public TMP_Text winStreakText;
    public TMP_Text totalLoanedText;

    [Header("Navigation Buttons")]
    public Button playGameButton;
    public Button signOutButton;
    public Button refreshButton;

    [Header("Loan UI")]
    public Button requestLoanButton;
    public Button repayLoanButton;
    public TMP_InputField repayAmountInput;
    public GameObject repayLoanPanel; // Panel containing repay input and confirm button
    public Button confirmRepayButton;
    public Button cancelRepayButton;
    public TMP_Text loanStatusText; // Shows loan eligibility or status messages

    [Header("Scenes")]
    public string gameSceneName = "TableGame";
    public string loginSceneName = "Login";

    private void Start()
    {
        // Setup button listeners
        if (playGameButton != null)
        {
            playGameButton.onClick.AddListener(OnPlayGameClicked);
        }

        if (signOutButton != null)
        {
            signOutButton.onClick.AddListener(OnSignOutClicked);
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(OnRefreshClicked);
        }

        // Loan buttons
        if (requestLoanButton != null)
        {
            requestLoanButton.onClick.AddListener(OnRequestLoanClicked);
        }

        if (repayLoanButton != null)
        {
            repayLoanButton.onClick.AddListener(OnRepayLoanClicked);
        }

        if (confirmRepayButton != null)
        {
            confirmRepayButton.onClick.AddListener(OnConfirmRepayClicked);
        }

        if (cancelRepayButton != null)
        {
            cancelRepayButton.onClick.AddListener(OnCancelRepayClicked);
        }

        // Hide repay panel initially
        if (repayLoanPanel != null)
        {
            repayLoanPanel.SetActive(false);
        }

        // Check if user is authenticated
        if (AccountService.Instance == null || !AccountService.Instance.IsAuthenticated())
        {
            // Not signed in, go to login screen
            Debug.LogWarning("Not authenticated, redirecting to login...");
            // You might want to create a Login scene, or handle this differently
            return;
        }

        // Subscribe to account updates
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            AccountManager.Instance.OnStatsChanged += UpdateStatsDisplay;
            AccountManager.Instance.OnWinStreakChanged += UpdateStreakDisplay;
            AccountManager.Instance.OnLoanChanged += UpdateLoanDisplay;
        }

        // Load account data
        LoadAccountData();
        UpdateLoanButtonStates();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (AccountManager.Instance != null)
        {
            AccountManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            AccountManager.Instance.OnStatsChanged -= UpdateStatsDisplay;
            AccountManager.Instance.OnWinStreakChanged -= UpdateStreakDisplay;
            AccountManager.Instance.OnLoanChanged -= UpdateLoanDisplay;
        }
    }

    /// <summary>
    /// Loads and displays account data
    /// </summary>
    private void LoadAccountData()
    {
        AccountData account = AccountService.Instance?.GetCurrentAccount();
        
        if (account != null)
        {
            // Update username display
            if (usernameText != null)
            {
                usernameText.text = $"Welcome, {account.username}!";
            }

            // Update all displays
            UpdateMoneyDisplay(account.currentMoney);
            UpdateStatsDisplay(account.totalWins, account.totalLosses);
            UpdateStreakDisplay(account.currentWinStreak);

            // Update total loaned
            if (totalLoanedText != null)
            {
                totalLoanedText.text = $"Total Loaned: ${account.totalLoaned:N0}";
            }
        }
        else
        {
            // Fallback to AccountManager if available
            if (AccountManager.Instance != null)
            {
                UpdateMoneyDisplay(AccountManager.Instance.GetMoney());
                UpdateStatsDisplay(AccountManager.Instance.GetTotalWins(), AccountManager.Instance.GetTotalLosses());
                UpdateStreakDisplay(AccountManager.Instance.GetCurrentWinStreak());
                
                if (totalLoanedText != null)
                {
                    totalLoanedText.text = $"Total Loaned: ${AccountManager.Instance.GetTotalLoaned():N0}";
                }
            }
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

        if (winRateText != null && AccountManager.Instance != null)
        {
            float winRate = AccountManager.Instance.GetWinRate();
            winRateText.text = $"Win Rate: {winRate:F1}%";
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
    }

    /// <summary>
    /// Updates loan display
    /// </summary>
    private void UpdateLoanDisplay(int totalLoaned)
    {
        if (totalLoanedText != null)
        {
            totalLoanedText.text = $"Debt to Bank: ${totalLoaned:N0}";
        }
        UpdateLoanButtonStates();
    }

    /// <summary>
    /// Updates loan button states based on current money and loan status
    /// </summary>
    private void UpdateLoanButtonStates()
    {
        if (AccountManager.Instance == null) return;

        bool canRequestLoan = AccountManager.Instance.CanRequestLoan();
        bool hasLoan = AccountManager.Instance.HasLoan();
        int currentMoney = AccountManager.Instance.GetMoney();
        int loanThreshold = AccountManager.Instance.GetLoanThreshold();

        // Request Loan button - only enabled if below $100
        if (requestLoanButton != null)
        {
            requestLoanButton.interactable = canRequestLoan;
        }

        // Repay Loan button - only enabled if has loan and has money
        if (repayLoanButton != null)
        {
            repayLoanButton.interactable = hasLoan && currentMoney > 0;
        }

        // Update status text
        if (loanStatusText != null)
        {
            if (canRequestLoan)
            {
                loanStatusText.text = $"You can request a loan (below ${loanThreshold})";
                loanStatusText.color = Color.yellow;
            }
            else if (hasLoan)
            {
                loanStatusText.text = "You have outstanding debt. Consider repaying!";
                loanStatusText.color = new Color(1f, 0.6f, 0.2f); // Orange
            }
            else
            {
                loanStatusText.text = "No debt. Good standing!";
                loanStatusText.color = Color.green;
            }
        }
    }

    #region Loan Functions

    /// <summary>
    /// Called when Request Loan button is clicked
    /// </summary>
    private void OnRequestLoanClicked()
    {
        Debug.Log("Request Loan button clicked!");
        
        if (AccountManager.Instance == null)
        {
            Debug.LogError("AccountManager.Instance is null!");
            UpdateLoanStatusMessage("Error: Account system not available.", Color.red);
            return;
        }

        int currentMoney = AccountManager.Instance.GetMoney();
        int threshold = AccountManager.Instance.GetLoanThreshold();
        Debug.Log($"Current money: ${currentMoney}, Threshold: ${threshold}, Can request: {currentMoney < threshold}");

        if (AccountManager.Instance.RequestLoan())
        {
            // Loan granted
            Debug.Log("Loan granted!");
            UpdateLoanStatusMessage("Loan granted! You received $500. $525 added to your debt (5% interest).", Color.green);
            LoadAccountData();
            UpdateLoanButtonStates();
        }
        else
        {
            // Loan denied
            Debug.Log($"Loan denied - you have ${currentMoney}, need less than ${threshold}");
            UpdateLoanStatusMessage($"Loan denied. You have ${currentMoney}. Must have less than ${threshold} to request a loan.", Color.red);
        }
    }

    /// <summary>
    /// Called when Repay Loan button is clicked - shows repay panel
    /// </summary>
    private void OnRepayLoanClicked()
    {
        if (repayLoanPanel != null)
        {
            repayLoanPanel.SetActive(true);
        }

        // Set default repay amount to min of current money or total loan
        if (repayAmountInput != null && AccountManager.Instance != null)
        {
            int suggestedAmount = Mathf.Min(AccountManager.Instance.GetMoney(), AccountManager.Instance.GetTotalLoaned());
            repayAmountInput.text = suggestedAmount.ToString();
        }
    }

    /// <summary>
    /// Called when Confirm Repay button is clicked
    /// </summary>
    private void OnConfirmRepayClicked()
    {
        if (AccountManager.Instance == null || repayAmountInput == null) return;

        string inputText = repayAmountInput.text.Trim();
        
        if (string.IsNullOrEmpty(inputText))
        {
            UpdateLoanStatusMessage("Please enter an amount to repay.", Color.red);
            return;
        }

        if (!int.TryParse(inputText, out int repayAmount))
        {
            UpdateLoanStatusMessage("Please enter a valid number.", Color.red);
            return;
        }

        if (repayAmount <= 0)
        {
            UpdateLoanStatusMessage("Amount must be greater than 0.", Color.red);
            return;
        }

        if (AccountManager.Instance.RepayLoan(repayAmount))
        {
            // Repayment successful
            int remaining = AccountManager.Instance.GetTotalLoaned();
            if (remaining > 0)
            {
                UpdateLoanStatusMessage($"Repaid ${repayAmount}. Remaining debt: ${remaining}", Color.green);
            }
            else
            {
                UpdateLoanStatusMessage("Congratulations! Your debt is fully paid off!", Color.green);
            }
            
            // Hide repay panel
            if (repayLoanPanel != null)
            {
                repayLoanPanel.SetActive(false);
            }
            
            LoadAccountData();
        }
        else
        {
            // Repayment failed
            UpdateLoanStatusMessage("Repayment failed. Check your balance.", Color.red);
        }
    }

    /// <summary>
    /// Called when Cancel Repay button is clicked
    /// </summary>
    private void OnCancelRepayClicked()
    {
        if (repayLoanPanel != null)
        {
            repayLoanPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the loan status message
    /// </summary>
    private void UpdateLoanStatusMessage(string message, Color color)
    {
        if (loanStatusText != null)
        {
            loanStatusText.text = message;
            loanStatusText.color = color;
        }
    }

    #endregion

    /// <summary>
    /// Called when Play Game button is clicked
    /// </summary>
    private void OnPlayGameClicked()
    {
        // Sync account before leaving
        SyncAccount();
        
        // Load game scene
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Called when Sign Out button is clicked
    /// </summary>
    private void OnSignOutClicked()
    {
        // Sync account before signing out
        SyncAccount();
        
        // Sign out
        if (AccountService.Instance != null)
        {
            AccountService.Instance.SignOut();
        }

        // Navigate to login screen
        SceneManager.LoadScene(loginSceneName);
    }

    /// <summary>
    /// Called when Refresh button is clicked
    /// </summary>
    private void OnRefreshClicked()
    {
        // Sync account data
        SyncAccount();
        
        // Reload display
        LoadAccountData();
    }

    /// <summary>
    /// Syncs current account data to cloud
    /// </summary>
    private void SyncAccount()
    {
        if (AccountService.Instance != null && AccountManager.Instance != null)
        {
            AccountData account = AccountService.Instance.GetCurrentAccount();
            if (account != null)
            {
                // Update account data from AccountManager
                account.currentMoney = AccountManager.Instance.GetMoney();
                account.totalWins = AccountManager.Instance.GetTotalWins();
                account.totalLosses = AccountManager.Instance.GetTotalLosses();
                account.currentWinStreak = AccountManager.Instance.GetCurrentWinStreak();
                account.bestWinStreak = AccountManager.Instance.GetBestWinStreak();
                account.totalGamesPlayed = AccountManager.Instance.GetTotalGamesPlayed();
                account.totalMoneyWon = AccountManager.Instance.GetTotalMoneyWon();
                account.totalMoneyLost = AccountManager.Instance.GetTotalMoneyLost();
                account.totalLoaned = AccountManager.Instance.GetTotalLoaned();

                // Sync to cloud
                AccountService.Instance.SyncAccount(account);
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Sync when app is paused (mobile)
        if (pauseStatus)
        {
            SyncAccount();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Sync when app loses focus
        if (!hasFocus)
        {
            SyncAccount();
        }
    }
}

