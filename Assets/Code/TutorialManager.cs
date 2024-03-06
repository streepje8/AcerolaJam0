using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TutorialFase
{
    Introduction,
    RollInstruction,
    LevelOut,
    ForwardThrust,
    Instrumentation,
    TurnInstruction,
    PitchInstruction
}

[RequireComponent(typeof(AudioSource))]
public class TutorialManager : MonoBehaviour
{
    [field: SerializeField]public List<GameObject> Areas { get; private set; } = new List<GameObject>();
    [field: SerializeField]public List<Collider> Regions { get; private set; } = new List<Collider>();
    [field: SerializeField]public List<AudioClip> InstructionAudios { get; private set; }  = new List<AudioClip>();
    [field: SerializeField]public List<AudioClip> ComplementAudios { get; private set; }  = new List<AudioClip>();
    [field: SerializeField]public Transform Player { get; private set; }
    [field: SerializeField]public Collider ThatOneSpecialWall { get; private set; }
    
    public TutorialFase CurrentFase { get; private set; } = TutorialFase.Introduction;
    public int CurrentInstructionIndex { get; private set; } = 0;
    public int CurrentAreaIndex { get; private set; } = 0;
    
    public bool TutorialStarted { get; private set; } = false;
    
    private AudioSource source;
    private Rigidbody pRb;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        pRb = Player.GetComponent<Rigidbody>();
        if(ThatOneSpecialWall != null) ThatOneSpecialWall.gameObject.SetActive(true);
    }

    public void ToggleNextArea()
    {
        Areas[CurrentAreaIndex]?.SetActive(false);
        CurrentAreaIndex++;
        Areas[CurrentAreaIndex]?.SetActive(true);
    }
    
    public void PlayNextInstruction()
    {
        source.Stop();
        source.PlayOneShot(InstructionAudios[CurrentInstructionIndex]);
        CurrentInstructionIndex++;
    }

    private float timer = 0;
    private float timerTwo = 0;
    private bool flagA = false;
    private bool flagB = false;
    private bool flagC = false;
    private bool rotationLock = false;
    
    //YES the tutorial is entirely hardcoded because i need to save some time 0-o
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))source.Stop();
        switch (CurrentFase)
        {
            case TutorialFase.Introduction:
                if (TutorialStarted)
                {
                    if (!source.isPlaying)
                    {
                        timer += Time.deltaTime;
                        if (timer > 1)
                        {
                            timer = 0;
                            PlayNextInstruction();
                            Areas[0]?.SetActive(true);
                            CurrentFase = TutorialFase.RollInstruction;
                        }
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer >= 3)
                    {
                        timer = 0;
                        PlayNextInstruction();
                        TutorialStarted = true;
                    }
                }
                break;
            case TutorialFase.RollInstruction:
                if (flagB)
                {
                    if (!source.isPlaying)
                    {
                        ToggleNextArea();
                        PlayNextInstruction();
                        timer = 0;
                        flagA = false;
                        flagB = false;
                        CurrentFase = TutorialFase.Instrumentation;
                    }
                }
                else
                {
                    if (!flagA)
                    {
                        if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.S)) flagA = true;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                        if (timer > 8)
                        {
                            PlayComplement();
                            flagB = true;
                        }
                    }
                }
                break;
            case TutorialFase.Instrumentation:
                if (!source.isPlaying)
                {
                    timer += Time.deltaTime;
                    if (timer > 2f)
                    {
                        timer = 0;
                        flagA = false;
                        flagB = false;
                        ToggleNextArea();
                        PlayNextInstruction();
                        CurrentFase = TutorialFase.LevelOut;
                    }
                }
                break;
            case TutorialFase.LevelOut:
                if (flagB)
                {
                    if (!source.isPlaying)
                    {
                        ToggleNextArea();
                        PlayNextInstruction();
                        timer = 0;
                        flagA = false;
                        flagB = false;
                        if(ThatOneSpecialWall != null) ThatOneSpecialWall.gameObject.SetActive(false);
                        CurrentFase = TutorialFase.ForwardThrust;
                    }
                }
                else
                {
                    if (flagA)
                    {
                        if (Vector3.Distance(Vector3.up, Player.rotation * Vector3.up) < 0.01f) timer += Time.deltaTime;
                        if (timer > 2)
                        {
                            PlayComplement();
                            flagB = true;
                        }
                    } else {
                        timer += Time.deltaTime;
                        if (timer > 2)
                        {
                            flagA = true;
                            timer = 0;
                        }
                    }
                }
                break;
            case TutorialFase.ForwardThrust:
                if (flagB)
                {
                    if (!source.isPlaying)
                    {
                        ToggleNextArea();
                        PlayNextInstruction();
                        timer = 0;
                        flagA = false;
                        flagB = false;
                        rotationLock = true;
                        CurrentFase = TutorialFase.TurnInstruction;
                    }
                }
                else
                {
                    if (flagA)
                    {
                        if (Regions[0].bounds.Contains(Player.transform.position)) timer += Time.deltaTime;
                        if (timer > 6)
                        {
                            PlayComplement();
                            flagB = true;
                        }
                    } else {
                        timer += Time.deltaTime;
                        if (timer > 2)
                        {
                            flagA = true;
                            timer = 0;
                        }
                    }
                }
                break;
            case TutorialFase.TurnInstruction:
                if (flagB)
                {
                    if (!source.isPlaying)
                    {
                        ToggleNextArea();
                        CurrentInstructionIndex = 7; //Skip the repeat
                        PlayNextInstruction();
                        timer = 0;
                        timerTwo = 0;
                        flagA = false;
                        flagB = false;
                        flagC = false;
                        rotationLock = false;
                        CurrentFase = TutorialFase.PitchInstruction;
                    }
                }
                else
                {
                    if (flagA)
                    {
                        if(!flagC)timerTwo += Time.deltaTime;
                        if (timerTwo > 10)
                        {
                            timerTwo = 0;
                            PlayNextInstruction();
                            flagC = true;
                        }
                        if (Regions[1].bounds.Contains(Player.transform.position)) timer += Time.deltaTime;
                        if (timer > 6)
                        {
                            PlayComplement();
                            flagB = true;
                        }
                    } else {
                        timer += Time.deltaTime;
                        if (timer > 2)
                        {
                            flagA = true;
                            timer = 0;
                        }
                    }
                }
                break;
            case TutorialFase.PitchInstruction:
                if (flagB)
                {
                    if (!source.isPlaying)
                    {
                        ToggleNextArea();
                        PlayNextInstruction();
                        timer = 0;
                        flagA = false;
                        flagB = false;
                        rotationLock = false;
                        CurrentFase = TutorialFase.PitchInstruction;
                    }
                }
                else
                {
                    if (flagA)
                    {
                        if (Regions[2].bounds.Contains(Player.transform.position)) timer += Time.deltaTime;
                        if (timer > 6)
                        {
                            PlayComplement();
                            flagB = true;
                        }
                    } else {
                        timer += Time.deltaTime;
                        if (timer > 2)
                        {
                            flagA = true;
                            timer = 0;
                        }
                    }
                }
                break;
        }

    }

    private void FixedUpdate()
    {
        if (rotationLock)
        {
            Quaternion newRotation = Quaternion.Euler(0,Player.transform.rotation.eulerAngles.y,0);
            pRb.MoveRotation(newRotation);
            Player.rotation = newRotation;
        }
    }

    private void PlayComplement()
    {
        source.Stop();
        source.PlayOneShot(ComplementAudios.OrderBy(x => Random.Range(0,999)).First());
    }
}