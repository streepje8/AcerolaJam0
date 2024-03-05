using UnityEngine;

public class AttitudeIndicator : MonoBehaviour
{
    [field: SerializeField]public float MaxMovement { get; private set; } = 220;
    [field: SerializeField]public Transform ToMeasure { get; private set; }
    [field: SerializeField]public RectTransform Lines { get; private set; }
    [field: SerializeField]public RectTransform Horizon { get; private set; }
    [field: SerializeField]public RectTransform ZeroPoint { get; private set; }
    
    void FixedUpdate()
    {
        Vector3 refRot = ToMeasure.localRotation.eulerAngles;
        var refRotZ = refRot.z % 360.0f;
        if (refRotZ > 180) refRotZ = refRotZ - 360.0f;
        if (Mathf.Abs(refRotZ) < 90f)
        {
            //Right Side up mode
            Lines.localRotation = Quaternion.Slerp(Lines.localRotation, Quaternion.Euler(0,0,refRotZ), 10f * Time.fixedDeltaTime);
            Horizon.localPosition = Vector3.Lerp(Horizon.localPosition, new Vector3(0,Mathf.Lerp(-MaxMovement,MaxMovement,((refRot.x + 180.0f) % 360.0f) / 360f)), 10f * Time.fixedDeltaTime);
            ZeroPoint.localPosition = Vector3.Lerp(ZeroPoint.localPosition, new Vector3(Mathf.Lerp(-MaxMovement,MaxMovement,((refRot.y + 180.0f) % 360.0f) / 360f),0), 10f * Time.fixedDeltaTime);
        }
        else
        {
            refRotZ -= 180.0f;
            Lines.localRotation = Quaternion.Slerp(Lines.localRotation, Quaternion.Euler(0, 0, refRotZ), 10f * Time.fixedDeltaTime);
            Horizon.localPosition = Vector3.Lerp(Horizon.localPosition, new Vector3(0,Mathf.Lerp(-MaxMovement,MaxMovement,((refRot.x + 180.0f) % 360.0f) / 360f)), 10f * Time.fixedDeltaTime);
            ZeroPoint.localPosition = Vector3.Lerp(ZeroPoint.localPosition, new Vector3(Mathf.Lerp(-MaxMovement,MaxMovement,((refRot.y + 180.0f) % 360.0f) / 360f),0), 10f * Time.fixedDeltaTime);
        }
    }
}
