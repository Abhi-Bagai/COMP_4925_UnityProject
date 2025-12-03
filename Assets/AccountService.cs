using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/// <summary>
/// Service for managing cloud-hosted accounts
/// Handles sign-in, account creation, and data synchronization
/// </summary>
public class AccountService : MonoBehaviour
{
    public static AccountService Instance;

    [Header("API Configuration")]
    [SerializeField] private string apiBaseUrl = "https://your-api-endpoint.com/api"; // Replace with your actual API endpoint
    [SerializeField] private bool useLocalStorage = true; // Fallback to local storage if API unavailable

    private AccountData currentAccount;
    private bool isAuthenticated = false;

    // Events
    public event Action<AccountData> OnAccountLoaded;
    public event Action<string> OnLoginSuccess;
    public event Action<string> OnLoginFailed;
    public event Action<string> OnSignUpSuccess;
    public event Action<string> OnSignUpFailed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Signs in with username (finds existing account)
    /// </summary>
    public void SignIn(string username, Action<bool, AccountData> callback = null)
    {
        StartCoroutine(SignInCoroutine(username, callback));
    }

    private IEnumerator SignInCoroutine(string username, Action<bool, AccountData> callback)
    {
        if (useLocalStorage)
        {
            // Try local storage first
            string accountKey = $"Account_{username}";
            if (PlayerPrefs.HasKey(accountKey))
            {
                string json = PlayerPrefs.GetString(accountKey);
                AccountData account = AccountData.FromJson(json);
                currentAccount = account;
                isAuthenticated = true;
                OnAccountLoaded?.Invoke(account);
                OnLoginSuccess?.Invoke("Signed in successfully!");
                callback?.Invoke(true, account);
                yield break;
            }
        }

        // Try cloud API
        string url = $"{apiBaseUrl}/accounts/{username}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AccountData account = AccountData.FromJson(request.downloadHandler.text);
                currentAccount = account;
                isAuthenticated = true;
                
                // Save locally as backup
                if (useLocalStorage)
                {
                    SaveAccountLocally(account);
                }
                
                OnAccountLoaded?.Invoke(account);
                OnLoginSuccess?.Invoke("Signed in successfully!");
                callback?.Invoke(true, account);
            }
            else
            {
                // Account not found - could create new account
                string error = request.downloadHandler.text;
                OnLoginFailed?.Invoke($"Account not found: {username}");
                callback?.Invoke(false, null);
            }
        }
    }

    /// <summary>
    /// Creates a new account
    /// </summary>
    public void CreateAccount(string username, int startingMoney = 1000, Action<bool, AccountData> callback = null)
    {
        StartCoroutine(CreateAccountCoroutine(username, startingMoney, callback));
    }

    private IEnumerator CreateAccountCoroutine(string username, int startingMoney, Action<bool, AccountData> callback)
    {
        AccountData newAccount = new AccountData(username, startingMoney);

        if (useLocalStorage)
        {
            // Save locally first
            SaveAccountLocally(newAccount);
            currentAccount = newAccount;
            isAuthenticated = true;
            OnAccountLoaded?.Invoke(newAccount);
            OnSignUpSuccess?.Invoke("Account created successfully!");
            callback?.Invoke(true, newAccount);
        }

        // Try to save to cloud
        string url = $"{apiBaseUrl}/accounts";
        string json = newAccount.ToJson();
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Account created in cloud
                OnSignUpSuccess?.Invoke("Account created successfully!");
                callback?.Invoke(true, newAccount);
            }
            else
            {
                // Cloud save failed, but local save succeeded
                Debug.LogWarning($"Cloud save failed, but account saved locally: {request.error}");
                OnSignUpSuccess?.Invoke("Account created (local storage only)");
                callback?.Invoke(true, newAccount);
            }
        }
    }

    /// <summary>
    /// Syncs current account data to cloud
    /// </summary>
    public void SyncAccount(AccountData account, Action<bool> callback = null)
    {
        StartCoroutine(SyncAccountCoroutine(account, callback));
    }

    private IEnumerator SyncAccountCoroutine(AccountData account, Action<bool> callback)
    {
        if (account == null)
        {
            callback?.Invoke(false);
            yield break;
        }

        account.lastUpdated = DateTime.UtcNow.ToString("O");
        currentAccount = account;

        // Save locally first
        if (useLocalStorage)
        {
            SaveAccountLocally(account);
        }

        // Try to sync to cloud
        string url = $"{apiBaseUrl}/accounts/{account.accountId}";
        string json = account.ToJson();
        
        using (UnityWebRequest request = UnityWebRequest.Put(url, json))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogWarning($"Cloud sync failed: {request.error}");
                callback?.Invoke(false); // Still return true since local save succeeded
            }
        }
    }

    /// <summary>
    /// Saves account to local storage
    /// </summary>
    private void SaveAccountLocally(AccountData account)
    {
        string accountKey = $"Account_{account.username}";
        PlayerPrefs.SetString(accountKey, account.ToJson());
        PlayerPrefs.SetString("LastAccount", account.username);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads account from local storage
    /// </summary>
    public AccountData LoadAccountLocally(string username)
    {
        string accountKey = $"Account_{username}";
        if (PlayerPrefs.HasKey(accountKey))
        {
            string json = PlayerPrefs.GetString(accountKey);
            return AccountData.FromJson(json);
        }
        return null;
    }

    /// <summary>
    /// Gets the currently authenticated account
    /// </summary>
    public AccountData GetCurrentAccount()
    {
        return currentAccount;
    }

    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        return isAuthenticated && currentAccount != null;
    }

    /// <summary>
    /// Signs out current account
    /// </summary>
    public void SignOut()
    {
        if (currentAccount != null)
        {
            SyncAccount(currentAccount); // Sync before signing out
        }
        currentAccount = null;
        isAuthenticated = false;
    }

    /// <summary>
    /// Gets the last used account username from local storage
    /// </summary>
    public string GetLastAccountUsername()
    {
        return PlayerPrefs.GetString("LastAccount", "");
    }
}

