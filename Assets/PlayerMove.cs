using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        this.gameObject.transform.Translate(
                                            horizontal * moveSpeed * Time.deltaTime, 
                                            vertical * moveSpeed * Time.deltaTime, 
                                            0);
        bool fire1 = Input.GetButtonDown("Fire1");
        if (fire1)
        {
            Debug.Log("Fire1 button pressed");

    }
    }
}
