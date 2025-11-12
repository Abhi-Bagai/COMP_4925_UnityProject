using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DiceShooter : MonoBehaviour
{
    [Header("Dice Settings")]
    public GameObject dicePrefab;       // assign your dice prefab here
    public int diceCount = 2;
    public float powerMultiplier = 10f; // adjust for shot strength

    [Header("Spawn Settings")]
    public Transform spawnPoint;        // empty object at center of table
    public float spawnSpread = 0.5f;    // random offset between dice

    [Header("Line Settings")]
    public Color lineColor = Color.white;
    public float lineWidth = 0.05f;

    private Vector2 dragStart;
    private bool isDragging;
    private LineRenderer line;

    void Awake()
    {
        // setup the line renderer for aim visualization
        line = GetComponent<LineRenderer>();
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
    }

void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Debug.Log("Mouse down!");
        dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    if (isDragging)
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DrawLine(dragStart, currentPos);
    }

    if (Input.GetMouseButtonUp(0) && isDragging)
    {
        Vector2 dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (dragStart - dragEnd).normalized;
        float power = (dragStart - dragEnd).magnitude * powerMultiplier;

        ShootDice(direction, power);
        ClearLine();
        isDragging = false;
    }
}

    void ShootDice(Vector2 direction, float power)
    {
        for (int i = 0; i < diceCount; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-spawnSpread, spawnSpread),
                Random.Range(-spawnSpread, spawnSpread)
            );
            Vector2 spawnPos = (spawnPoint ? (Vector2)spawnPoint.position : Vector2.zero) + offset;

            GameObject dice = Instantiate(dicePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = dice.GetComponent<Rigidbody2D>();

            // throw the dice
            rb.gravityScale = 0; // no downward gravity
            rb.AddForce(direction * power, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);

            DiceManager.Instance.RegisterDice(dice.GetComponent<DiceRoller2D>());
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void ClearLine()
    {
        line.positionCount = 0;
    }
}
