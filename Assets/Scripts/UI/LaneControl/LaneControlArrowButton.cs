using EyegazeCore;

using UnityEngine;

using UnityEngine.UI;


[RequireComponent(typeof(StandardAccessibleButton))]
public class LaneControlArrowButton : MonoBehaviour, IAccessUpdatable
{
    [SerializeField]
    private Image arrowFill;
    
    [SerializeField]
    private Image hl;
    
    private ColorBlock _colours;
    private AccessVO _access;

    private StandardAccessibleButton _button;

    private void Awake()
    {
        _button = GetComponent<StandardAccessibleButton>();

        _colours = ColorBlock.defaultColorBlock;
        _colours.normalColor = Color.white;
        _colours.disabledColor = new Color(200f, 200f, 200f, 0.5f);
        _colours.highlightedColor = new Color(245f / 255f, 148f / 255f, 245f / 255f);

        _button.GetComponent<Button>().colors = _colours;
    }


    void Update()
    {
        if (_access == null) return;

        if (_button.state != ButtonState.Idle)
            hl.fillAmount = Mathf.Clamp01(_button.animator.growTime / _access.eyeGazeConfig.dwellTime);
        else hl.fillAmount = 0;

    }

    public void OnUpdate(AccessVO data)
    {
        _access = data;
    }
}
