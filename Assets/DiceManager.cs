using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("UI")]
    public TMP_Text totalText; // assign in Inspector

    private List<DiceRoller2D> activeDice = new List<DiceRoller2D>();
    private bool showingResult = false;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterDice(DiceRoller2D dice)
    {
        activeDice.Add(dice);
        showingResult = false; // reset UI when new dice thrown
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
    }
}
