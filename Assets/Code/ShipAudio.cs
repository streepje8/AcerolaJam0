using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShipAudio : MonoBehaviour
{
    [field: SerializeField]public AudioClip NotificationSound { get; private set; }
    [field: SerializeField]public AudioClip AlertSound { get; private set; }
    [field: SerializeField]public AudioClip BeepSound { get; private set; }
    [field: SerializeField]public AudioClip BoopBoopSound { get; private set; }
    [field: SerializeField]public AudioClip TutuSound { get; private set; }
    
    public static ShipAudio Instance { get; private set; } 

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
            Debug.LogWarning("Overwrote a singleton, this should only happen on a scene change!");
        }
    }

    public void Notify() => source.PlayOneShot(NotificationSound);
    public void Beep() => source.PlayOneShot(BeepSound);
    public void BoopBoop() => source.PlayOneShot(BoopBoopSound);
    public void Alert() => source.PlayOneShot(AlertSound);
    public void Tutu() => source.PlayOneShot(TutuSound);
    
}
