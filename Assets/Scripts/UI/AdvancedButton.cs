using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

[AddComponentMenu("TMW/Advanced Button")]
public class AdvancedButton : Button
{
    // Event delegate triggered on mouse or touch down.
    [SerializeField]
    ButtonDownEvent _onDown = new ButtonDownEvent();

    [SerializeField]
    ButtonUpEvent _onUp = new ButtonUpEvent();

    CanvasMonoBehaviour _canvasMonoBehaviour;
    public CanvasMonoBehaviour canvasMonoeBehaviour
    {
        get
        {
            if (_canvasMonoBehaviour == null)
                _canvasMonoBehaviour = Util.GetOrAddComponent<CanvasMonoBehaviour>(gameObject);
            return _canvasMonoBehaviour;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if(interactable)
            _onDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (interactable)
            _onUp.Invoke();
    }

    public ButtonDownEvent onDown
    {
        get { return _onDown; }
        set { _onDown = value; }
    }

    public ButtonUpEvent onUp
    {
        get { return _onUp; }
        set { _onUp = value; }
    }

    [Serializable]
    public class ButtonDownEvent : UnityEvent { }

    [Serializable]
    public class ButtonUpEvent : UnityEvent { }
}