using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI controller for the login/sign-in screen
/// </summary>
public class LoginScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField usernameInput;
    public Button signInButton;
    public Button createAccountButton;
    public TMP_Text statusText;
    public GameObject loadingPanel;
    public TMP_Text loadingText;

    [Header("Navigation")]
    public string homeSceneName = "Menu"; // Or create a separate Home scene

    private void Start()
    {
        // Setup button listeners
        if (signInButton != null)
        {
            signInButton.onClick.AddListener(OnSignInClicked);
        }

        if (createAccountButton != null)
        {
            createAccountButton.onClick.AddListener(OnCreateAccountClicked);
        }

        // Subscribe to account service events
        if (AccountService.Instance != null)
        {
            AccountService.Instance.OnLoginSuccess += OnLoginSuccess;
            AccountService.Instance.OnLoginFailed += OnLoginFailed;
            AccountService.Instance.OnSignUpSuccess += OnSignUpSuccess;
            AccountService.Instance.OnSignUpFailed += OnSignUpFailed;
            AccountService.Instance.OnAccountLoaded += OnAccountLoaded;
        }

        // Load last username if available
        if (usernameInput != null && AccountService.Instance != null)
        {
            string lastUsername = AccountService.Instance.GetLastAccountUsername();
            if (!string.IsNullOrEmpty(lastUsername))
            {
                usernameInput.text = lastUsername;
            }
        }

        UpdateStatus("Enter your username to sign in or create a new account");
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (AccountService.Instance != null)
        {
            AccountService.Instance.OnLoginSuccess -= OnLoginSuccess;
            AccountService.Instance.OnLoginFailed -= OnLoginFailed;
            AccountService.Instance.OnSignUpSuccess -= OnSignUpSuccess;
            AccountService.Instance.OnSignUpFailed -= OnSignUpFailed;
            AccountService.Instance.OnAccountLoaded -= OnAccountLoaded;
        }
    }

    /// <summary>
    /// Called when Sign In button is clicked
    /// </summary>
    public void OnSignInClicked()
    {
        string username = usernameInput != null ? usernameInput.text.Trim() : "";

        if (string.IsNullOrEmpty(username))
        {
            UpdateStatus("Please enter a username", Color.red);
            return;
        }

        if (username.Length < 3)
        {
            UpdateStatus("Username must be at least 3 characters", Color.red);
            return;
        }

        ShowLoading("Signing in...");
        
        if (AccountService.Instance != null)
        {
            AccountService.Instance.SignIn(username, (success, account) =>
            {
                HideLoading();
                if (success && account != null)
                {
                    // Account loaded, will navigate in OnAccountLoaded
                }
                else
                {
                    UpdateStatus("Account not found. Click 'Create Account' to create a new account.", Color.yellow);
                }
            });
        }
        else
        {
            HideLoading();
            UpdateStatus("Account service not available", Color.red);
        }
    }

    /// <summary>
    /// Called when Create Account button is clicked
    /// </summary>
    public void OnCreateAccountClicked()
    {
        string username = usernameInput != null ? usernameInput.text.Trim() : "";

        if (string.IsNullOrEmpty(username))
        {
            UpdateStatus("Please enter a username", Color.red);
            return;
        }

        if (username.Length < 3)
        {
            UpdateStatus("Username must be at least 3 characters", Color.red);
            return;
        }

        ShowLoading("Creating account...");
        
        if (AccountService.Instance != null)
        {
            AccountService.Instance.CreateAccount(username, 1000, (success, account) =>
            {
                HideLoading();
                if (success && account != null)
                {
                    // Account created, will navigate in OnAccountLoaded
                }
                else
                {
                    UpdateStatus("Failed to create account. Please try again.", Color.red);
                }
            });
        }
        else
        {
            HideLoading();
            UpdateStatus("Account service not available", Color.red);
        }
    }

    /// <summary>
    /// Called when login succeeds
    /// </summary>
    private void OnLoginSuccess(string message)
    {
        UpdateStatus(message, Color.green);
    }

    /// <summary>
    /// Called when login fails
    /// </summary>
    private void OnLoginFailed(string error)
    {
        UpdateStatus(error, Color.red);
    }

    /// <summary>
    /// Called when sign up succeeds
    /// </summary>
    private void OnSignUpSuccess(string message)
    {
        UpdateStatus(message, Color.green);
    }

    /// <summary>
    /// Called when sign up fails
    /// </summary>
    private void OnSignUpFailed(string error)
    {
        UpdateStatus(error, Color.red);
    }

    /// <summary>
    /// Called when account is loaded (after sign in or create)
    /// </summary>
    private void OnAccountLoaded(AccountData account)
    {
        if (account != null)
        {
            // Update AccountManager with loaded account data
            if (AccountManager.Instance != null)
            {
                AccountManager.Instance.LoadAccountData(account);
            }

            // Navigate to home screen
            NavigateToHome();
        }
    }

    /// <summary>
    /// Navigates to the home screen
    /// </summary>
    private void NavigateToHome()
    {
        SceneManager.LoadScene(homeSceneName);
    }

    /// <summary>
    /// Updates the status text
    /// </summary>
    private void UpdateStatus(string message, Color? color = null)
    {
        if (statusText != null)
        {
            statusText.text = message;
            if (color.HasValue)
            {
                statusText.color = color.Value;
            }
        }
    }

    /// <summary>
    /// Shows loading panel
    /// </summary>
    private void ShowLoading(string message = "Loading...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        if (loadingText != null)
        {
            loadingText.text = message;
        }
        if (signInButton != null) signInButton.interactable = false;
        if (createAccountButton != null) createAccountButton.interactable = false;
    }

    /// <summary>
    /// Hides loading panel
    /// </summary>
    private void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (signInButton != null) signInButton.interactable = true;
        if (createAccountButton != null) createAccountButton.interactable = true;
    }
}

