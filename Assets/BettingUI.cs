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

    [Header("Preset Bet Buttons")]
    public Button bet10Button;
    public Button bet25Button;
    public Button bet50Button;
    public Button bet100Button;
    public Button bet250Button;
    public Button bet500Button;
    public Button bet1000Button;

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

    [Header("Loan Button")]
    public Button requestLoanButton; // Button to request a loan
    public TMP_Text loanButtonText; // Optional text on loan button

    private void Start()
    {
        // Connect button events
        if (increaseBetButton != null)
        {
            increaseBetButton.onClick.AddListener(OnIncreaseBet);
            Debug.Log("Increase bet button connected");
        }
        else
        {
            Debug.LogWarning("Increase Bet Button is not assigned in BettingUI!");
        }
        
        if (decreaseBetButton != null)
        {
            decreaseBetButton.onClick.AddListener(OnDecreaseBet);
            Debug.Log("Decrease bet button connected");
        }
        else
        {
            Debug.LogWarning("Decrease Bet Button is not assigned in BettingUI!");
        }

        // Connect preset bet amount buttons
        ConnectPresetBetButton(bet10Button, 10);
        ConnectPresetBetButton(bet25Button, 25);
        ConnectPresetBetButton(bet50Button, 50);
        ConnectPresetBetButton(bet100Button, 100);
        ConnectPresetBetButton(bet250Button, 250);
        ConnectPresetBetButton(bet500Button, 500);
        ConnectPresetBetButton(bet1000Button, 1000);

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

        // Connect loan button
        if (requestLoanButton != null)
        {
            requestLoanButton.onClick.AddListener(OnRequestLoan);
            Debug.Log("Request loan button connected");
        }

        // Initial update
        UpdateBetAmountDisplay();
        UpdateActiveBetsDisplay();
        UpdateLoanButton();
    }

    private void Update()
    {
        // Update displays every frame (or use events for better performance)
        UpdateBetAmountDisplay();
        UpdateActiveBetsDisplay();
        UpdateButtonStates();
        UpdateLoanButton();
    }

    private void ConnectBetButton(Button button, BetType betType)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => OnPlaceBet(betType));
        }
    }

    private void ConnectPresetBetButton(Button button, int amount)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => OnSetBetAmount(amount));
        }
    }

    /// <summary>
    /// Sets the bet amount to a preset value
    /// </summary>
    private void OnSetBetAmount(int amount)
    {
        if (CrapsGameManager.Instance != null)
        {
            CrapsGameManager.Instance.SetBetAmount(amount);
            Debug.Log($"Bet amount set to: ${amount}");
            UpdateBetAmountDisplay();
        }
    }

    private void OnIncreaseBet()
    {
        if (CrapsGameManager.Instance != null)
        {
            int oldAmount = CrapsGameManager.Instance.GetCurrentBetAmount();
            CrapsGameManager.Instance.IncreaseBetAmount();
            int newAmount = CrapsGameManager.Instance.GetCurrentBetAmount();
            Debug.Log($"Increase bet: ${oldAmount} -> ${newAmount}");
            UpdateBetAmountDisplay(); // Force immediate update
        }
        else
        {
            Debug.LogWarning("CrapsGameManager.Instance is null!");
        }
    }

    private void OnDecreaseBet()
    {
        if (CrapsGameManager.Instance != null)
        {
            int oldAmount = CrapsGameManager.Instance.GetCurrentBetAmount();
            CrapsGameManager.Instance.DecreaseBetAmount();
            int newAmount = CrapsGameManager.Instance.GetCurrentBetAmount();
            Debug.Log($"Decrease bet: ${oldAmount} -> ${newAmount}");
            UpdateBetAmountDisplay(); // Force immediate update
        }
        else
        {
            Debug.LogWarning("CrapsGameManager.Instance is null!");
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
        if (betAmountText == null)
        {
            Debug.LogWarning("BetAmountText is not assigned in BettingUI!");
            return;
        }

        if (CrapsGameManager.Instance == null)
        {
            betAmountText.text = "Bet Amount: $--";
            return;
        }

        int amount = CrapsGameManager.Instance.GetCurrentBetAmount();
        betAmountText.text = $"Bet Amount: ${amount}";
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

    /// <summary>
    /// Handles loan request button click
    /// </summary>
    private void OnRequestLoan()
    {
        if (AccountManager.Instance != null)
        {
            bool success = AccountManager.Instance.RequestLoan();
            if (success)
            {
                Debug.Log("Loan requested successfully!");
                UpdateBetAmountDisplay(); // Update money display
            }
            else
            {
                Debug.LogWarning("Cannot request loan - you have too much money!");
            }
        }
    }

    /// <summary>
    /// Updates loan button state and text
    /// </summary>
    private void UpdateLoanButton()
    {
        if (requestLoanButton == null) return;

        if (AccountManager.Instance == null)
        {
            requestLoanButton.interactable = false;
            return;
        }

        bool canRequest = AccountManager.Instance.CanRequestLoan();
        requestLoanButton.interactable = canRequest;

        // Update button text if assigned
        if (loanButtonText != null)
        {
            if (canRequest)
            {
                int currentMoney = AccountManager.Instance.GetMoney();
                int loanAmount = 500; // Default loan amount
                loanButtonText.text = $"Request Loan (${loanAmount})\nYou have: ${currentMoney}";
            }
            else
            {
                int currentMoney = AccountManager.Instance.GetMoney();
                loanButtonText.text = $"Request Loan\nYou have: ${currentMoney}";
            }
        }
    }
}

