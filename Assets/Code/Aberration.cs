using System.Runtime.InteropServices;
using UnityEngine;

public enum AberrationType
{
    TypeA = 0,
    TypeB = 1,
    TypeC = 2,
    TypeD = 3,
    TypeE = 4
}

[StructLayout(LayoutKind.Sequential)]
public struct AberrationStruct
{
    public Vector3 position;
    public Vector3 scale;
    public int id;
}

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider))]
public class Aberration : MonoBehaviour
{
    [field: SerializeField]public AberrationType Type { get; private set; }
    [field: SerializeField]public string AberrationName { get; private set; }
    [field: SerializeField]public AudioClip NavigationCommandAudio { get; private set; }
    [field: SerializeField]public AudioClip AnalyzingCommandAudio { get; private set; }

    private BoxCollider col;
    
    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        FindObjectOfType<Aberrations>().RequestRecacheAberrations();
    }

    public AberrationStruct ToStruct()
    {
        return new AberrationStruct()
        {
            position = transform.position,
            scale = transform.lossyScale,
            id = (int)Type
        };
    }

    private void OnDestroy()
    {
        //For when you destroy an aberration in the scene view
        FindObjectOfType<Aberrations>()?.RequestRecacheAberrations();
    }
}
