using EyegazeCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSFX : MonoBehaviour
{
    private BaseFollower _follower;

    private AudioSource _engineSound;
    private AudioSource _accelerateSound;
    private AudioSource _breakSound;

    [SerializeField]
    private string engineSoundKey;

    private float _fadeVolume = 0;

    private bool _lastThrusterState = false;
    private bool _lastBrakeState = false;

    void Start()
    {
        _follower = GetComponent<BaseFollower>();

        //init sounds
        _engineSound = SFX.GetSource(engineSoundKey, true);
        _engineSound.Play();
        _engineSound.loop = true;

        _accelerateSound = SFX.GetSource("Eyedrive_SFX_In Game_Boost_Accelerate_v.1", false);
        _accelerateSound.loop = false;

        _breakSound = SFX.GetSource("Eyedrive_SFX_In Game_Break (long)_v.1", false);
        _breakSound.loop = false;

        _fadeVolume = 0;

    }

    void Update()
    {

    }
}
