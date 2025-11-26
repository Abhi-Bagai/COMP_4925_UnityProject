using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]  ParticleSystem particleSystem;
    private LevelManager levelManager;
    private void Awake()
    {
        LevelManager levelManager = FindAnyObjectByType<LevelManager>();
        
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
