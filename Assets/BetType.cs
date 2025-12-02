using UnityEngine;

/// <summary>
/// Types of bets available in craps
/// </summary>
public enum BetType
{
    PassLine,      // Basic pass line bet (2:1)
    Place4,        // Place bet on 4 (9:5 payout)
    Place5,        // Place bet on 5 (7:5 payout)
    Place6,        // Place bet on 6 (7:6 payout)
    Place8,        // Place bet on 8 (7:6 payout)
    Place9,        // Place bet on 9 (7:5 payout)
    Place10,       // Place bet on 10 (9:5 payout)
    Field,         // Field bet (one roll, pays on 2,3,4,9,10,11,12)
    AnyCraps,      // One roll bet on 2, 3, or 12 (7:1 payout)
    Hard4,         // Hard way 4 (one roll, both dice show 2) (7:1 payout)
    Hard6,         // Hard way 6 (one roll, both dice show 3) (9:1 payout)
    Hard8,         // Hard way 8 (one roll, both dice show 4) (9:1 payout)
    Hard10         // Hard way 10 (one roll, both dice show 5) (7:1 payout)
}

/// <summary>
/// Represents an active bet
/// </summary>
[System.Serializable]
public class ActiveBet
{
    public BetType betType;
    public int amount;
    public int targetNumber; // For place bets and hard ways

    public ActiveBet(BetType type, int betAmount, int number = 0)
    {
        betType = type;
        amount = betAmount;
        targetNumber = number;
    }
}

