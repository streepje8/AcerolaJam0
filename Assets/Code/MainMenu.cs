using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    [field: SerializeField]public AudioClip ButtonSound { get; private set; }
    [field: SerializeField]public RawImage Fade { get; private set; }
    private AudioSource source;
    public bool fadeOut = false;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Tutorial()
    {
        SceneManager.LoadSceneAsync(1);
        source.PlayOneShot(ButtonSound);
        fadeOut = true;
    }

    public void Play()
    {
        SceneManager.LoadSceneAsync(2);
        source.PlayOneShot(ButtonSound);
        fadeOut = true;
    }

    public void QuitGame()
    {
        source.PlayOneShot(ButtonSound);
        fadeOut = true;
        Application.Quit();
    }

    private void Update()
    {
        Fade.color = new Color(0, 0, 0, Mathf.Lerp(Fade.color.a,fadeOut ? 1 : 0, 10f * Time.deltaTime));
    }
}
