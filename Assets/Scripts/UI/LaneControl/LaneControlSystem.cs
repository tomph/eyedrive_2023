using EyegazeCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LaneControlSystem : MonoBehaviour
{

    public Signal<int> ON_CHOOSE_DIR = new Signal<int>();

    private CanvasGroup _container;
    
    [SerializeField]
    private EventSystem _eventSystem;
    
    [SerializeField]
    private StandardAccessibleButton _leftArrow;

    [SerializeField]
    private StandardAccessibleButton _rightArrow;

    [SerializeField]
    private LaneControlArrowButton _frontArrow;

    
    private int _lastLane = 0;
    private int _direction = 0;
    private int _lastDirection = 0;

    private void Awake()
    {
        _container = GetComponent<CanvasGroup>();

        _leftArrow.ON_IMMEDIATE_ACTION.AddListener(OnLeftClick);
        _rightArrow.ON_IMMEDIATE_ACTION.AddListener(OnRighClick);
        //_frontArrow.button.onClick.AddListener(OnMiddleClick);

        Hide();
    }

    // private void OnMiddleClick()
    // {
    //     Debug.Log("select lane middle");
    //     ON_CHOOSE_DIR.Dispatch(0);
    // }
    
    private void OnLeftClick()
    {
        Debug.Log("select lane left");
        ON_CHOOSE_DIR.Dispatch(-1);
    }
    
    private void OnRighClick()
    {
        Debug.Log("select lane right");
        ON_CHOOSE_DIR.Dispatch(1);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
