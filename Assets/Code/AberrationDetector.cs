using System.Collections.Generic;
using UnityEngine;

public class AberrationDetector : MonoBehaviour
{
    [field: SerializeField]public LayerMask AberrationLayer { get; private set; }
    [field: SerializeField] public float ScanInterval { get; private set; } = 1;

    public float ClosestAberrationDistance { get; private set; } = 99999;
    private readonly List<Bounds> aberrationColliders = new List<Bounds>();
    private float timer = 0;

    private void Start()
    {
        foreach (var aberration in Aberrations.AllAberrations)
        {
            aberrationColliders.Add(aberration.GetComponentInChildren<Collider>(true).bounds);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > ScanInterval)
        {
            PerformScan();
            timer = 0;
        }
    }

    private void PerformScan()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 150, AberrationLayer))
        {
            Aberration aberration = hit.collider.gameObject.GetComponent<Aberration>();
            MissionManager.Instance.IncrementScan(aberration.Type);
        }

        ClosestAberrationDistance = 9999;
        foreach (var aberration in aberrationColliders)
        {
            ClosestAberrationDistance = Mathf.Min(ClosestAberrationDistance, Vector3.Distance(transform.position, aberration.ClosestPoint(transform.position)));
            if (aberration.Contains(transform.position))
            {
                StoryManager.Instance.TransitionToGameOver();
                enabled = false;
            }
        }
    }
}
