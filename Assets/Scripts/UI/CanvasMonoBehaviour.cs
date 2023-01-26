using UnityEngine;

/// <summary>
/// A substitude for MonoBehaviour that gives direct access to commonly used behaviours
/// - CanvasGroup
/// </summary>
public class CanvasMonoBehaviour : MonoBehaviour
{
    public Signal RECT_CHANGED = new Signal();

    private void OnRectTransformDimensionsChange()
    {
        RECT_CHANGED.Dispatch();
    }

    protected CanvasGroup _canvasGroup;
    public CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = Util.GetOrAddComponent<CanvasGroup>(gameObject);
            return _canvasGroup;
        }
    }

    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    public bool show
    {
        set
        {
            gameObject.SetActive(value);
        }
    }
}
