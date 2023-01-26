
using UnityEngine;

public class EyeGazePreciseInput : MonoBehaviour
{
    private Vector3 _lastPos;
    private Vector3 _pos = Vector3.zero;
    private bool _moved = false;
    private Vector3 _mousePos = Vector3.zero;
    [SerializeField]
    private Camera _cam;

    [SerializeField] private AnimationCurve _easing;
    

    public Signal<Vector3> ON_POSITION_UPDATE = new Signal<Vector3>();

    [SerializeField]
    private Rect _deadZone = new Rect(.45f, .45f, .1f, .1f);


    private void Awake()
    {
        _moved = false;
        _cam = Camera.main;
    }

    public void Tick(float delta)
    {
        _mousePos = Input.mousePosition;
        
        Vector3 vpPos = _cam.ScreenToViewportPoint(_mousePos);
        Vector2 clamped = new Vector2(vpPos.x - .5f, vpPos.y - .5f);

        if (_deadZone.Contains(new Vector2(Mathf.Abs(clamped.x), Mathf.Abs(clamped.y))) == false)
        {
            _pos = Vector3.zero;
        }
        else
        {
            _pos = new Vector3(clamped.x * 2, clamped.y * 2);
        }
        
        if (_pos != _lastPos)
        {
            _lastPos = _pos;
            ON_POSITION_UPDATE.Dispatch(_pos);
        }
    }
    

    public void Pause(float delta)
    {
        _pos = Vector3.zero;
        _lastPos = _pos;
    }


    public float GetShipDistance(Vector3 ship)
    {
        Vector3 shipScreenPos = _cam.WorldToScreenPoint(ship);
        Vector3 gazeScreenPos = _mousePos;

        float side = Util.AngleDir(Vector3.forward, gazeScreenPos - shipScreenPos, Vector3.up);

        float  dist = Vector3.Distance(shipScreenPos, gazeScreenPos) / Screen.width;

        float distX = Vector3.Project(shipScreenPos - gazeScreenPos, -Vector3.right).x/Screen.width;
        float clamped = dist.Remap(0, .5f, 0, 1, true);
        float distXMapped = Mathf.Clamp((distX * 2) * -1, -1, 1);

        return _easing.Evaluate(Mathf.Abs(distXMapped))*side;
    }
}
