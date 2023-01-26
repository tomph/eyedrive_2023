using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseFollower : SplineFollower
{
    public Signal ON_JUNCTION = new Signal();
    
    private Node _nextNode = null;
    private NodeGate _gate = null;

    protected abstract void OnPauseUpdate(float delta);
    protected abstract void OnUpdate(float delta);

    protected abstract void OnInit(double percent);

    public abstract void OnAwake();

    private SplineComputer _computer;

    private bool _exitAssigned = false;
    private bool _addressAssigned = false;

    private PIDController _pid;

    private Intention _intention;
    protected float _speed = 0;
    protected Rigidbody _body;
    protected bool _inJunction = false;
    protected double _lastpercent = 0.0;

    private VehicleAVFX _avfx;
    private float _lapCoolDown = 0;

    private int _route = -1;

    protected bool braking = false;

    private bool _isComplete = false;


    private SplineResult _currentResult;


    private static Vector3 DEFAULT_GRAVITY = new Vector3(0, -9.81f, 0);

    //for grabbing current computer
    private SplineComputer _currentComp;
    private double _currentPercent;
    private Spline.Direction _currentDirection;

    public  Signal ON_LAP_COMPLETE = new Signal();
    
    protected bool isOnGround = false;

    [BoxGroup("Physics Properties")] public float _mass, _drag, _angularDrag;
    
    [BoxGroup("Hover Settings")]
    public float hoverHeight = 1.5f;        //The height the ship maintains when hovering
    [BoxGroup("Hover Settings")]
    public float maxGroundDist = 5f;        //The distance the ship can be above the ground before it is "falling"
    [BoxGroup("Hover Settings")]
    public float hoverForce = 300f;         //The force of the ship's hovering
    [BoxGroup("Hover Settings")]
    public float hoverGravity = 20f;        //The gravity applied to the ship while it is on the ground
    [BoxGroup("Hover Settings")]
    public float fallGravity = 80f;         //The gravity applied to the ship while it is falling
    [BoxGroup("Hover Settings")]
    public LayerMask groundMask;
    
    void Awake()
    {
        base.Awake();

        _pid = new PIDController();
        _body = GetComponent<Rigidbody>();
        
        
        OnAwake();
    }
    
    private void Reset()
    {
        
    }

    public void Tick(float delta)
    {
        CalculateHover();
        
        _lapCoolDown -= Time.fixedDeltaTime;

        Physics.gravity = DEFAULT_GRAVITY;

        _body.constraints = RigidbodyConstraints.None;
        _body.angularDrag = _angularDrag;
        _body.drag = _drag;
        _body.mass = _mass;
        
        UpdateFX(delta);
        OnUpdate(delta);
    }

    public void OnPause(float delta)
    {
        _body.drag = 100;
        CalculateHover();
        OnPauseUpdate(delta);
    }
    
    private void UpdateFX(float delta)
    {
        avfx.Tick(delta, body.velocity.magnitude, braking);
    }

    public void Init(double percent)
    {
        ThreeWayJunction[] junctions = computer.GetComponentsInChildren<ThreeWayJunction>();
        foreach (ThreeWayJunction j in junctions)
        {
            j.ON_JUNCTION_TRIGGERED.RemoveAllListeners();
            j.ON_JUNCTION_TRIGGERED.AddListener(this.OnJunctionEntered);
            
            j.ON_JUNCTION_EXIT.RemoveAllListeners();
            j.ON_JUNCTION_EXIT.AddListener(this.OnJunctionExited);
        }

        SetPercent(percent);
        _lastpercent = result.percent;

        // _lastPercent = _currentPercent = _result.percent;
          _body.position = result.position;
          _body.transform.forward = result.direction;
          
          OnInit(percent);
    }

    private void OnJunctionExited()
    {
        _inJunction = false;
    }

    private void OnJunctionEntered(JunctionAddress junctionAddress)
    {
        Debug.Log("OnJunctionEntered: " + junctionAddress.direction + " / " + intention);
        
        if(junctionAddress.direction == intention)
        {
            //Clear
            if (address.elements.Length > 1) ExitAddress(1);

            //Assign Route
            EnterAddress(junctionAddress.node, junctionAddress.index);

            //Assign Exit
            EnterAddress(junctionAddress.exit, 0);
        }

        ON_JUNCTION.Dispatch();
        
        _inJunction = true;
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("FinishLine") && _lapCoolDown <= 0)
        {
            OnFinishLine();

            //stop ships going backwards and re-triggering lap
            _lapCoolDown = 5;
        }
    }

    protected void OnFinishLine()
    {
        ON_LAP_COMPLETE.Dispatch();
        
        ClearAddress();
        CollapseAddress();
    }
    

   

    void CalculateHover()
    {
        //This variable will hold the "normal" of the ground. Think of it as a line
        //the points "up" from the surface of the ground
        Vector3 groundNormal;

        //Calculate a ray that points straight down from the ship
        Transform trn = transform;
        Ray ray = new Ray(trn.position, -trn.up);

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
            _body.AddForce(force, ForceMode.Force);
            _body.AddForce(gravity, ForceMode.Force);
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

    public Rigidbody body
    {
        get
        {
            if (_body == null)
                _body = GetComponent<Rigidbody>();

            return _body;
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

    protected Intention intention
    {
        set
        {
            Debug.Log("set intention: " + value);
            _intention = value;
        }

        get => _intention;
    }
}
