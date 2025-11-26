using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
public class DiceRoller2D : MonoBehaviour
{
    private AudioPlayer player;

    [Header("Dice Sprites (1–6)")]
    public Sprite[] diceFaces;

    [Header("Roll Settings")]
    public float lateralMoveThreshold = 0.1f;   // how much XY movement counts as rolling
    public float faceChangeSpeed = 0.05f;       // flicker speed
    public float collisionImpactThreshold = 1f; // min impact to flicker
    public float settleVelocityThreshold = 0.05f;
    public float settleAngularThreshold = 5f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int currentValue = 1;
    private bool isRolling = false;
    private bool hasFinalValue = false;

    void Awake()
    {
        player = FindAnyObjectByType<AudioPlayer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        RollOnce(); // initial random face
    }

    void Update()
    {
        float lateralSpeed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y).magnitude;

        if (lateralSpeed > lateralMoveThreshold)
        {
            if (!isRolling)
                StartCoroutine(RollAnimation());
            hasFinalValue = false;
        }
        else if (!isRolling && !hasFinalValue && IsSettled())
        {
            // lock current face as the final logical value
            LockFinalValue();
            hasFinalValue = true;
        }
    }

    IEnumerator RollAnimation()
    {
        isRolling = true;

        while (new Vector2(rb.linearVelocity.x, rb.linearVelocity.y).magnitude > lateralMoveThreshold)
        {
            int newValue = Random.Range(1, 7);
            spriteRenderer.sprite = diceFaces[newValue - 1];
            currentValue = newValue; // keep currentValue synced with what’s shown
            yield return new WaitForSeconds(faceChangeSpeed);
        }

        isRolling = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > collisionImpactThreshold && !isRolling)
        {
            StartCoroutine(RollAnimation());
            player?.PlayDiceRollSoundOne();
        }
    }

    // Single random roll (for initial state)
    private void RollOnce()
    {
        currentValue = Random.Range(1, 7);
        spriteRenderer.sprite = diceFaces[currentValue - 1];
        player?.PlayDiceRollSoundTwo();
    }

    // When dice settle, just lock the last visible face (no new random)
    private void LockFinalValue()
    {
        Debug.Log("Final value locked: " + currentValue);
    }

    public int CurrentValue => currentValue;

    public bool IsStopped() => IsSettled();

    private bool IsSettled()
    {
        Vector2 lateralVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        return lateralVelocity.magnitude < settleVelocityThreshold &&
               Mathf.Abs(rb.angularVelocity) < settleAngularThreshold;
    }
}
