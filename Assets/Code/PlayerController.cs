using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Controls
{
    public KeyCode FORWARD;
    public KeyCode LEFT;
    public KeyCode RIGHT;
    public KeyCode UP;
    public KeyCode DOWN;
    public KeyCode RLEFT;
    public KeyCode RRIGHT;
}

public enum Command
{
    Trust,
    RollLeft,
    RollRight
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [field: SerializeField] public Controls Controls { get; private set; } = new Controls()
    {
        FORWARD = KeyCode.Space,
        LEFT = KeyCode.J,
        RIGHT = KeyCode.F,
        UP = KeyCode.D,
        DOWN = KeyCode.K,
        RLEFT = KeyCode.L,
        RRIGHT = KeyCode.S
    };

    [field: SerializeField] public float TurnControl { get; set; } = 0.04f;
    [field: SerializeField] public float UpDownControl { get; set; } = 0.04f;
    [field: SerializeField] public float EngineTrustAmount { get; set; } = 10f;
    [field: SerializeField] public float UpDownEngineTrustAmount { get; set; } = 10f;
    [field: SerializeField] public float RollTrustAmount { get; set; } = 10f;
    [field: SerializeField] public ParticleSystem LeftEngineParticles { get; private set; }
    [field: SerializeField] public ParticleSystem RightEngineParticles { get; private set; }
    [field: SerializeField] public Transform LeftEngine { get; private set; }
    [field: SerializeField] public Transform RightEngine { get; private set; }
    [field: SerializeField] public Transform UpEngine { get; private set; }
    [field: SerializeField] public Transform DownEngine { get; private set; }
    [field: SerializeField] public Transform RollLUpEngine { get; private set; }
    [field: SerializeField] public Transform RollRUpEngine { get; private set; }
    [field: SerializeField] public Transform RollLDownEngine { get; private set; }
    [field: SerializeField] public Transform RollRDownEngine { get; private set; }
    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Queue<Command> commands = new Queue<Command>();
    private float trustDirection = 0;
    private float upDownDirection = 0;
    void Update()
    {
        trustDirection = Mathf.Lerp(trustDirection, -(Input.GetKey(Controls.LEFT) ? 1 : 0) + (Input.GetKey(Controls.RIGHT) ? 1 : 0), 3f * Time.deltaTime);
        upDownDirection = Mathf.Lerp(upDownDirection, -(Input.GetKey(Controls.DOWN) ? 1 : 0) + (Input.GetKey(Controls.UP) ? 1 : 0), 3f * Time.deltaTime);
        if (Input.GetKey(Controls.FORWARD))
        {
            if(!commands.Contains(Command.Trust)) commands.Enqueue(Command.Trust);  
        }
        if (Input.GetKey(Controls.RLEFT))
        {
            if(!commands.Contains(Command.RollLeft)) commands.Enqueue(Command.RollLeft);  
        }
        if (Input.GetKey(Controls.RRIGHT))
        {
            if(!commands.Contains(Command.RollRight)) commands.Enqueue(Command.RollRight);  
        }
    }

    private void FixedUpdate()
    {
        while(commands.Count > 0)
        {
            switch (commands.Dequeue())
            {
                case Command.Trust:
                    float val = ((trustDirection * -TurnControl) + 1f) * 0.5f;
                    EngineTrust(LeftEngine, EngineTrustAmount * Time.fixedDeltaTime * (1 - val));
                    EngineTrust(RightEngine, EngineTrustAmount * Time.fixedDeltaTime * val);
                    
                    float upDownVal = ((upDownDirection * -UpDownControl) + 1f) * 0.5f;
                    EngineTrust(UpEngine, UpDownEngineTrustAmount * Time.fixedDeltaTime * (1 - upDownVal));
                    EngineTrust(DownEngine, UpDownEngineTrustAmount * Time.fixedDeltaTime * upDownVal);

                    var valL = (trustDirection + 1) * 0.5f;
                    var valR = (1 - valL);
                    LeftEngineParticles.Emit(Mathf.CeilToInt(6 * valL));
                    RightEngineParticles.Emit(Mathf.CeilToInt(6 * valR));
                    break;
                case Command.RollLeft:
                    EngineTrust(RollLUpEngine, RollTrustAmount * Time.fixedDeltaTime);
                    EngineTrust(RollRDownEngine, RollTrustAmount * Time.fixedDeltaTime);
                    break;
                case Command.RollRight:
                    EngineTrust(RollRUpEngine, RollTrustAmount * Time.fixedDeltaTime);
                    EngineTrust(RollLDownEngine, RollTrustAmount * Time.fixedDeltaTime);
                    break;
            }
        }
    }

    private void EngineTrust(Transform engine, float amount)
    {
        rb.AddForceAtPosition(engine.forward * (amount * 100f), engine.position);
    }
}
