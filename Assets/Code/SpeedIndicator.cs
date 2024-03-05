using TMPro;
using UnityEngine;

public class SpeedIndicator : MonoBehaviour
{
    [field: SerializeField]public TMP_Text Text { get; private set; }
    [field: SerializeField]public string Suffix { get; private set; } = "su/s";
    [field: SerializeField]public Rigidbody ToMeasure { get; private set; }

    private float speed = 0;
    private void Update()
    {
        speed = Mathf.Lerp(speed,((ToMeasure.velocity.magnitude * 10f) / 195.0f) * 200f, 5f * Time.deltaTime);
        Text.text = $"{Mathf.FloorToInt(speed)} {Suffix}";
    }
}
