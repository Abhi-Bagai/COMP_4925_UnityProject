using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    private AudioPlayer player;
    [SerializeField]  ParticleSystem particleSystem;

    void Awake()
    {
        player = FindAnyObjectByType<AudioPlayer>();
    }

    public void LoadNewRun()
    {   
        player.PlayMenuMusic();
        particleSystem.Play();
        StartCoroutine(LoadAfterWait("TableGame", 0.5f));
    }

    IEnumerator LoadAfterWait(string scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }

        // public void StartRun()
    // {
    //     //SceneManager.LoadScene(1); // Load the game scene
    //     SceneManager.LoadScene("TableGame");
    //     SceneManager.LoadScene("TableGame", LoadSceneMode.Additive);
    // }

    // IEnumerator StartNewRunAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     SceneManager.LoadScene("TableGame");
    // }



        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
