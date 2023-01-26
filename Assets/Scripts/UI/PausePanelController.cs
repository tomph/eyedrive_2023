using EyegazeCore;
using UnityEngine;
using UnityEngine.Events;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private StandardAccessibleButton _exit, _restart, _settings, _resume;

    public void Show()
    {
        gameObject.SetActive(true);

        Debug.Log("ShowPausePanel");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public UnityEvent ON_EXIT => _exit.ON_ANIMATED_ACTION;
    public UnityEvent ON_RESTART => _restart.ON_ANIMATED_ACTION;
    public UnityEvent ON_SETTINGS => _settings.ON_ANIMATED_ACTION;

    public UnityEvent ON_RESUME => _resume.ON_ANIMATED_ACTION;

}
