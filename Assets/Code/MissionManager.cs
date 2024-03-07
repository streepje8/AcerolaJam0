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

    public Mission CurrentMission { get; private set; } = new Mission("Invalid mission");
    public Stack<Mission> Missions { get; private set; } = new Stack<Mission>();
    public static MissionManager Instance { get; private set; }

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

        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GenerateMissions();
    }

    public void GenerateMissions()
    {
        foreach (var aberration in ToAnalyze.OrderBy(x => Random.Range(0,9999)))
        {
            Mission analyze = new Mission($"<color=\"green\">Analyze aberration '{aberration.Type}' by pointing your ship nose at if from different angles.</color>");
            analyze.OnStart.Add(() =>
            {
                ObjectiveText.text = $"Current Objective: {analyze.Objective}";
                Navigation.Instance.NavigateTo(aberration.transform.position);
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
                PlayVoiceover(aberration.NavigationCommandAudio);
            });
            navigate.OnComplete.Add(() =>
            {
                Instance.NextMission();
            });
            Missions.Push(navigate); //navigate
        }
    }

    private void PlayVoiceover(AudioClip voiceover)
    {
        source.PlayOneShot(voiceover);
    }

    public void NextMission()
    {
        CurrentMission = Missions.Pop();
        foreach (var action in CurrentMission.OnStart) action.Invoke();
    }
}
