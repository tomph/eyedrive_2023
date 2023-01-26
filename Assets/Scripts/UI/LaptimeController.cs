using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaptimeController : CanvasMonoBehaviour
{

    [SerializeField]
    private Text _lapTimeText;

    [SerializeField]
    private Text _bestTimeText;

    private float _lastBest = -1;
    private GameState _state;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public GameState state
    {
        set
        {
            _state = value;
            if(value == GameState.Racing)
                Hide();
            else
                Show();
        }
    }
    
    public void Tick(float delta)
    {
        _lapTimeText.text = _state != GameState.RaceComplete ? Util.FormatTimeMMSSMSMS(StaticEyeDriveSession.INSTANCE.lapTime) : "Race Over!";

        float b = StaticEyeDriveSession.INSTANCE.bestTime;

        if(_lastBest != b) _bestTimeText.text = b == 0 ? "" : Util.FormatTimeMMSSMSMS(b);
        _lastBest = StaticEyeDriveSession.INSTANCE.bestTime;
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
