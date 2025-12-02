using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] new ParticleSystem particleSystem;
    private LevelManager levelManager;
    
    private void Awake()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void PressedButton()
    {
        levelManager.LoadNewRun();
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
