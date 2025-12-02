using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    [Header("Dice Roll Clips")]
    [SerializeField] AudioClip diceRollSoundOne;
    [SerializeField] AudioClip diceRollSoundTwo;
    [SerializeField] AudioClip diceRollFallback;
    [SerializeField] AudioClip menuMusic;

    [Header("Game Outcome Clips")]
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;

    [SerializeField] [Range(0f, 1f)] float volume = 1f;

    AudioClip GetClip(AudioClip preferredClip)
    {
        if (preferredClip != null)
        {
            return preferredClip;
        }
        return diceRollFallback;
    }

    void PlayClip(AudioClip clip)
    {
        AudioClip clipToPlay = GetClip(clip);
        if (clipToPlay == null)
        {
            return;
        }

        Vector3 position = Camera.main != null ? Camera.main.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
    }

    public void PlaySound()
    {
        PlayClip(diceRollFallback);
    }

    public void PlayDiceRollSoundOne()
    {
        PlayClip(diceRollSoundOne);
    }

    public void PlayDiceRollSoundTwo()
    {
        PlayClip(diceRollSoundTwo);
    }

    public void PlayMenuMusic()
    {
        PlayClip(menuMusic);
    }

    public void PlayWinSound()
    {
        PlayClip(winSound);
    }

    public void PlayLoseSound()
    {
        PlayClip(loseSound);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
