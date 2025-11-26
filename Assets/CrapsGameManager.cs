using UnityEngine;
using TMPro;

public class CrapsGameManager : MonoBehaviour
{
    public static CrapsGameManager Instance;

    [Header("UI")]
    public TMP_Text gameStateText; // Optional: display game state
    public TMP_Text pointText;     // Optional: display point value

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
        UpdateUI();
    }

    /// <summary>
    /// Called by DiceShooter to check if rolling is allowed
    /// </summary>
    public bool CanRoll()
    {
        return canRoll && !rollInProgress;
    }

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
    public void OnRollCompleted(int total)
    {
        if (!rollInProgress)
        {
            Debug.LogWarning("OnRollCompleted called but no roll was in progress");
            return;
        }

        rollInProgress = false;

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
            // Natural - win immediately
            Debug.Log($"Natural! {total} - You Win!");
            // Reset for next round
            ResetGameState();
        }
        else if (total == 2 || total == 3 || total == 12)
        {
            // Craps - lose immediately
            Debug.Log($"Craps! {total} - You Lose!");
            // Reset for next round
            ResetGameState();
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
            // Made the point - win
            Debug.Log($"Made the point! {total} - You Win!");
            // Reset for next round
            ResetGameState();
        }
        else if (total == 7)
        {
            // Seven out - lose
            Debug.Log($"Seven out! {total} - You Lose!");
            // Reset for next round
            ResetGameState();
        }
        else
        {
            // Continue rolling (any other number)
            Debug.Log($"Roll again. Rolled: {total}");
            canRoll = true; // Ready for next roll
        }
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

    // Public getters for UI or other systems
    public bool IsPointOn => pointIsOn;
    public int PointValue => pointValue;
    public bool IsComeoutRoll => isComeoutRoll;
    public bool IsRollInProgress => rollInProgress;
}

