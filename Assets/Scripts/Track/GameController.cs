using System;
using Dreamteck.Splines;
using EyegazeCore;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : AbstractScene, IAccessUpdatable
{
    private float _winElapsed = 0;

    [SerializeField]
    private FollowerController _follower;

    [FormerlySerializedAs("_uiController")] [SerializeField]
    private UIController _ui;

    [FormerlySerializedAs("_track")] [SerializeField]
    private TrackInitiator _trackBuilder;

    private TrackController _track;
    
    private FinishLineController _finishLine;
    
    private EyeDriveSession _session;
    private EyeDriveGameSettingsVO _settings;

    [FormerlySerializedAs("_preciseOrbView")]
    [BoxGroup("Input")] 
    [SerializeField]
    private PreciseOrbIndicator preciseOrbIndicator;
    
    private GameState _previousState;

    protected override void OnAwake()
    {
        _session = GetSession<EyeDriveSession>();

        _settings = GetGameSettings<EyeDriveGameSettingsVO>();

        //reset state
        _session.lapTimes = new List<float>();
        _session.bestTime = 0;
        _session.lap = 0;
        _session.lapTime = 0;

        StaticEyeDriveSession.INSTANCE = _session;
        StaticEyeDriveSettingsVO.INSTANCE = _settings;

        //could move this into a preloader
        _track = _trackBuilder.Init(_session, _settings);
        _track.ON_ORB_COLLECTED.AddListener(OnOrbCollected);

        

        state = GameState.Countdown;
    }

    private ModeType mode
    {
        get
        {
            AccessVO access = user.settings;
            
            if (access.eyeGazeActive && access.eyeGazeConfig.directMode)
                return ModeType.Precise;
            
            if (access.switchActive || (access.eyeGazeActive && !access.eyeGazeConfig.directMode))
                return ModeType.Lanes;
            
            if (input.controllerConnected)
                return ModeType.Gamepad;
            
            return ModeType.Mouse;

            
        }
    }
    
    protected override void Init()
    {
        _session.totalOrbs = _track.totalOrbs;
        
        _follower.Init(GetComponentInChildren<SplineComputer>(), input, mode);
        _follower.ON_LAP_COMPLETE.Add(OnLapComplete);

        _finishLine = GetComponentInChildren<FinishLineController>();
        _ui.Init(_session, input);
        _ui.ON_HOME(OnHome);
        _ui.ON_NEXT(OnNext);
        _ui.ON_REPLAY(OnReplay);
        _ui.ON_RESTART.AddListener(OnReplay);
        _ui.pausePanel.ON_EXIT.AddListener(OnHome);
        _ui.pausePanel.ON_RESUME.AddListener(OnResume);
        _ui.pausePanel.ON_SETTINGS.AddListener(ShowSESettings);
        _ui.pausePanel.ON_RESTART.AddListener(OnReplay);
        _ui.ON_PAUSE.AddListener(OnPause);
        _ui.ON_RESUME.AddListener(OnResume);
        _ui.REFRESH_SWITCHES.AddListener(() => {switches.Refresh();});
        _ui.ON_CLASSIC_PRECISE_TOGGLE.AddListener((isPrecise) =>
        {
            _session.user.settings.eyeGazeConfig.directMode = isPrecise;
            OnSettingsChangedByUser(_session.user.settings);
        });
        
        SECornerButtons.ON_SETTINGS.AddListener(OnPause);

        _finishLine.ON_COUNTDOWN_COMPLETE.AddListener(OnCountdownComplete);
        

        input.ON_CONTROLLER_START_BUTTON.AddListener(OnGamepadStartButton);
        input.ON_CONTROLLER_CONNECTED.AddListener((c) =>
        {
            _follower.OnModeChange(mode);
        });
    }


    private void OnGamepadStartButton()
    {
        if (state == GameState.Racing)
            OnPause();
        else if (state == GameState.Paused)
            OnResume();
    }

    private void OnReplay()
    {
        TransitionOut("Game");
    }

    private void OnNext()
    {
        if (_session.track < 2)
            _session.track++;
        else
        {
            int w = (int)_session.world;
            if (w < 2)
            {
                _session.world++;
                _session.track = 0;
            }
            else
            {
                //whats next?
            }
        }

        OnReplay();
    }

    private void OnHome()
    {
        TransitionOut("Menu");
    }

    private void OnOrbCollected()
    {
        _ui.dash.OnOrbCollected();
        
        _session.orbs++;
        if (_session.orbs >= _track.totalOrbs)
            OnOrbCollectionFinished();
    }

    private void OnLapComplete()
    {
        if(_session.lap > 0)
        {
            _session.lapTimes.Insert(0, _session.lapTime);
            
            if (_session.bestTime == 0 || _session.lapTime < _session.bestTime)
                _session.bestTime = _session.lapTime;
        }

        _session.lapTime = 0;
        _session.lap++;
        
        if (_session.gameType == GameType.TimeTrial)
        {
            if(_session.lap == 2)
            {
                SFX.PlayOneShot("Eyedrive_SFX_In Game__Lap 2__v.1");
            }else if(_session.lap == 3)
            {
                SFX.PlayOneShot("Eyedrive_SFX_In Game__Final Lap__v.1");
            }else if (_session.lap == 4)
            {
                OnRaceComplete();
            }
        }
    }

    //eeds when all have been collected
    void OnOrbCollectionFinished()
    {
        SFX.PlayOneShot("Eyedrive_SFX_In Game_Break (long)_v.1");
        state = GameState.RaceCompleteOrbs;
        

        SaveOrbScore(_session.orbs);
        
        _follower.ON_LAP_COMPLETE.RemoveAll();
    }

    void OnRaceComplete()
    {
        SFX.PlayOneShot("Eyedrive_SFX_In Game_Break (long)_v.1");
        SaveTrackScore(_session.bestTime);
        
        _follower.ON_LAP_COMPLETE.RemoveAll();

        state = GameState.RaceComplete;
    }
    

    public void SaveOrbScore(int score)
    {
        // orb score saved in LevelProgress.state
        int level = _session.track + 1;
        int zone = (int)_session.world + 1;

        // save current levels for server model
        game.zone = zone <= Constants.NUM_ZONES ? zone : Constants.NUM_ZONES;
        game.level = level <= Constants.NUM_TRACKS_PER_ZONE ? zone : Constants.NUM_TRACKS_PER_ZONE;

        string orbState = "{orbs:" + score + "}";

        api.SaveGame(game, 0, orbState);
    }

    public void SaveTrackScore(float score = 0)
    {
        Debug.Log("User Service: Save Track completed, set the next track / world");

        int level = _session.track + 1;
        int zone = (int)_session.world + 1;

        Debug.Log("current zone : " + zone);
        Debug.Log("current level : " + level);

        Debug.Log("current zoneBest : " + game.zoneBest);
        Debug.Log("current levelBest : " + game.levelBest);

        // save current levels for server model
        game.zone = zone <= Constants.NUM_ZONES ? zone : Constants.NUM_ZONES;
        game.level = level <= Constants.NUM_TRACKS_PER_ZONE ? zone : Constants.NUM_TRACKS_PER_ZONE;

        // do we need to update zonebest / levelbest
        if (
            game.zoneBest != Constants.NUM_ZONES ||
            game.levelBest != Constants.NUM_TRACKS_PER_ZONE
        )
        {
            Debug.Log("look to update");

            // do we need to up the zone best?
            if (level == Constants.NUM_TRACKS_PER_ZONE && game.zoneBest != Constants.NUM_ZONES)
            {
                int targetZoneBest = game.zoneBest + 1;

                Debug.Log("last level completed, check to up the zone best to next zone");

                // do we need to up the zone best?
                if (targetZoneBest > game.zoneBest)
                {
                    Debug.Log("upping zone best to : " + targetZoneBest);
                    game.zoneBest = targetZoneBest;
                    game.levelBest = 0;
                }
            }
            else
            {
                // we're not changing the zone, so check if new level best
                Debug.Log("sticking within zone");

                if (level > game.levelBest && game.levelBest < Constants.NUM_TRACKS_PER_ZONE - 1)
                {
                    Debug.Log("level higher than level best, upping level best");
                    game.levelBest = level;
                }
            }
        }

        Debug.Log("new zoneBest : " + game.zoneBest);
        Debug.Log("new levelBest : " + game.levelBest);
        
        api.SaveGame(game, score);
    }

    private string GetWorldMusic(World world)
    {
        string music = "Eyedrive_Music_Dorian IV_Demo Loop_v.1";

        switch (world)
        {
            case World.City:
                music = "Eyedrive_Music_La Cuna_Demo Loop_v.1";
                break;
            case World.Desert:
                music = "Eyedrive_Music_Dorian IV_Demo Loop_v.1";
                break;
            case World.Space:
                music = "Eyedrive_Music_Sol Base_Demo Loop_v.1";
                break;
        }

        return music;
    }


    private void OnCountdownComplete()
    {
        state = GameState.Racing;

        //load music
        music.FadeIn(GetWorldMusic(_session.world));
        
    }
    
    public GameState state
    {
        set
        {
            _previousState = _session.state;

            _ui.state = value;
            
            _session.state = value;
            
            if(_session.state != _previousState)
                switches.Refresh();
        }

        get => _session.state;
    }
    
    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        
        if (_session == null || _finishLine == null) return;
        
        switch (state)
        {
            case GameState.Countdown:
                _finishLine.Tick(delta, _session.lap);
                break;
            case GameState.Racing:
                _ui.Tick(delta, mode);

                //lap time
                _session.lapTime += delta;
                _follower.Tick(delta);
                _finishLine.Tick(delta, _session.lap);
                
                if(_session.gameType == GameType.Collect)
                     _track.Tick(delta, _follower.position, _follower.percent);
                else
                {
                    _track.Tick(delta);
                }
                
                preciseOrbIndicator.Tick(delta, mode);

                break;
            case GameState.Paused:
                _follower.Pause(delta);
                _track.Pause(delta);
                
                break;
            case GameState.RaceComplete:
                _follower.Pause(delta);
                _track.Pause(delta);
                break;
            case GameState.RaceCompleteOrbs:
                _follower.Pause(delta);
                _track.Pause(delta);
                break;
        }
    }

    protected override void OnBack()
    {
        if (state == GameState.Paused) OnResume();
        else TransitionOut("Menu");
    }

    void OnPause()
    {
        isPaused = true;
        state = GameState.Paused;
    }

    void OnResume()
    {
        isPaused = false;
        state = GameState.Racing;
    }

    protected override UITransition BuildTransitionIn()
    {
        return FadeFromWhiteTransition();
    }

    protected override UITransition BuildTransitionOut()
    {
        return FadeToWhiteTransition();
    }

    public void OnUpdate(AccessVO data)
    {
        _follower.OnModeChange(mode);
    }

    protected override void OnTick(float delta)
    {
       
    }
}

public enum GameState{Countdown, Racing, Paused, RaceComplete, RaceCompleteOrbs}
