using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    public Signal ON_NEXT = new Signal();
    public Signal<int> ON_SELECT_UPDATE = new Signal<int>();


    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    protected void Next()
    {
        ON_NEXT.Dispatch();
    }

    public void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
    }
}
