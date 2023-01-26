using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class GazeBase : MonoBehaviour
{
    public Signal<GazeBase> ON_GAZE_ENTER = new Signal<GazeBase>();
    public Signal ON_GAZE_EXIT = new Signal();

    private EventTrigger _eventTrigger;
    
    public Vector2 dwellOffset = Vector2.zero;

    public abstract void Invoke();
    
    public abstract void Init(EventSystem eventSystem);

    public bool isEnabled = true;

    public void Awake()
    {
        
        _eventTrigger = gameObject.AddComponent<EventTrigger>();
        
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
        _eventTrigger.triggers.Add(entryEnter);
        
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
        _eventTrigger.triggers.Add(entryExit);

    }

    public void OnPointerEnterDelegate(PointerEventData pointerEventData)
    {
        if(isEnabled) ON_GAZE_ENTER.Dispatch(this);
    }

    public void OnPointerExitDelegate(PointerEventData pointerEventData)
    {
        ON_GAZE_EXIT.Dispatch();
    }

    public void Cancel()
    {
        ON_GAZE_EXIT.Dispatch();
    }
    
    // public Rect GetScreenCoordinates(RectTransform uiElement)
    // {
    //     var worldCorners = new Vector3[4];
    //     uiElement.GetWorldCorners(worldCorners);
    //     var result = new Rect(
    //         worldCorners[0].x,
    //         worldCorners[0].y,
    //         worldCorners[2].x - worldCorners[0].x,
    //         worldCorners[2].y - worldCorners[0].y);
    //     return result;
    // }
}
