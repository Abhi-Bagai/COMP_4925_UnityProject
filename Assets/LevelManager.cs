using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    private AudioPlayer player;
    [SerializeField] new ParticleSystem particleSystem;

    void Awake()
    {
        player = FindAnyObjectByType<AudioPlayer>();
    }

    public void LoadNewRun()
    {   
        if (player != null)
        {
            player.PlayMenuMusic();
        }
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        StartCoroutine(LoadAfterWait("TableGame", 2f));
    }

    /// <summary>
    /// Loads the menu scene
    /// </summary>
    public void LoadMenu()
    {
        if (player != null)
        {
            player.PlayMenuMusic();
        }
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        StartCoroutine(LoadAfterWait("Menu", 2f));
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
