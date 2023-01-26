﻿using System;
using System.Collections;
using System.Collections.Generic;
using EyegazeCore;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DashController : CanvasMonoBehaviour, IAccessUpdatable
{
    [FormerlySerializedAs("_timeDisplay")] [SerializeField]
    private LaptimeController _lapTimeView;

    [SerializeField]
    private CanvasGroup _orbDisplay;

    [SerializeField] private ClassicPreciseToggle _classicPreciseToggle;

    [SerializeField] private StandardAccessibleButton _pauseButton;

    [SerializeField] private GameObject _gamepadIndicator, _mouseIndicator;
    
    private Text _orbCounter;
    private Animator _orbAnimator;

    
    void Start()
    {
        
        _lapTimeView.gameObject.SetActive(StaticEyeDriveSession.INSTANCE.gameType == GameType.TimeTrial);
        
        _orbDisplay.alpha = 0;

        _orbCounter = _orbDisplay.GetComponentInChildren<Text>();
        _orbAnimator = _orbDisplay.GetComponent<Animator>();
        
    }

    public void OnOrbCollected()
    {
        _orbAnimator.Play("OrbCollect");
    }

    public void Tick(float delta, int lap, ModeType mode)
    {
        _orbDisplay.alpha = (StaticEyeDriveSession.INSTANCE.gameType == GameType.Collect && StaticEyeDriveSession.INSTANCE.lap > -1) ? 1 : 0;
        
        _lapTimeView.gameObject.SetActive(lap > 0);
        _lapTimeView.Tick(delta);
        
        _pauseButton.gameObject.SetActive(mode != ModeType.Gamepad);
        _classicPreciseToggle.gameObject.SetActive(mode != ModeType.Gamepad && mode != ModeType.Mouse);
        _gamepadIndicator.SetActive(mode == ModeType.Gamepad);
        _mouseIndicator.SetActive(mode == ModeType.Mouse);

        if (_orbDisplay.alpha > 0)
        {
            if(StaticEyeDriveSession.INSTANCE.totalOrbs == StaticEyeDriveSession.INSTANCE.orbs)
            {
                _orbCounter.text = "<color=#FC00FF>All Orbs Collected!</color>";
            }
            else
            {
                _orbCounter.text =  StaticEyeDriveSession.INSTANCE.orbs.ToString() + " / " + StaticEyeDriveSession.INSTANCE.totalOrbs.ToString() + "<size=30><color=#fc00ff> Orbs</color></size>";
            }
        }
    }

    public GameState state
    {
        set
        {
            _lapTimeView.state = value;

        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public BoolEvent ON_CLASSIC_PRECISE_TOGGLE => _classicPreciseToggle.ON_TOGGLE;

    public void OnUpdate(AccessVO data)
    {
        bool isActive = data.eyeGazeActive;
        _classicPreciseToggle.interactable = isActive;
        if (isActive)
        {
            _classicPreciseToggle.OnUpdate(data);
        }
    }

    public bool controllerConnected
    {
        set
        {
           
        }
    }
}
