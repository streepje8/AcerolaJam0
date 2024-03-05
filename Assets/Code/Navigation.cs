using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Navigation : MonoBehaviour
{
    private Vector3 GoalPos { get; set; } = Vector3.zero;
    private Vector3 DestPos { get; set; } = Vector3.zero;
    public static Navigation Instance { get; private set; } 

    private void Awake()
    {
        DestPos = Vector3.one * 100;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
            Debug.LogWarning("Overwrote a singleton, this should only happen on a scene change!");
        }
    }

    public void NavigateTo(Vector3 pos) => DestPos = pos;

    void Update()
    {
        GoalPos = Vector3.Lerp(GoalPos, DestPos, 1f * Time.deltaTime);
        if (GoalPos.sqrMagnitude > 0.01f)
        {
            Vector3 direction = (GoalPos - transform.position).normalized;
            //transform.localPosition = Vector3.Lerp(transform.localPosition, , 10f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, direction), 10f * Time.deltaTime);
        }
    }
}
