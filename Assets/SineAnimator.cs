using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineAnimator : MonoBehaviour
{
    [field: SerializeField]public Vector3 Position { get; private set; }
    [field: SerializeField]public Vector3 Rotation { get; private set; }

    private Vector3 startPosition;
    private Vector3 startRotation;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.position = startPosition + (Position * Mathf.Sin(Time.time));
        transform.rotation = Quaternion.Euler(startRotation) * Quaternion.Euler(Rotation * Mathf.Sin(Time.time));
    }
}
