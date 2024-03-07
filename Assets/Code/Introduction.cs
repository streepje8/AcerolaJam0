using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Introduction : MonoBehaviour
{

    [field: SerializeField]public RawImage Fade { get; private set; } 
    private AudioSource source;
    private bool started = false;
    private bool finished = false;
    private float timer = 0;
    private float timerTwo = 0;
    private float fade = 0;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 4 && !started)
        {
            source.Play();
            started = true;
        }

        if (started && !source.isPlaying)
        {
            timerTwo += Time.deltaTime;
            fade = 1;
        }

        Fade.color = new Color(0, 0, 0, Mathf.Lerp(Fade.color.a, fade, 5f * Time.deltaTime));
        
        if (timerTwo > 3 && !finished)
        {
            SceneManager.LoadSceneAsync(3);
            finished = true;
        }
    }
}
