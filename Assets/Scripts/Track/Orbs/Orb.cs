using Dreamteck.Splines;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Collider _collider;
    private Vector3 _anchor;
    private ParticleSystem[] _particles;

    public SplineComputer computer;
    public double SplinePosition = 0.0;

    public Signal<Orb> ON_HIT_BY_PLAYER = new Signal<Orb>();

    private void Awake()
    {
        _particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        _anchor = transform.position;
        _collider = GetComponentInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ON_HIT_BY_PLAYER.Dispatch(this);
        }
    }

    public void Follow(Vector3 pos)
    {
        _collider.enabled = true;
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.fixedDeltaTime * 30);
    }

    public void Retreat()
    {
        if(_collider == null) _collider = GetComponentInChildren<Collider>();

        _collider.enabled = false;
        transform.position = Vector3.MoveTowards(transform.position, _anchor, Time.fixedTime * 5);
    }

    public bool active
    {
        set
        {
            foreach(ParticleSystem ps in _particles) ps.gameObject.SetActive(value);
        }
    }

    private void OnDestroy()
    {
        ON_HIT_BY_PLAYER.RemoveAll();
    }
}
