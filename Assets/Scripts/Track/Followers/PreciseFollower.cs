using Dreamteck.Splines;
using Services.Constants;
using UnityEngine;

public class PreciseFollower : BaseFollower
{
    public float ACCELERATION = 10f;
    public float DEACCELERATION = 10f;
    public float SPEED = 100f;

    public float TURN_SPEED = 1f;
    public float MAX_TURN = 40f;
    public float MAX_ROLL = 20f;
    public float ROLL_SPEED = 6;
    public float TRACK_BOUNDS = 9;
    public float BOUNDS_FORCE = 30;
    public float WALL_FRICTION = .91f;


    public float SLERP_TIME = 2.5f;


    private float _turn = 0;
    private float _roll = 0;

    private SplineResult _cResult;

    private Vector3 _gaze = Vector3.zero;
    private EyeGazePreciseInput _eyeGaze;


    public AnimationCurve speedGain;
    public AnimationCurve speedLoss;
    public float gravityForce = 1f;

    public Signal<float> ON_BUMP = new Signal<float>();

    //input


    protected override void OnInit(double percent)
    {
        UpdateResult();
        UpdateMotion(0);
    }

    public override void OnAwake()
    {
        _eyeGaze = Util.GetOrAddComponent<EyeGazePreciseInput>(this.gameObject);
        _eyeGaze.ON_POSITION_UPDATE.Add(OnGazePositionUpdated);
        
        //start position
        SetPercent(Project(GetTransform().position).percent);
    }
    
    private void OnGazePositionUpdated(Vector3 gaze)
    {
        _gaze = gaze;
    }
    
    private new void Start()
    {
        base.Start();
        UpdateInput(0);
    }


    private void UpdateBounds(SplineResult r)
    {
        Vector3 dist = r.position - _body.position;
        float mag = dist.magnitude;
        Vector3 dir = dist / mag;

        if(mag > TRACK_BOUNDS*.9f)
        {
            _body.AddForce(BOUNDS_FORCE * mag * dir , ForceMode.Force);
            _speed *= 1 - WALL_FRICTION;

            ON_BUMP.Dispatch(mag);
        }
    }

    private void UpdateInput(float delta)
    {
        _eyeGaze.Tick(delta);
        float sensitivity = StaticEyeDriveSettingsVO.INSTANCE.STEERING_SENSITIVITY;

        if (gaze != Vector3.zero)
        {
            if (gaze.y > -.3f)
            {
                _speed = Mathf.Lerp(_speed, 1, delta * ACCELERATION * (gaze.y+.3f));
                braking = false;
            }
            else
            {
                braking = true;
                _speed = Mathf.Lerp(_speed, 0, delta * DEACCELERATION);
            }
        }
        else
        {
            _speed = 0;
        }
        
        _turn = Mathf.Lerp(_turn, gaze.x, delta*(TURN_SPEED*sensitivity));
        _roll = Mathf.Lerp(_roll, gaze.x, delta * (ROLL_SPEED * sensitivity));

        //intention
        if (gaze.x > .1f) intention = Intention.Right;
        else if (gaze.x < -.1f) intention = Intention.Left;
        else intention = Intention.Middle;
    }
    

    void UpdateMotion(float delta)
    {
        if (_cResult != null)
        {
           if(!_inJunction) UpdateBounds(_cResult);

            Transform bTran = _body.transform;
            
            Quaternion rot = Quaternion.Slerp(bTran.rotation, Quaternion.Euler(bTran.up * (_turn * MAX_TURN) + (-bTran.forward * (_roll * MAX_ROLL))) * _cResult.rotation, delta * SLERP_TIME);

            float gradient = UpdateGradient(delta);
            float g = gradient > 0 ? gradient : 0;

            _body.AddForce((_speed + (gradient)) * (SPEED * StaticEyeDriveSettingsVO.INSTANCE.SPEED) * bTran.forward, ForceMode.Acceleration);
            _body.MoveRotation(rot);
        }
    }
    public void UpdateResult(double overridePerc = 0)
    {

        if (overridePerc > 0)
        {
            _lastpercent = overridePerc;
        }

        float clamped = Mathf.Repeat((float)_lastpercent + 0.005f, 1);
        double clampedD = System.Convert.ToDouble(clamped);

        //Loop
        if (clampedD < _lastpercent)
        {
            _lastpercent = 0;

            //  clamped = Mathf.Repeat((float)_lastpercent + 0.002f, 1);
            // // Debug.Log("clamped: " + clamped);
            //  clampedD = System.Convert.ToDouble(clamped);
        }


        double percent = address.Project(_body.position, 4, _lastpercent, 1);
        // Debug.Log("percent: " + _lastpercent + " / " + clampedD + " / " + percent);
        SetPercent(percent);

        _lastpercent = percent;
        _cResult = address.Evaluate(percent);
    }

    float UpdateGradient(float delta)
    {
        float dot = Vector3.Dot(transform.forward, Vector3.down);
        // float dotPercent = Mathf.Lerp(-60 / 90f, 60 / 90f, (dot + 1f) / 2f);
        float dotPercent = Mathf.Lerp(-0.666f, 0.666f, (dot + 1f) / 2f);

        
        if (dotPercent > 0f)
        {
            return gravityForce * dotPercent * speedGain.Evaluate(_speed) * delta;
        }
        else
        {
            return gravityForce * dotPercent * speedLoss.Evaluate(1f - _speed) * delta;
        }
        
    }

    protected override void OnPauseUpdate(float delta)
    {
        _eyeGaze.Pause(delta);
    }

    protected override void OnUpdate(float delta)
    {
        UpdateInput(delta);
        UpdateMotion(delta);
        UpdateResult();
    }

    private Vector3 gaze
    {
        get
        {
            return _gaze;
        }
    }
    
}
