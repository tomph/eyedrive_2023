using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GazeButton : GazeBase
{
    private Button _button;
    private EventSystem _eventSystem;
    private ColorBlock _colours;

    private void Start()
    {

        _button = GetComponent<Button>();
        _button.onClick.AddListener(Cancel);

        if (GetComponent<ButtonSound>() == null)
        {
            gameObject.AddComponent<ButtonSound>();
        }

        _colours = ColorBlock.defaultColorBlock;
        _colours.normalColor = Color.white;
        _colours.disabledColor = new Color(200f, 200f, 200f, 0.5f);
        _colours.highlightedColor = new Color(245f / 255f, 148f / 255f, 245f / 255f);

        _button.colors = _colours;
    }



    public override void Invoke()
    {
        ExecuteEvents.Execute(_button.gameObject, new BaseEventData(_eventSystem), ExecuteEvents.submitHandler);
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
            _button.interactable && alphaVisible &&
            screenRect.Contains(GazeSystem.uiCam.WorldToScreenPoint(GetComponent<RectTransform>().position));

    }

    public Button button => _button;
}
