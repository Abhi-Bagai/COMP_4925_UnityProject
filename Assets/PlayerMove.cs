using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    
    [Header("Low Money Particle Effect")]
    [SerializeField] private ParticleSystem lowMoneyParticle;
    [SerializeField] private int lowMoneyThreshold = 100;
    
    private bool isParticlePlaying = false;
    
    void Start()
    {
        // Get particle system if not assigned
        if (lowMoneyParticle == null)
        {
            lowMoneyParticle = GetComponent<ParticleSystem>();
        }
        
        // Make sure particle doesn't play on start
        if (lowMoneyParticle != null)
        {
            lowMoneyParticle.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateLowMoneyParticle();
    }
    
    /// <summary>
    /// Controls the blue particle effect based on player's money
    /// Only plays when money is $100 or less
    /// </summary>
    private void UpdateLowMoneyParticle()
    {
        if (lowMoneyParticle == null || AccountManager.Instance == null) return;
        
        int currentMoney = AccountManager.Instance.GetMoney();
        bool shouldPlay = currentMoney <= lowMoneyThreshold;
        
        if (shouldPlay && !isParticlePlaying)
        {
            // Start playing particle - low on money!
            lowMoneyParticle.Play();
            isParticlePlaying = true;
        }
        else if (!shouldPlay && isParticlePlaying)
        {
            // Stop playing particle - has enough money
            lowMoneyParticle.Stop();
            isParticlePlaying = false;
        }
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
