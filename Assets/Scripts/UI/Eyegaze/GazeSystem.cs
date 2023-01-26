using EyegazeCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GazeSystem : MonoBehaviour, IAccessUpdatable
{

    public static Camera uiCam;
    
    [SerializeField]
    private GazeTickView _view;

    [SerializeField]
    private EventSystem _eventSystem;

    private GazeBase[] _gazeObjects;

    private Gaze _state = Gaze.Idle;
    private GazeBase _target;
    private float _gazeElapsed = 0;

    private AccessVO _access;

    private void Awake()
    {
        
        uiCam = GameObject.Find("UICamera").GetComponent<Camera>();
        
        _gazeObjects = GetComponentsInChildren<GazeBase>();
        foreach(GazeBase gb in _gazeObjects)
        {
            gb.Init(_eventSystem);
            gb.ON_GAZE_ENTER.Add(OnGazeEnter);
            gb.ON_GAZE_EXIT.Add(OnGazeEnd);
        }
    }

    private void OnGazeEnter(GazeBase target)
    {
        _gazeElapsed = 0;
        _target = target;
        _state = Gaze.Active;
        _view.pie = _access.eyeGazeConfig.anim == "pie";
    }

    private void OnGazeEnd()
    {
        _gazeElapsed = 0;
        _target = null;
        _state = Gaze.Idle;
    }

    void Update()
    {
        if (_access == null) return;
        if(!_access.eyeGazeActive) return;
        
        switch (_state)
        {
            case Gaze.Idle:
                _gazeElapsed = 0;
                _view.Hide();
                break;
            case Gaze.Active:

                if (_gazeElapsed > _access.eyeGazeConfig.dwellTime)
                {
                    _state = Gaze.Complete;
                    _view.Hide();

                    _target.Invoke();
                }
                else
                {
                    
                    Vector3 targetPosition = _target.gameObject.transform.position;
                    
                    Vector3 dwellPosition = new Vector3(
                        targetPosition.x + _target.dwellOffset.x, 
                        targetPosition.y + _target.dwellOffset.y,
                        targetPosition.z
                        );
                    
                    _view.Draw(_gazeElapsed / _access.eyeGazeConfig.dwellTime, dwellPosition);
                    _gazeElapsed += Time.deltaTime;
                }
                break;
            case Gaze.Complete:
                _gazeElapsed = 0;
                _target = null;
                break;
        }
    }

    public void OnUpdate(AccessVO data)
    {
        _access = data;
    }
}
