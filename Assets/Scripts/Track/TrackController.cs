using Dreamteck.Splines;
using EyegazeCore;
using UnityEngine;
using UnityEngine.Events;

public class TrackController : MonoBehaviour
{
    [SerializeField]
    private SplineComputer _mainComputer;

    private bool init = false;
    private float inittime = .1f;


    private SplineMesh[] splinemeshes = null;
    private FinishLineController _finishLine;
    private AIController _ai;
    private OrbController _orbs;
    
    public TrackController Init(EyeDriveGameSettingsVO settings, EyeDriveSession session)
    {
        splinemeshes = GetComponentsInChildren<SplineMesh>();
        SplineComputer[] computers = GetComponentsInChildren<SplineComputer>();
        
        if(settings.AI_CARS)
        {
            foreach (SplineComputer c in computers)
            {
                if (_ai == null)
                {
                    var ai = c.GetComponentInChildren<AIController>();

                    if (ai != null)
                    {
                        _ai = ai;
                        _ai.Init(c);
                    }
                   
                }
                
            }
        }

        _finishLine = GetComponentInChildren<FinishLineController>();
        if (_finishLine == null) Debug.LogError("No Finishline has been found for this track!");

     
        if(session.gameType == GameType.Collect)
            InitOrbs(session);

        return this;
        
    }

    public void InitOrbs(EyeDriveSession session)
    {
        SplineComputer[] computers = GetComponentsInChildren<SplineComputer>();
            
        foreach (SplineComputer c in computers)
            orbs.GenerateOrbs(c);
    }
    

    public void Tick(float delta)
    {
        inittime -= delta;
        if (inittime < 0 && init == false)
        {
            init = true;
            foreach (SplineMesh sm in splinemeshes) sm.Rebuild(true);
        }
        
        if(_ai != null)
            _ai.Tick(delta);
    }

    public void Tick(float delta, Vector3 playerPosition, double percent)
    {
        Tick(delta);
        orbs.Tick(playerPosition, percent);
    }

    public void Pause(float delta)
    {
        if(_ai != null)
            _ai.Pause(delta);
    }

    public SplineComputer computer
    {
        get
        {
            return _mainComputer;
        }
    }

    public FinishLineController finishLine
    {
        get
        {
            return _finishLine;
        }
    }

    private OrbController orbs
    {
        get
        {
            if (_orbs == null)
                _orbs = GetComponentInChildren<OrbController>();

            return _orbs;
        }
    }
    
    public UnityEvent ON_ORB_COLLECTED => orbs.ON_ORB_COLLECTED;
    public int totalOrbs => _orbs.totalOrbs;

}
