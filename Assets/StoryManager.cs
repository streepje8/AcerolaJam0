using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class StoryManager : MonoBehaviour
{
    [field: SerializeField]public AudioClip IntroAudio { get; private set; }
    [field: SerializeField]public RawImage FadeImage { get; private set; }
    public float Fade { get; set; } = 0;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private float timer = 0f;
    private bool flagA = false;
    private bool flagB = false;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3 && !flagA)
        {
            source.PlayOneShot(IntroAudio);
            flagA = true;
        }

        if (flagA && !source.isPlaying && !flagB)
        {
            MissionManager.Instance.NextMission();
            flagB = true;
        }
        FadeImage.color = new Color(0, 0, 0, Mathf.Lerp(FadeImage.color.a, Fade, 5f * Time.deltaTime));
    }
}
