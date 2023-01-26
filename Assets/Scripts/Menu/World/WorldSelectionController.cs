using EyegazeCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectionController : MenuPanel
{
    [SerializeField]
    private StandardAccessibleButton _space, _desert, _city, _go;

    [SerializeField]
    private Image _desertLock, _cityLock;

    private Dictionary<World, Animator> _animators = new Dictionary<World, Animator>();

    private WorldButton[] _buttons;


    public void Start()
    {
        _space.ON_IMMEDIATE_ACTION.AddListener(() => OnWorldSelected(World.Space));
        _desert.ON_IMMEDIATE_ACTION.AddListener(() => OnWorldSelected(World.Desert));
        _city.ON_IMMEDIATE_ACTION.AddListener(() => OnWorldSelected(World.City));

        _animators.Add(World.Space, _space.GetComponent<Animator>());
        _animators.Add(World.Desert, _desert.GetComponent<Animator>());
        _animators.Add(World.City, _city.GetComponent<Animator>());

        _go.ON_ANIMATED_ACTION.AddListener(Next);

        OnWorldSelected(StaticEyeDriveSession.INSTANCE.world);
    }

    private void OnWorldSelected(World obj)
    {
        foreach (World w in _animators.Keys)
            _animators[w].Play(obj == w ? "Selected" : "Unselected");

        StaticEyeDriveSession.INSTANCE.world = obj;

    }

    public void Unlock(int zoneBest)
    {
        _space.interactable = true;
        _desert.interactable = zoneBest > 0;
        _city.interactable = zoneBest > 1;

        _desertLock.enabled = !_desert.interactable;
        _cityLock.enabled = !_city.interactable;

        OnWorldSelected(StaticEyeDriveSession.INSTANCE.world);
    }

}
