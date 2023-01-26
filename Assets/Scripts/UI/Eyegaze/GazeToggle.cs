using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class GazeToggle : GazeBase
{
    private Toggle _toggle;
    private EventSystem _eventSystem;
    private ColorBlock _colours;
    
    [SerializeField]
    private float m_Width;
    [SerializeField]
    private float m_Height;

    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }

    private void toggled(bool enabled)
    {
        Cancel();
    }
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        
        toggle.onValueChanged.AddListener(toggled);
        
        _colours = ColorBlock.defaultColorBlock;
        _colours.normalColor = Color.white;
        _colours.disabledColor = new Color(200f, 200f, 200f, 0.5f);
        _colours.highlightedColor = new Color(245f / 255f, 148f / 255f, 245f / 255f);

        _toggle.colors = _colours;
        
        
        // create child object
        GameObject hit = new GameObject("HitArea");

        hit.AddComponent<CanvasRenderer>();
        
        RectTransform hitzoneRectTransform = hit.AddComponent<RectTransform>();
        {
            hitzoneRectTransform.SetParent(transform);
            hitzoneRectTransform.localPosition = Vector3.zero;
            hitzoneRectTransform.localScale = Vector3.one;
            hitzoneRectTransform.sizeDelta = new Vector2(m_Width, m_Height);
        }

        // create transparent graphic
        hit.AddComponent<EmptyGraphic>();
    }

    void OnValidate()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            m_Width = Mathf.Max(m_Width, rectTransform.sizeDelta.x);
            m_Height = Mathf.Max(m_Height, rectTransform.sizeDelta.y);
        }
    }

    public override void Invoke()
    {
        ExecuteEvents.Execute(_toggle.gameObject, new BaseEventData(_eventSystem), ExecuteEvents.submitHandler);
    }

    public override void Init(EventSystem eventSystem)
    {
        _eventSystem = eventSystem;
    }

    private void Update()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        CanvasGroup[] cgs = GetComponentsInParent<CanvasGroup>();

        bool alphaVisible = true;

        // check all parent Canvas groups are visible
        for (int i = 0; i < cgs.Length; i++)
        {
            if (cgs[i].alpha == 0)
            {
                alphaVisible = false;
                break;
            }
        }

        isEnabled =
            _toggle.interactable &&
            alphaVisible &&
            screenRect.Contains(GazeSystem.uiCam.WorldToScreenPoint(GetComponent<RectTransform>().position));
    }

    public Toggle toggle => _toggle;
}
