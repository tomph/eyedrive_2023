using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EyegazeCore;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MenuPanel
{
    private LoopedImageFader _fader;

    public bool initiated = false;

    [SerializeField]
    private StandardAccessibleButton _selectButton;

    void Awake()
    {
        _fader = GetComponentInChildren<LoopedImageFader>();
        _fader.ON_SELECT_UPDATE.Add(OnSelection);
        _selectButton.ON_ANIMATED_ACTION.AddListener(Next);
    }

    private void OnSelection(int obj)
    {
        ON_SELECT_UPDATE.Dispatch(obj);
        
        _selectButton.gameObject.SetActive(_fader.isLocked(obj));
    }

    public void AnimateIn()
    {
        _fader.AnimateIn();
    }

    public void AnimateIn(int index)
    {
        _fader.AnimateIn(index);
    }

    public void Init(List<Sprite> sprites)
    {
        _fader.Init(sprites);

        ON_SELECT_UPDATE.Dispatch(0);

        initiated = true;
    }

    public void Unlock(int unlock)
    {
        for (int i = 0; i < unlock; i++)
        {
            _fader.Unlock(i);
        }
    }
}
