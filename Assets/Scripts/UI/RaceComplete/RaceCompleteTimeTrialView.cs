using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCompleteTimeTrialView : MonoBehaviour
{
    private CanvasGroup _group;

    [SerializeField]
    private List<Text> _times;

    [SerializeField]
    private Text _fastestTime;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        Debug.Log("Show Times");
        _group.alpha = 1;
        _group.interactable = _group.blocksRaycasts = true;
    }

    public void Hide()
    {
        Debug.Log("Hide Times");
        _group.alpha = 0;
        _group.interactable = _group.blocksRaycasts = false;
    }

    public void Draw(List<float> times)
    {
        _fastestTime.text = Util.FormatTimeMMSSMSMS(StaticEyeDriveSession.INSTANCE.bestTime);

        for(int i = 0; i < times.Count; i++)
        {

            if (_times.Count >= i + 1) _times[i].text = "LAP 0" + (i+1).ToString() + ": <size=40>" + Util.FormatTimeMMSSMSMS(times[i]) + "</size>";
        }
    }
}
