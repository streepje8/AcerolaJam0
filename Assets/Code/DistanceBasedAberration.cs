using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class DistanceBasedAberration : MonoBehaviour
{
    [field: SerializeField]public AberrationDetector Detector { get; private set; }

    private PostProcessVolume volume;
    private ChromaticAberration chromaticAberration;

    private void Awake()
    {
        chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromaticAberration.enabled.Override(true);
        chromaticAberration.intensity.Override(0);
        volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, chromaticAberration);
    }

    private void Update()
    {
        chromaticAberration.intensity.Override((1.0f - Mathf.Clamp01(Detector.ClosestAberrationDistance / 100.0f)) * 0.1f);
    }

    private void OnDestroy()
    {
        RuntimeUtilities.DestroyVolume(volume, true, false);
    }
}
