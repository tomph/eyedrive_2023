using Dreamteck.Splines;

using UnityEngine;

public class AIFollower : BaseFollowerAI
{

    public float SPEED = 100f;

    public float MAX_ROLL = 20f;


    public float SLERP_TIME = 2.5f;

    public float LANE_SIZE = 12;
    public float LANE_CHANGE_SPEED = 40;

    private Vector3 _laneMovement = Vector3.zero;
    
    private float _turn = 0;
    private float _roll = 0;

    private int _lane = 0;
    private int _intendedLane = 0;

    private double _lastpercent = 0.0;
    private float _laneChangeElapsed = 0;
    private float _laneChangeSeed = 1;

    private SplineResult _cResult;
    
    public override void OnAwake()
    {
        _body = GetComponent<Rigidbody>();
        _laneChangeSeed = UnityEngine.Random.Range(3, 10);
    }

    public void Init(SplineComputer computer, double percent)
    {
        this.computer = computer;
        SetPercent(percent);
        _lastpercent = percent;
        _body.transform.position = result.position;
    }
    
    
    void UpdateResult()
    {
        float clamped = Mathf.Repeat((float)_lastpercent + 0.005f, 1);
        double clampedD = System.Convert.ToDouble(clamped);

        //Loop
        if (clampedD < _lastpercent)
        {
            _lastpercent = 0;

            clamped = Mathf.Repeat((float)_lastpercent + 0.005f, 1);
            clampedD = System.Convert.ToDouble(clamped);
        }

        double percent = address.Project(_body.position, 2, _lastpercent, clampedD);
        this.SetPercent(percent);

        _lastpercent = percent;


        //_cResult = address.Evaluate(percent);
        this._cResult = this.result;
    }

    public override void OnUpdate()
    {
        UpdateResult();
        UpdateMotion();
        UpdateLane();
    }

    void UpdateLane()
    {
        if(_laneChangeElapsed > _laneChangeSeed)
        {
            ChangeLane();
        }
        else
        {
            _laneChangeElapsed += Time.deltaTime;
        }

        if (_lane == -1) intention = Intention.Left;
        else if (_lane == 0) intention = Intention.Middle;
        else intention = Intention.Right;
    }

    void ChangeLane()
    {
        _lane = Random.Range(-1, 1);
        _laneChangeElapsed = 0;
        _laneChangeSeed = UnityEngine.Random.Range(3, 10);
    }

    void UpdateMotion()
    {
        if (_cResult != null)
        {
            //UpdateBounds(_cResult);

            Quaternion rot = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler((-_body.transform.forward * (_roll * MAX_ROLL))) * _cResult.rotation, Time.fixedDeltaTime * SLERP_TIME);

            _body.MoveRotation(rot);
            _laneMovement = Vector3.Lerp(_laneMovement, _cResult.right * (_lane * LANE_SIZE), Time.fixedDeltaTime * LANE_CHANGE_SPEED);

            Vector3 anchor = _cResult.position + _cResult.right * (_lane * LANE_SIZE);
            Vector3 dest = anchor - _body.position;
            Vector3 fwd = _body.transform.forward * (SPEED * StaticEyeDriveSettingsVO.INSTANCE.SPEED);

            float lerpSpeed = isOnGround ? 50 : 20;
            _body.AddForce(dest * lerpSpeed + fwd, ForceMode.Force);

            // replicating speed from manual follower, for vfx
            _speed = transform.InverseTransformDirection(_body.velocity).z / 20;
         
        }
    }

    public override void OnGazeUpdated(Vector3 pos)
    {
    }

    public override void OnPlayPaused()
    {
        // _trails.SetActive(false);
    }
}
