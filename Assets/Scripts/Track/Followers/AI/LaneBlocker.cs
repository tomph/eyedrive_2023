using System;
using Dreamteck.Splines;
using UnityEngine;

public class LaneBlocker : MonoBehaviour
{
    private float _laneWidth;
    private float _laneChangeFrequency;
    private float _speed;

    private Rigidbody _body;

    private float _laneChangeElapsed = 0;

    private Vector3 _force = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;

    private Vector3 _offset = Vector3.zero;

    private Vector3 _anchor = Vector3.zero;
    private Vector3 _target = Vector3.zero;

    private int _lane = 0;

    public void Init(float laneWidth, float laneChangeFrequency, float speed)
    {
        _laneChangeFrequency = laneChangeFrequency;
        _laneWidth = laneWidth;
        _speed = speed;

        //_lane = Random.Range(-1, 1);
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();

        _laneChangeFrequency = 3;
        _laneWidth = 10;
        _speed = 1;

        _target = _anchor = _offset = Vector3.zero;
        _lane = 0;

    }

    void Start()
    {
        _anchor = _body.position;

        _lane = UnityEngine.Random.Range(0, 1) < .5f ? -1 : 1;
        SwitchLane();
    }

    private void FixedUpdate()
    {
        if(_lane != 0) UpdateMotion();
    }

    void UpdateMotion()
    {
        _body.position = Vector3.Lerp(_body.position, _target, Time.fixedDeltaTime*_speed);

        if (Vector3.Distance(_body.position, _target) < 1) SwitchLane();
    }

    private void SwitchLane()
    {
        _lane = _lane * -1;
        _offset = transform.right * (_lane * _laneWidth);
        _target = _anchor + _offset;
    }
}
