using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileUIController : MonoBehaviour
{
    [SerializeField]
    private AdvancedButton _left;

    [SerializeField]
    private AdvancedButton _right;

    public Signal ON_RIGHT_CLICK = new Signal();
    public Signal ON_RIGHT_DOWN = new Signal();
    public Signal ON_LEFT_CLICK = new Signal();
    public Signal ON_LEFT_DOWN = new Signal();

    private bool _isLeft;
    private bool _isRight;

    private void Awake()
    {
        _left.onDown.AddListener(OnLeftDown);
        _left.onUp.AddListener(OnLeftUp);

        _right.onDown.AddListener(OnRightDown);
        _right.onUp.AddListener(OnRightUp);
    }

    private void OnRightUp()
    {
        _isRight = false;
        ON_RIGHT_CLICK.Dispatch();
    }

    private void OnRightDown()
    {
        _isRight = true;
    }

    private void OnLeftUp()
    {
        _isLeft = false;
        ON_LEFT_CLICK.Dispatch();
    }

    private void OnLeftDown()
    {
        _isLeft = true;
    }

    public bool isIdle()
    {
        return (_isLeft == false && _isRight == false);
    }

    private void Update()
    {
        if(_isRight) ON_RIGHT_DOWN.Dispatch();
        if(_isLeft) ON_LEFT_DOWN.Dispatch();
    }
}
