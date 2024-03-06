using UnityEngine;

public class PlayerToWallMaterial : MonoBehaviour
{
    [field: SerializeField]public Transform Player { get; private set; }
    [field: SerializeField]public Material Material { get; private set; }
    [field: SerializeField]public Material MaterialTwo { get; private set; }

    private void Awake()
    {
        if (Player == null || Material == null || MaterialTwo == null)
        {
            Debug.LogError("The Player To Wall Script requires you to drag some things", this);
            enabled = false;
        } 
    }

    void Update()
    {
        Material.SetVector("_PlayerPos", Player.position);
        MaterialTwo.SetVector("_PlayerPos", Player.position);
    }
}
