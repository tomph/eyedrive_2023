using EyegazeCore;

using Services.Constants;
using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MenuController : AbstractScene
{
    private Animator _uiAnimator;

    [SerializeField] private MenuAccessibles _accessibles;

    [BoxGroup("MainMenu")]
    [SerializeField] private StandardAccessibleButton _startButton, _optionsButton;
    
    [BoxGroup("Mode Select")]
    [SerializeField] private StandardAccessibleButton _tourButton, _quickButton;

    [BoxGroup("Mode Select")]
    [SerializeField] private StandardAccessibleButton _orbCollectButton,_timedPracticeButton;

    
    [SerializeField] private Button _tourTrackButton;
    

    [SerializeField] private Button _openWorldButton;
    

    [SerializeField] private SelectionController _trackSelector;

    [SerializeField] private SelectionController _shipSelector;

    [SerializeField] private WorldSelectionController _worldSelector;

    [SerializeField] private ModeSelectionController _modeSelector;
    

    [SerializeField] private MenuOptionsController _settingsController;
   
    [SerializeField] private MenuShipDisplay _shipDisplay;

    [SerializeField] private NextTrackDisplay _nextTrackDisplay;

    [SerializeField] private Text _bestLapTime;

    [SerializeField] private UserNameText _userText;

    private MenuState _state;

    public static MenuState INIT_STATE = MenuState.Title;

    private EyeDriveSession _session;
    private EyeDriveGameSettingsVO _settings;

    [SerializeField]
    private List<EyeDriveWorld> _worlds;

    protected override void OnAwake()
    {
        _session = GetSession<EyeDriveSession>();
        StaticEyeDriveSession.INSTANCE = _session;
        _session.Reset();

        _settings = GetGameSettings<EyeDriveGameSettingsVO>();

        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // _testProgressButton.gameObject.SetActive(false);
  

        _uiAnimator = GetComponent<Animator>();

        _startButton.ON_ANIMATED_ACTION.AddListener(OnStart);
        _optionsButton.ON_ANIMATED_ACTION.AddListener(OnOptions);

        _tourButton.ON_ANIMATED_ACTION.AddListener(OnTournamentSelected);
        _quickButton.ON_ANIMATED_ACTION.AddListener(OnQuickSelected);
        
        _tourTrackButton.onClick.AddListener(OnTournamentSelectTrackSelected);


        _timedPracticeButton.ON_ANIMATED_ACTION.AddListener(OnTimePracticeSelected);
        _orbCollectButton.ON_ANIMATED_ACTION.AddListener(OnOrbCollectSelected);
        _openWorldButton.onClick.AddListener(OnOpenWorldSelected);

        _worldSelector.ON_NEXT.Add(OnWorldToTrack);
        _worldSelector.ON_SELECT_UPDATE.Add(OnWorldSelected);

        _trackSelector.ON_NEXT.Add(OnTrackToShip);
        _trackSelector.ON_SELECT_UPDATE.Add(OnTrackSelected);

        _shipSelector.ON_NEXT.Add(GoToGame);
        _shipSelector.ON_SELECT_UPDATE.Add(OnShipSelected);

        _modeSelector.ON_NEXT.Add(OnModeToGame);
        _modeSelector.ON_SELECT_UPDATE.Add(OnModeSelected);

        _nextTrackDisplay.ON_NEXT.Add(OnNextTrackSelected);


        _state = MenuState.Title;

        state = INIT_STATE;

     //   _uiAnimator.speed = 0;

        music.FadeIn("Eyedrive_Music_Title_Menu_v.1");
        _shipSelector.Init(Resources.LoadAll<Sprite>("Ships/Thumbs").ToList<Sprite>());

        Debug.Log("INIT_STATE:::: " + INIT_STATE);

        if (INIT_STATE != MenuState.Title)
        {
            Loaded();
        }
    }

    protected override void Init()
    {
        _nextTrackDisplay.Init(_worlds[(int)_session.world]);
        _userText.Init(game.username);

        _settingsController.Init(_settings, _session);
        _settingsController.ON_AI_CARS_TOGGLE.AddListener((v) => { _settings.AI_CARS = v;});
        _settingsController.ON_CRASH_SFX_TOGGLE.AddListener((v) => { _settings.CRASH_SFX = v;});
        _settingsController.ON_SLOW_STEER_TOGGLE.AddListener((v) => { _settings.STEERING_SENSITIVITY = v == true ? GameDefaults.STEERING_SENSITIVITY_SLOW : GameDefaults.STEERING_SENSITIVITY_NORMAL;});
        _settingsController.ON_SLOW_SPEED_TOGGLE.AddListener((v) => { _settings.SPEED = v == true ? GameDefaults.SPEED_SLOW : GameDefaults.SPEED_NORMAL;});

        _modeSelector.Init(_session);
    }



    public void Loaded()
    {
        _uiAnimator.speed = 1;

//        if (UserService.Instance.gameModel.settings.MUSIC_ON) Music.Play();

        
    }

    protected override void OnBack()
    {
        if (_state == MenuState.Settings)
        {
            game.settings = JsonUtility.ToJson(_settings);
            api.SaveSettings(game);

            state = MenuState.Title;
        }
        else if (_state == MenuState.GameMode)
        {
            state = MenuState.Title;
        }
        else if (_state == MenuState.QuickMenu)
        {
            state = MenuState.GameMode;
        }
        else if (_state == MenuState.World)
        {
            if (_session.playType == PlayType.Tour) state = MenuState.NextTrack;
            else state = MenuState.QuickMenu;
        }
        else if (_state == MenuState.Ship)
        {
            if (_session.playType == PlayType.Tour) state = MenuState.NextTrack;
            else state = MenuState.Track;
        }
        else
        {
            float s = (float) _state;
            s = Mathf.Clamp(s - 1, 0, Enum.GetNames(typeof(MenuState)).Length);

            state = (MenuState) s;

            
        }
    }


    private void OnTournamentSelected()
    {
        _session.gameType = GameType.TimeTrial;
        _session.playType = PlayType.Tour;

        state = MenuState.NextTrack;
    }

    private void OnTournamentSelectTrackSelected()
    {
        state = MenuState.World;
    }

    private void OnTimePracticeSelected()
    {
        _session.gameType = GameType.TimeTrial;
        _session.playType = PlayType.Practice;
        state = MenuState.World;
    }

    private void OnOrbCollectSelected()
    {
        _session.gameType = GameType.Collect;
        _session.playType = PlayType.OrbChase;
        state = MenuState.World;
    }

    private void OnOpenWorldSelected()
    {
        _session.playType = PlayType.Open;
        state = MenuState.World;
    }

    private void OnNextTrackSelected()
    {
        state = MenuState.Ship;
    }

    private void OnQuickSelected()
    {
        // UserService.Instance.gameModel.settings.PLAYTYPE = PlayType.Quick;
        state = MenuState.QuickMenu;
    }

    private void OnModeSelected(int obj)
    {
        _session.gameType = (GameType) obj;
    }

    private void OnWorldSelected(int obj)
    {
        _session.world = (World) obj;

        // use this for Leaderboards later
        // string levelProgress = UserService.GetLevelProgress(UserService.Instance.gameModel.settings.TRACK, (int)UserService.Instance.gameModel.settings.WORLD);
    }

    private void OnShipSelected(int obj)
    {
        _shipDisplay.SetShip(obj);
        _session.ship = obj;
    }

    private void OnTrackSelected(int obj)
    {
        _session.track = obj;

        _bestLapTime.text = BestLapTime(
            _session.world,
            _session.track
        );
    }

    public string BestLapTime(World world, int track)
    {
        LevelProgressVO prog = api.GetProgress(game, track + 1, (int)world + 1);

        if (prog == null) return "";
        return prog.highScore == 0 ? "" : "BEST LAP - " + Util.FormatTimeMMSSMSMS(prog.highScore);
    }

    private void OnModeToGame()
    {
        GoToGame();
    }

    private void OnTrackToShip()
    {
        state = MenuState.Ship;
    }

    private void OnWorldToTrack()
    {
        if (_session.playType == PlayType.Open)
        {
            //go to game scene
            GoToGame();
            return;
        }

        state = MenuState.Track;
    }

    private void OnShipToMode()
    {
        Debug.Log(_session.track);

        // Open World don't need to select mode, go straight to scene.
        if (_session.track == 3)
        {
            GoToGame();
            return;
        }

        state = MenuState.Mode;
    }

    private void GoToGame()
    {
        // set the state for returning to menu from game
        if (_session.playType == PlayType.Tour) INIT_STATE = MenuState.NextTrack;
        else INIT_STATE = MenuState.QuickMenu;

        // go to game
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private void OnShipSelected()
    {
        _uiAnimator.Play("ShipToLevel");
    }

    private void OnLevelToShip()
    {
        _uiAnimator.Play("LevelToShip");
    }

    private void OnShipToMenu()
    {
        _uiAnimator.Play("ShipToMenu");
    }

    private void OnOptionsBack()
    {
        game.settings = JsonUtility.ToJson(_settings);
        api.SaveSettings(game);

        _uiAnimator.Play("TitleIn");
    }

    private void OnOptions()
    {
        state = MenuState.Settings;
    }

    private void OnStart()
    {
        state = MenuState.GameMode;
    }

    public MenuState state
    {
        set
        {
            _accessibles.state = value;
            exitBackMode = value == MenuState.Title ? ExitBackMode.Exit : ExitBackMode.Back;


            Debug.Log("Setting State: " + value);

            _shipDisplay.gameObject.SetActive(false);

            switch (value)
            {
                case MenuState.Title:
                {
                    _uiAnimator.Play("TitleIn");

                        Debug.Log("TitleIn!");
                }
                    break;
                case MenuState.Settings:
                {
                    _settingsController.Init(_settings, _session);
                    _uiAnimator.Play("TitleToOptions");
                }
                    break;
                case MenuState.GameMode:
                {
                    if (_state == MenuState.Title) _uiAnimator.Play("TitleToGameMode");
                    else if (_state == MenuState.QuickMenu) _uiAnimator.Play("QuickMenuToGameMode");
                    else _uiAnimator.Play("GameModeIn");
                }
                    break;
                case MenuState.NextTrack:
                {
                    NextTrackSetter.SetNextTrack(_session, game);

                    Debug.Log("set settings next track" + _session.world + " " +
                              _session.track);

                    _uiAnimator.Play("NextTrackIn");
                    // if (_state == MenuState.Title) _uiAnimator.Play("TitleToGameMode");
                    // else if (_state == MenuState.QuickMenu) _uiAnimator.Play("QuickMenuToGameMode");
                    // else _uiAnimator.Play("GameModeIn");
                }
                    break;
                case MenuState.World:
                {
                    if (_state == MenuState.Title) _uiAnimator.Play("TitleToWorld");
                    else if (_state == MenuState.GameMode) _uiAnimator.Play("GameModeToWorld");
                    else _uiAnimator.Play("WorldIn");

                    _worldSelector.Unlock(game.zoneBest);
                }
                    break;
                case MenuState.QuickMenu:
                {
                    _uiAnimator.Play("GameModeToQuickMenu");
                    // if (_state == MenuState.Title) _uiAnimator.Play("TitleToWorld");
                    // else if (_state == MenuState.GameMode) _uiAnimator.Play("GameModeToWorld");
                    // else _uiAnimator.Play("WorldIn");
                }
                    break;
                case MenuState.Track:
                {
                    if (!_trackSelector.initiated || _state == MenuState.World)
                    {
                        _trackSelector.Init(_worlds[(int)_session.world].thumbs);
                    }

                    if (_state == MenuState.World)
                    {
                        _uiAnimator.Play("WorldToTrack");
                    }
                    else
                    {
                        _uiAnimator.Play("TrackIn");
                    }


                    int track = BestLevelForZone((int) _session.world);

                    _trackSelector.Unlock(track);
                    _trackSelector.AnimateIn(0);
                }
                    break;

                case MenuState.Ship:
                    // if (_state == MenuState.Track)
                    // {
                    _uiAnimator.Play("TrackToShip");
                    // }

                    _shipDisplay.transform.localScale = Vector3.zero;
                    _shipDisplay.transform.DOScale(Vector3.one, .75f).SetEase(Ease.InOutBack).Play().SetDelay(.5f).OnStart(() => { _shipDisplay.gameObject.SetActive(true); });


                    // unlock all ships
                    _shipSelector.Unlock(3);
                    _shipSelector.AnimateIn(_session.ship);
                    _shipDisplay.SetShip(_session.ship);

                    break;

                case MenuState.Mode:
                    if (_state == MenuState.Ship)
                    {
                        _uiAnimator.Play("ShipToMode");
                    }

                    break;
            }



           switches.Refresh();

            _state = value;
        }
    }

    public int BestLevelForZone(int zone)
    {
        Debug.Log("best level for zone" + zone);
        // return level 1-3 depending on progress, eg is zoneBest is higher that zone specified, all levels are open
        int levelOpen = (
            (int)game.zoneBest > zone + 1)
            ? Constants.NUM_TRACKS_PER_ZONE
            : game.levelBest + 1;

        Debug.Log(levelOpen);

        // if (levelOpen == 0) levelOpen = 1;

        return levelOpen;
    }

    public void OnBlueBoxAppear()
    {
        SFX.PlayOneShot("Eyedrive_SFX_Menu_Blue Boxes Appear_v.1");
    }

    public void OnLevelsAppear()
    {
        SFX.PlayOneShot("Eyedrive_SFX_Menu_Options_Levels Appear_v.1");
    }

    protected override UITransition BuildTransitionIn()
    {
        return FadeFromWhiteTransition();
    }

    protected override UITransition BuildTransitionOut()
    {
        return FadeToWhiteTransition();
    }
}