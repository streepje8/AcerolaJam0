using UnityEngine;

public class AberrationDetector : MonoBehaviour
{
    [field: SerializeField]public LayerMask AberrationLayer { get; private set; }
    [field: SerializeField] public float ScanInterval { get; private set; } = 1;


    private float timer = 0;
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
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100, AberrationLayer))
        {
            Aberration aberration = hit.collider.gameObject.GetComponent<Aberration>();
            Debug.Log("Aberration detected: " + aberration.Type);
        }
    }
}
