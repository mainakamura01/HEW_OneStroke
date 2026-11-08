
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip nodeActivateSE;
    [SerializeField] private AudioClip polygonCompleteSE;
    [SerializeField] private AudioClip playerDamageSE;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float nodeVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float polygonVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float damageVolume = 0.9f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayNodeActivate()
    {
        PlaySE(nodeActivateSE, nodeVolume);
    }

    public void PlayPolygonComplete()
    {
        PlaySE(polygonCompleteSE, polygonVolume);
    }

    public void PlayPlayerDamage()
    {
        PlaySE(playerDamageSE, damageVolume);
    }

    private void PlaySE(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null)
            return;

        // ìØéûÇ…ï°êîÇÃå¯â âπÇçƒê∂Ç≈Ç´ÇÈ
        audioSource.PlayOneShot(clip, volume);
    }
}
