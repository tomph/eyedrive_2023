using System;
using System.Collections.Generic;
using EyegazeCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IAccessUpdatable
{
    [SerializeField]
    private PausePanelController _pausePanel;

    [SerializeField]
    private LaneControlSystem _lanesInputPanel;

    [SerializeField]
    private Button _quitButton;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private Button _replayButton;

    [SerializeField]
    private DashController _dash;

    private bool _dashActive = false;

    [SerializeField]
    private RaceCompletePanelController _raceCompletePanel;
    
    public UnityEvent ON_PAUSE = new UnityEvent();
    public UnityEvent REFRESH_SWITCHES = new UnityEvent();

    private EyeDriveSession _session;
    private GameState _state;

    private InputManager input;

    private void Start()
    {
        _pauseButton.onClick.AddListener(OnPauseClicked);
    }

    public void Init(EyeDriveSession session, InputManager input)
    {
        _session = session;
    }

    public void Tick(float delta, ModeType mode)
    {
        _dash.Tick(delta, _session.lap, mode); 
    }

    public GameState state
    {
        set
        {
            _pausePanel.Hide();
            _raceCompletePanel.Hide();
            _dash.Hide();
            _dash.state = value;

            _state = value;
   
            switch(value)
            {
                case GameState.Countdown:
                    break;
                case GameState.Racing:
                    _dash.Show();
                    break;
                case GameState.Paused:
                    _pausePanel.Show();
                    break;
                case GameState.RaceComplete:
                    _raceCompletePanel.Show(_session.lapTimes);
                    _dash.Hide();
                    break;
                case GameState.RaceCompleteOrbs:
                    _raceCompletePanel.Show(_session.orbs, _session.totalOrbs);
                    _dash.Hide();
                    break;
        
            }
        }
    }

    private void OnPauseClicked()
    {
        ON_PAUSE.Invoke();
    }
    
    public void ON_REPLAY(Action a)
    {
        _raceCompletePanel.ON_REPLAY.AddOnce(a);
    }

    public Button.ButtonClickedEvent ON_RESTART => _replayButton.onClick;

    public void ON_NEXT(Action a)
    {
        _raceCompletePanel.ON_NEXT.AddOnce(a);
    }

    public void ON_HOME(Action a)
    {
        _raceCompletePanel.ON_HOME.AddOnce(a);
    }
    

    public PausePanelController pausePanel => _pausePanel;
    public DashController dash => _dash;

    public UnityEvent ON_RESUME => _pausePanel.ON_RESUME;

    public BoolEvent ON_CLASSIC_PRECISE_TOGGLE => _dash.ON_CLASSIC_PRECISE_TOGGLE;

    public void OnUpdate(AccessVO data)
    {
        bool lanesOn = (data.switchActive || data.eyeGazeActive && !data.eyeGazeConfig.directMode);

        if (lanesOn) _lanesInputPanel.Show();
        else _lanesInputPanel.Hide();
    }
    
}
