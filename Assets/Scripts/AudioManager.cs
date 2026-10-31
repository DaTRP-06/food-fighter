using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource walkSource;

    [Header("Musik & Ambience")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip ambience;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip fireballHit;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip collect;
    [SerializeField] private AudioClip playerDamage;
    [SerializeField] private AudioClip walking;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartLoop(musicSource, backgroundMusic);
        StartLoop(ambienceSource, ambience);

        if (walkSource != null)
        {
            walkSource.clip = walking;
            walkSource.loop = true;
            walkSource.playOnAwake = false;
        }
    }

    private void StartLoop(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    public static void PlayJump()         { if (Instance != null) Instance.PlayOneShot(Instance.jump); }
    public static void PlayAttack()       { if (Instance != null) Instance.PlayOneShot(Instance.attack); }
    public static void PlayFireballHit()  { if (Instance != null) Instance.PlayOneShot(Instance.fireballHit); }
    public static void PlayEnemyDeath()   { if (Instance != null) Instance.PlayOneShot(Instance.enemyDeath); }
    public static void PlayCollect()      { if (Instance != null) Instance.PlayOneShot(Instance.collect); }
    public static void PlayPlayerDamage() { if (Instance != null) Instance.PlayOneShot(Instance.playerDamage); }

    /// <summary>Nyalakan / matikan suara langkah (loop).</summary>
    public static void SetWalking(bool isWalking)
    {
        if (Instance == null || Instance.walkSource == null || Instance.walking == null) return;

        if (isWalking && !Instance.walkSource.isPlaying)
            Instance.walkSource.Play();
        else if (!isWalking && Instance.walkSource.isPlaying)
            Instance.walkSource.Stop();
    }
}
