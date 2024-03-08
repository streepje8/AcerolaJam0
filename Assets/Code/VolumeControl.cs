using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour
{
    [field: SerializeField]public AudioMixerGroup Mixer { get; private set; }
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnVolumeUpdate);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnVolumeUpdate);
    }

    public void OnVolumeUpdate(float newValue)
    {
        Mixer.audioMixer.SetFloat("MegaVolume", newValue);
    }
}
