using Dreamteck.Splines;
using UnityEngine;

public abstract class BaseFollowerAI : SplineFollower
{
    public static float NODE_GATE_PADDING = 30;

    private Node _nextNode = null;
    private NodeGate _gate = null;

    public abstract void OnPlayPaused();
    public abstract void OnUpdate();
    public abstract void OnAwake();
    public abstract void OnGazeUpdated(Vector3 pos);

    private SplineComputer _computer;
    private double _lastPercent = 0;

    private bool _exitAssigned = false;
    private bool _addressAssigned = false;

    private PIDController _pid;

    protected Intention intention;
    protected float _speed = 0;
    protected Rigidbody _body;
    
    private VehicleAVFX _avfx;

    private int _route = -1;

    private Vector3 _gaze = Vector3.zero;

    private SplineResult _currentResult;


    private static Vector3 DEFAULT_GRAVITY = new Vector3(0, -9.81f, 0);

    //for grabbing current computer
    private SplineComputer _currentComp;
    private double _currentPercent;
    private Spline.Direction _currentDirection;

    public static Signal ON_LAP_FINISHED = new Signal();

    protected bool isOnGround = false;

    [Header("Hover Settings")]
    public float hoverHeight = 1.5f;        //The height the ship maintains when hovering
    public float maxGroundDist = 5f;        //The distance the ship can be above the ground before it is "falling"
    public float hoverForce = 300f;         //The force of the ship's hovering
    public float hoverGravity = 20f;        //The gravity applied to the ship while it is on the ground
    public float fallGravity = 80f;         //The gravity applied to the ship while it is falling
    public LayerMask groundMask;

    //gaze side*distance = turn
    
    void Awake()
    {
        base.Awake();
        OnAwake();

        _pid = new PIDController();
        _body = GetComponent<Rigidbody>();
    }

    private void OnPause(bool val)
    {
        if (val)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    public void Tick(float delta)
    {
        CalculateHover();
        OnUpdate();
        UpdateFX(delta);

        _lastPercent = _currentPercent;
        Physics.gravity = DEFAULT_GRAVITY;
    }

    private void UpdateFX(float delta)
    {
       avfx.Tick(delta, _body.velocity.magnitude);
    }

    public void Pause()
    {
        _body.velocity = Vector3.zero;
        Physics.gravity = Vector3.zero;
        OnPlayPaused();
    }
    

    protected SplineResult currentResult
    {
        get
        {
            return _currentResult;
        }
    }

    protected SplineComputer currentComputer
    {
        get
        {
            return _computer;
        }
    }

    void CalculateHover()
    {
        //This variable will hold the "normal" of the ground. Think of it as a line
        //the points "up" from the surface of the ground
        Vector3 groundNormal;

        //Calculate a ray that points straight down from the ship
        Ray ray = new Ray(transform.position, -transform.up);

        //Declare a variable that will hold the result of a raycast
        RaycastHit hitInfo;

        //Determine if the ship is on the ground by Raycasting down and seeing if it hits 
        //any collider on the whatIsGround layer
        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, groundMask);

        //If the ship is on the ground...
        if (isOnGround)
        {
            //...determine how high off the ground it is...
            float height = hitInfo.distance;
            //...save the normal of the ground...
            groundNormal = hitInfo.normal.normalized;
            //...use the PID controller to determine the amount of hover force needed...
            float forcePercent = _pid.Seek(hoverHeight, height);

            //...calulcate the total amount of hover force based on normal (or "up") of the ground...
            Vector3 force = groundNormal * hoverForce * forcePercent;
            //...calculate the force and direction of gravity to adhere the ship to the 
            //track (which is not always straight down in the world)...
            Vector3 gravity = -groundNormal * hoverGravity * height;

            //...and finally apply the hover and gravity forces
            _body.AddForce(force, ForceMode.Acceleration);
            _body.AddForce(gravity, ForceMode.Acceleration);
        }
        //...Otherwise...
        else
        {
            //...use Up to represent the "ground normal". This will cause our ship to
            //self-right itself in a case where it flips over
            groundNormal = Vector3.up;

            //Calculate and apply the stronger falling gravity straight down on the ship
            Vector3 gravity = -groundNormal * fallGravity;
            _body.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    protected float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public Vector3 gaze
    {
        get
        {
            return _gaze;
        }
    }

    public float speed
    {
        get
        {
            return _speed;
        }
    }


    public double projectedPosition
    {
        get
        {
            return _result.percent;
        }
    }

    private VehicleAVFX avfx
    {
        get
        {
            if (_avfx == null)
            {
                _avfx = GetComponent<VehicleAVFX>();
            }

            return _avfx;
        }
    }
}
