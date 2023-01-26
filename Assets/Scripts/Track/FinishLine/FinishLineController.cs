using EyegazeCore;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinishLineController : MonoBehaviour
{
    [SerializeField]
    private List<Texture> _countdownTextures;

    [SerializeField]
    private List<GameObject> _skins;

    [SerializeField]
    private Material _material;

    private FinishLineTrigger _trigger;

    private CountdownState _state = CountdownState.Idle;
    private float _elapsed = 0;
    private int _index = 0;


    public UnityEvent ON_COUNTDOWN_COMPLETE = new UnityEvent();

    private AudioSource _countdownSound;

    // Start is called before the first frame update
    void Awake()
    {
        state = CountdownState.Countdown;

        for(int i = 0; i < _skins.Count; i++)
        {
            _skins[i].SetActive(i == (int)StaticEyeDriveSession.INSTANCE.world);
        }

        _countdownSound = SFX.GetSource("Eyedrive_SFX_In Game_Countdown (no pause)_v.1", false);
    }
    
    public void Tick(float delta, int lap)
    {
        if (_state == CountdownState.Countdown)
        {
            _elapsed += delta;

            if (_elapsed >= 1)
            {
                _elapsed = 0;
                _index++;

                if (_index > 3)
                {
                    state = CountdownState.Complete;
                }
            }
        }


        if (_index == 1 && _countdownSound.isPlaying == false)
        {
            _countdownSound.Play();
        }
        
        UpdateTexture(_index + lap);
    }

    private void UpdateTexture(int index)
    {
        
        if(index < _countdownTextures.Count && _material.mainTexture != _countdownTextures[index])
        {
            _material.SetTexture("_EmissionMap", _countdownTextures[index]);
            _material.mainTexture = _countdownTextures[index];
        }
    }

    private CountdownState state
    {
        set
        {
            switch(value)
            {
                case CountdownState.Idle:
                    _index = 0;
                    break;
                case CountdownState.Countdown:
                    _elapsed = -1;
                    _index = 0;

                    break;
                case CountdownState.Complete:
                    ON_COUNTDOWN_COMPLETE.Invoke();
                    break;
            }

            _state = value;
        }
    }
}

enum CountdownState {Idle, Countdown, Complete}
