using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    [Header("Dice Roll Clips")]
    [SerializeField] AudioClip diceRollSoundOne;
    [SerializeField] AudioClip diceRollSoundTwo;
    [SerializeField] AudioClip diceRollFallback;
    [SerializeField] AudioClip menuMusic;

    [SerializeField] [Range(0f, 1f)] float volume = 1f;

    static AudioPlayer instance = null;

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

    void Awake()
    {
        if (instance != null && instance != this)
            {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
