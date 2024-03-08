using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameOverButton : MonoBehaviour
{
    [field: SerializeField]public AudioClip ClickSound { get; private set; }
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Click()
    {
        source.PlayOneShot(ClickSound);
        SceneManager.LoadSceneAsync(3);
        gameObject.SetActive(false);
    }
}
