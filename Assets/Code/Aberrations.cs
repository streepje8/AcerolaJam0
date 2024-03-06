using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteAlways]
public class Aberrations : MonoBehaviour
{
    [field: SerializeField]public Material SkyMaterial { get; private set; }

    private List<Aberration> aberrations = new List<Aberration>();
    
    private ComputeBuffer aberrationBuffer;
    private static readonly int AberrationsShaderProperty = Shader.PropertyToID("_Aberrations");
    private static readonly int AberrationCountShaderProperty = Shader.PropertyToID("_AberrationCount");

    private void OnEnable()
    {
        RecacheAberrations();
    }
    
    private void OnDisable()
    {
        if (aberrationBuffer != null)
        {
            aberrationBuffer.Dispose();
            aberrationBuffer = null;
        }
    }

    public void RequestRecacheAberrations()
    {
        if(aberrationBuffer != null) RecacheAberrations();
    }

    private void RecacheAberrations()
    {
        if (aberrationBuffer != null)
        {
            aberrationBuffer.Dispose();
            aberrationBuffer = null;
        }
        
        aberrations = GetComponentsInChildren<Aberration>().ToList();
        if(aberrations.Count > 0) aberrationBuffer = new ComputeBuffer(aberrations.Count, Marshal.SizeOf(typeof(AberrationStruct)));
    }

    private void Update()
    {
        if (aberrationBuffer != null)
        {
            aberrationBuffer.SetData(aberrations.Select(x => x.ToStruct()).ToArray());
            SkyMaterial.SetBuffer(AberrationsShaderProperty, aberrationBuffer);
            SkyMaterial.SetInt(AberrationCountShaderProperty, aberrations.Count);
        }
    }

    private void OnApplicationQuit()
    {
        if (aberrationBuffer != null)
        {
            aberrationBuffer.Dispose();
            aberrationBuffer = null;
        }
    }

    private void OnDestroy()
    {
        if (aberrationBuffer != null)
        {
            aberrationBuffer.Dispose();
            aberrationBuffer = null;
        }
    }
}
