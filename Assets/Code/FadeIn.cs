using UnityEngine;using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class FadeIn : MonoBehaviour
{
    [field: SerializeField]
    public float Speed { get; private set; }
    private RawImage img;

    private void Awake()
    {
        img = GetComponent<RawImage>();
    }

    private void Update()
    {
        img.color = new Color(0, 0, 0, Mathf.Lerp(img.color.a, 0, Speed * Time.deltaTime));
    }
}
