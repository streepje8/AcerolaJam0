using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class StoryManager : MonoBehaviour
{
    [field: SerializeField]public AudioClip IntroAudio { get; private set; }
    [field: SerializeField]public AudioClip OutroAudio { get; private set; }
    [field: SerializeField]public RawImage FadeImage { get; private set; }
    [field: SerializeField]public GameObject EnemyShip { get; private set; }
    public static StoryManager Instance { get; private set; }
    public float Fade { get; set; } = 0;
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

    private float timer = 0f;
    private bool flagA = false;
    private bool flagB = false;
    private bool enemyShipActive = false;
    private bool flagC = false;
    private float timerThree = 0f;
    private bool flagD = false;
    private bool flagE = false;
    private int nextSceneIndex = 0;
    private float TimeTilPlayer = 0;
    private Vector3 originalEnemyShipPosition = Vector3.zero;
    
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
        if (enemyShipActive)
        {
            Navigation.Instance.NavigateTo((Navigation.Instance.transform.position - EnemyShip.transform.position).normalized * 100f);
            if (!flagE && Vector3.Distance(Navigation.Instance.transform.position, EnemyShip.transform.position) < 5)
            {
                flagE = true;
                TransitionToEndScreen();
            }

            EnemyShip.transform.position = Vector3.Lerp(originalEnemyShipPosition, Navigation.Instance.transform.position, TimeTilPlayer);
            TimeTilPlayer += Time.deltaTime / 30.0f;
            EnemyShip.transform.rotation = Quaternion.Slerp(EnemyShip.transform.rotation, Quaternion.LookRotation((Navigation.Instance.transform.position - EnemyShip.transform.position).normalized, Vector3.up), 10f * Time.deltaTime);
        }

        if (flagC)
        {
            timerThree -= Time.deltaTime;
            if (timerThree <= 0 && !flagD)
            {
                SceneManager.LoadSceneAsync(nextSceneIndex);
                flagD = true;
            }
        }
    }

    public void TransitionToGameOver()
    {
        FadeImage.color = new Color(0, 0, 0, 0.6f);
        Fade = 1;
        ShipAudio.Instance.Alert();
        timerThree = 2;
        nextSceneIndex = 5;
        flagC = true;
    }
    
    private void TransitionToEndScreen()
    {
        Fade = 1;
        ShipAudio.Instance.Tutu();
        timerThree = 5;
        nextSceneIndex = 4;
        flagC = true;
    }

    public void PlayEnding()
    {
        source.PlayOneShot(OutroAudio);
        EnemyShip.transform.position = ShipAudio.Instance.gameObject.transform.position +
                                       ShipAudio.Instance.gameObject.transform.forward * 300;
        originalEnemyShipPosition = EnemyShip.transform.position;
        EnemyShip.gameObject.SetActive(true);
        enemyShipActive = true;
    }
}
