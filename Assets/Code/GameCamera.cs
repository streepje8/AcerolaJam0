using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [field: SerializeField]public float LookAboveTarget { get; private set; } = 5;
    [field: SerializeField]public Transform Target  { get; private set; }
    [field: SerializeField]public Transform Offset  { get; private set; }
    private Camera Camera  { get; set; }
    void Awake()
    {
        Camera = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Target.position, 1f * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, 4f * Time.fixedDeltaTime);
        Quaternion goalCameraRotation = Quaternion.FromToRotation(Vector3.forward, ((Target.transform.position + Vector3.up * LookAboveTarget) - Camera.transform.position).normalized);
        Camera.transform.rotation = Quaternion.Slerp(Camera.transform.rotation, Quaternion.Euler(goalCameraRotation.eulerAngles.x, goalCameraRotation.eulerAngles.y,0) , 10f * Time.fixedDeltaTime);
    }
}
