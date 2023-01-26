using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EyegazeCore;
using Services.Constants;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowerController : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;
    
    [SerializeField] private List<Ship> _ships;
    private Ship _ship;
    
    private SplineComputer _computer;
    private InputManager _input;
    
    public Signal ON_LAP_COMPLETE = new Signal();

    private ModeType _mode;

    private AccessVO _access;

    public void Init(SplineComputer computer, InputManager input, ModeType mode)
    {
        _computer = computer;
        _input = input;
        
        _ships = GetComponentsInChildren<Ship>(true).ToList();
        _ship = SetShip(StaticEyeDriveSession.INSTANCE.ship);
        
        _camera.gameObject.SetActive(false);
        _camera.LookAt = _ship.gameObject.transform;
        _camera.Follow = _ship.gameObject.transform;
        _camera.transform.position = _ship.transform.position;
        _camera.gameObject.SetActive(true);
        
        //just in case a controller was connected
        OnModeChange(mode);
    }

    public void OnModeChange(ModeType mode)
    {
        _mode = mode;
        
        if(_ship != null)
            _ship.OnModeChange(mode);
    }
    
    public void Tick(float delta)
    {
        _ship.Tick(delta, _mode);
    }

    public void Pause(float delta)
    {
        _ship.Pause(delta, _mode);
    }

    private Ship SetShip(int ship)
    {
        for (int i = 0; i < _ships.Count; i++)
        {
            _ships[i].gameObject.SetActive(i == ship);
        }

        Ship s = _ships[ship];
        
        SplineResult startPosition = _computer.Evaluate(Constants.SPLINE_START_PERCENT);
        s.transform.position = startPosition.position + (s.transform.up * 2);
        
        s.ON_LAP_COMPLETE.AddListener(OnLapComplete);
        s.Initialise(_computer, _input, Constants.SPLINE_START_PERCENT);
        
        return _ships[ship];
    }

    private void OnLapComplete()
    {
        ON_LAP_COMPLETE.Dispatch();
    }
    

    public Vector3 position
    {
        get { return _ship.transform.position; /*_activeFollower.transform.position;*/ }
    }

    public double percent
    {
        get
        {
            /*return _activeFollower.projectedPosition;*/
            return _ship.GetPercent(_mode);
        }
    }
    
}

