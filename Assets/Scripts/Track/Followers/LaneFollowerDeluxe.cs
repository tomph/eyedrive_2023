
using Dreamteck.Splines;

using UnityEngine;

public class LaneFollowerDeluxe : BaseFollower
{
    public float ACCELERATION = 10f;
    public float DEACCELERATION = 10f;

    public float SPEED = 100f;

    public float MAX_ROLL = 20f;
    public float ROLL_SPEED = 6;
    public float TRACK_BOUNDS = 9;
    public float BOUNDS_FORCE = 30;

    public float SLERP_TIME = 2.5f;

    public float LANE_SIZE = 12;

    private float _turn = 0;
    private float _roll = 0;

    private int _lane = 0;
    private int _intendedLane = 0;
    public AnimationCurve speedGain;
    public AnimationCurve speedLoss;
    public float gravityForce = 1f;

    private SplineResult _cResult;
    
    //input
    private Vector3 _gaze = Vector3.zero;
    private EyeGazePreciseInput _eyeGaze;

    [SerializeField]
    private LaneControlSystem _laneControlInput;

    protected override void OnInit(double percent)
    {
        UpdateResult();
    }

    protected override void OnPauseUpdate(float delta)
    {
        _eyeGaze.Pause(delta);
    }

    protected override void OnUpdate(float delta)
    {
        _eyeGaze.Tick(delta);
        UpdateResult();
        UpdateMotion(delta);
        UpdateGaze(delta);
    }

    public override void OnAwake()
    {
        ON_JUNCTION.Add(OnJunction);
        
        _body = GetComponent<Rigidbody>();

        _laneControlInput.ON_CHOOSE_DIR.Add(OnChooseDir);
        
        //used exclusively for speed (y value)
        _eyeGaze = Util.GetOrAddComponent<EyeGazePreciseInput>(this.gameObject);
        _eyeGaze.ON_POSITION_UPDATE.Add(OnGazePositionUpdated);
    }

    private void OnGazePositionUpdated(Vector3 g)
    {
        _gaze = g;
    }
    
    private void OnChooseDir(int direction)
    {
        _lane = Mathf.Clamp(_lane + direction, -1, 1);
        
        if (_lane == 0) intention = Intention.Middle;
        else if (_lane == 1) intention = Intention.Right;
        else if (_lane == -1) intention = Intention.Left;
        
    }

    private void UpdateBounds(SplineResult r)
    {
        Vector3 dist = r.position - _body.position;
        float mag = dist.magnitude;
        Vector3 direction = dist / mag;

        if (mag > TRACK_BOUNDS * .9f)
        {
            _body.AddForce(direction * mag * BOUNDS_FORCE, ForceMode.Force);
        }
    }
    
    public void UpdateResult(double overridePerc = 0)
    {

        if (overridePerc > 0)
        {
            _lastpercent = overridePerc;
        }

        float clamped = Mathf.Repeat((float)_lastpercent + 0.002f, 1);
        double clampedD = System.Convert.ToDouble(clamped);
        
        //Loop
        if (clampedD < _lastpercent)
        {
            _lastpercent = 0;

            clamped = Mathf.Repeat((float)_lastpercent + 0.002f, 1);
            clampedD = System.Convert.ToDouble(clamped);
        }

        double percent = address.Project(_body.position, 4, _lastpercent, clampedD);

        SetPercent(percent);

        _lastpercent = percent;
        _cResult = address.Evaluate(percent);
    }
    
    public void OnJunction()
    {
        _lane = 0;
    }

    private void UpdateGaze(float delta)
    {
        if (_gaze.y > -.3f)
        {
            _speed = Mathf.Lerp(_speed, 1, delta * ACCELERATION * (_gaze.y+.3f));
            braking = false;
        }
        else
        {
            braking = true;
            _speed = Mathf.Lerp(_speed, 0, delta * DEACCELERATION);
        }
        
        _roll = Mathf.Lerp(_roll, _gaze.x < 0 ? -1 : 1, delta * ROLL_SPEED);
    }

    void UpdateMotion(float delta)
    {
        if (_cResult != null)
        {
            UpdateBounds(_cResult);

            Quaternion rot = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler((-_body.transform.forward * (_roll * MAX_ROLL))) * _cResult.rotation, delta * SLERP_TIME);

            Vector3 anchor = _cResult.position + _cResult.right * (_lane * LANE_SIZE);
            Vector3 dest = anchor - _body.position;

            float gradient = UpdateGradient();
            float g = gradient > 0 ? gradient : 0;

            Vector3 fwd = _body.transform.forward * (_speed + (gradient)) * (SPEED * StaticEyeDriveSettingsVO.INSTANCE.SPEED);
            
            float lerpSpeed = isOnGround ? 50 : 20;
            _body.AddForce(dest * lerpSpeed + fwd, ForceMode.Acceleration);
            _body.MoveRotation(rot);
            _body.drag = isOnGround ? 10 : 7;
        }
    }

    float UpdateGradient()
    {
        float dot = Vector3.Dot(this.transform.forward, Vector3.down);
        float dotPercent = Mathf.Lerp(-60 / 90f, 60 / 90f, (dot + 1f) / 2f);


        float speedAdd = 0f;
        if (dotPercent > 0f)
        {
            speedAdd = gravityForce * dotPercent * speedGain.Evaluate(_speed) * Time.fixedDeltaTime;
        }
        else
        {
            speedAdd = gravityForce * dotPercent * speedLoss.Evaluate(1f - _speed) * Time.fixedDeltaTime;
        }

        return speedAdd;
    }
}
