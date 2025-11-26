using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("UI")]
    public TMP_Text totalText; // assign in Inspector

    [Header("Cleanup Settings")]
    public float diceDisplayTime = 2f; // How long to show dice before removing them

    private List<DiceRoller2D> activeDice = new List<DiceRoller2D>();
    private List<GameObject> diceGameObjects = new List<GameObject>(); // Track GameObjects for cleanup
    private bool showingResult = false;
    private Coroutine cleanupCoroutine;
    private bool isRegisteringNewRoll = false; // Track if we're in the middle of registering a new roll

    void Awake()
    {
        Instance = this;
    }

    public void RegisterDice(DiceRoller2D dice)
    {
        // If this is the first dice of a new roll, clean up old dice
        if (!isRegisteringNewRoll)
        {
            // Stop any running cleanup coroutine
            if (cleanupCoroutine != null)
            {
                StopCoroutine(cleanupCoroutine);
                cleanupCoroutine = null;
            }
            
            // Clean up any old dice before registering new ones (safety measure)
            CleanupDice();
            
            // Mark that we're starting a new roll registration
            isRegisteringNewRoll = true;
            showingResult = false; // reset UI when new dice thrown
        }
        
        activeDice.Add(dice);
        diceGameObjects.Add(dice.gameObject); // Store GameObject reference for cleanup
    }

    void Update()
    {
        if (activeDice.Count == 0 || showingResult) return;

        bool allStopped = true;
        foreach (var d in activeDice)
        {
            if (!d.IsStopped())
            {
                allStopped = false;
                break;
            }
        }

        if (allStopped)
        {
            int sum = 0;
            foreach (var d in activeDice)
                sum += d.CurrentValue;

            ShowResult(sum);
            activeDice.Clear();
            isRegisteringNewRoll = false; // Reset flag after roll completes
            // Start cleanup coroutine to remove dice after display time
            cleanupCoroutine = StartCoroutine(CleanupDiceAfterDelay());
        }
    }

    private void ShowResult(int total)
    {
        showingResult = true;
        Debug.Log($"Total Roll: {total}");

        if (totalText != null)
        {
            totalText.text = $"Total: {total}";
            totalText.gameObject.SetActive(true);
        }

        // Notify CrapsGameManager that the roll is complete
        if (CrapsGameManager.Instance != null)
        {
            CrapsGameManager.Instance.OnRollCompleted(total);
        }
    }

    /// <summary>
    /// Removes dice from the table after a delay
    /// </summary>
    private IEnumerator CleanupDiceAfterDelay()
    {
        yield return new WaitForSeconds(diceDisplayTime);
        CleanupDice();
    }

    /// <summary>
    /// Immediately destroys all active dice GameObjects
    /// </summary>
    public void CleanupDice()
    {
        foreach (GameObject dice in diceGameObjects)
        {
            if (dice != null)
            {
                Destroy(dice);
            }
        }
        diceGameObjects.Clear();
        activeDice.Clear();
    }
}
