using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class EndMenu : MonoBehaviour
{
    [field: SerializeField]public RectTransform BaseMenu { get; private set; }
    [field: SerializeField]public RectTransform SubMenu { get; private set; }
    [field: SerializeField]public AudioClip ClickSound { get; private set; }

    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void QuitGame()
    {
        source.PlayOneShot(ClickSound);
        Application.Quit();
    }

    public void Credits()
    {
        source.PlayOneShot(ClickSound);
        BaseMenu.gameObject.SetActive(false);
        SubMenu.gameObject.SetActive(true);
    }
    
    public void Back()
    {
        source.PlayOneShot(ClickSound);
        BaseMenu.gameObject.SetActive(true);
        SubMenu.gameObject.SetActive(false);
    }
    
    public void Replay()
    {
        source.PlayOneShot(ClickSound);
        SceneManager.LoadSceneAsync(0);
    }
}
