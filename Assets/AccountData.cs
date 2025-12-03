using System;
using UnityEngine;

/// <summary>
/// Serializable account data structure for cloud storage
/// </summary>
[Serializable]
public class AccountData
{
    public string accountId;
    public string username;
    public int currentMoney;
    public int totalWins;
    public int totalLosses;
    public int currentWinStreak;
    public int bestWinStreak;
    public int totalGamesPlayed;
    public int totalMoneyWon;
    public int totalMoneyLost;
    public int totalLoaned;
    public string lastUpdated; // Store as string for JSON serialization

    public AccountData()
    {
        accountId = Guid.NewGuid().ToString();
        lastUpdated = DateTime.UtcNow.ToString("O"); // ISO 8601 format
    }

    public AccountData(string username, int startingMoney = 1000)
    {
        accountId = Guid.NewGuid().ToString();
        this.username = username;
        currentMoney = startingMoney;
        totalWins = 0;
        totalLosses = 0;
        currentWinStreak = 0;
        bestWinStreak = 0;
        totalGamesPlayed = 0;
        totalMoneyWon = 0;
        totalMoneyLost = 0;
        totalLoaned = 0;
        lastUpdated = DateTime.UtcNow.ToString("O"); // ISO 8601 format
    }

    public DateTime GetLastUpdated()
    {
        if (DateTime.TryParse(lastUpdated, out DateTime result))
        {
            return result;
        }
        return DateTime.UtcNow;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static AccountData FromJson(string json)
    {
        return JsonUtility.FromJson<AccountData>(json);
    }
}

