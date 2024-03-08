using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Mission
{
    [field: SerializeField]public string Objective { get; set; }
    [field: SerializeField]public List<Action> OnStart { get; set; }
    [field: SerializeField]public List<Action> OnComplete { get; set; }

    public Mission(string objective)
    {
        Objective = objective;
        OnStart = new List<Action>();
        OnComplete = new List<Action>();
    }
}

public class MissionManager : MonoBehaviour
{
    [field: SerializeField] public List<Aberration> ToAnalyze { get; private set; } = new List<Aberration>();
    [field: SerializeField] public TMP_Text ObjectiveText { get; private set; }
    [field: SerializeField] public TMP_Text ScannerText { get; private set; }

    public Mission CurrentMission { get; private set; } = new Mission("Invalid mission");
    public Stack<Mission> Missions { get; private set; } = new Stack<Mission>();
    public static MissionManager Instance { get; private set; }

    private bool isInNavigation = false;
    private int scanProgress = 0;
    private AberrationType currentAberrationType;

    private AudioSource source;

    private void Awake()
    {
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
        Random.InitState(Mathf.RoundToInt(DateTime.UtcNow.ToFileTimeUtc()));
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GenerateMissions();
    }

    public void GenerateMissions()
    {
        foreach (var aberration in ToAnalyze.OrderBy(x => Random.Range(0,9999)).ToArray())
        {
            Mission analyze = new Mission($"<color=\"green\">Analyze aberration '{aberration.AberrationName}' by pointing your ship nose at if from different angles.</color>");
            analyze.OnStart.Add(() =>
            {
                ObjectiveText.text = $"Current Objective: {analyze.Objective}";
                Navigation.Instance.NavigateTo(aberration.transform.position);
                currentAberrationType = aberration.Type;
                ShipAudio.Instance.Notify();
                PlayVoiceover(aberration.AnalyzingCommandAudio);
            });
            analyze.OnComplete.Add(() =>
            {
                Instance.NextMission();
            });
            Missions.Push(analyze);
            
            Mission navigate = new Mission("<color=\"green\">Follow your navigation to the next aberration.</color>");
            navigate.OnStart.Add(() =>
            {
                ObjectiveText.text = $"Current Objective: {navigate.Objective}";
                Navigation.Instance.NavigateTo(aberration.transform.position);
                currentAberrationType = aberration.Type;
                isInNavigation = true;
                scanProgress = 0;
                ScannerText.text = "";
                ShipAudio.Instance.Notify();
                PlayVoiceover(aberration.NavigationCommandAudio);
            });
            navigate.OnComplete.Add(() =>
            {
                Instance.NextMission();
            });
            Missions.Push(navigate); //navigate
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            while (Missions.Count > 0) Missions.Pop();
        }
    }

    private void PlayVoiceover(AudioClip voiceover)
    {
        source.PlayOneShot(voiceover);
    }

    private bool ended = false;
    public void NextMission()
    {
        if (ended) return;
        if (Missions.Count > 0)
        {
            CurrentMission = Missions.Pop();
            foreach (var action in CurrentMission.OnStart) action.Invoke();
        }
        else
        {
            ObjectiveText.text = $"Current Objective: <color=\"green\">RUN!fvddfgjnk</color>";
            ShipAudio.Instance.Notify();
            StoryManager.Instance.PlayEnding();
            ended = true;
        }
    }

    public void IncrementScan(AberrationType type)
    {
        if (type == currentAberrationType)
        {
            if (isInNavigation)
            {
                NextMission();
                isInNavigation = false;
                ShipAudio.Instance.Beep();
                ScannerText.text = "Scanning, Progress: <color=\"red\">||||||||||</color>";
            }
            else
            {
                if (scanProgress >= 11)
                {
                    NextMission();
                    return;
                }
                ShipAudio.Instance.Notify();
                scanProgress++;
                string scannerText = "Scanning, Progress: <color=\"green\">";
                for (int i = 0; i < scanProgress; i++) scannerText += "|";
                scannerText += "</color><color=\"red\">";
                for (int i = 0; i < (11 - scanProgress); i++) scannerText += "|";
                scannerText += "</color>";
                ScannerText.text = scannerText;
                Debug.Log($"Scan Progress [{scanProgress}/10]");
            }
        }
    }
}
