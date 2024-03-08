using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FadeAudioIn : MonoBehaviour
{
    [field: SerializeField] public float ToVolume { get; private set; } = 1;
    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0;
    }

    private void Update()
    {
        source.volume = Mathf.Lerp(source.volume, ToVolume, 0.5f * Time.deltaTime);
    }
}
