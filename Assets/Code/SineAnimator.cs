using UnityEngine;

public class SineAnimator : MonoBehaviour
{
    [field: SerializeField]public Vector3 Position { get; private set; }
    [field: SerializeField]public Vector3 Rotation { get; private set; }
    [field: SerializeField] public float SpeedPosition { get; private set; } = 1f;
    [field: SerializeField] public float SpeedRotation { get; private set; } = 1f;

    private Vector3 startPosition;
    private Vector3 startRotation;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.position = startPosition + (Position * Mathf.Sin(Time.time * SpeedPosition));
        transform.rotation = Quaternion.Euler(startRotation) * Quaternion.Euler(Rotation * Mathf.Sin(Time.time * SpeedRotation));
    }
}
