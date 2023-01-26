using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCompletePanelController : MonoBehaviour
{
    [SerializeField]
    private Button _homeButton;

    [SerializeField]
    private Button _nextButton;

    [SerializeField]
    private Button _replayButton;

    [SerializeField]
    private RaceCompleteOrbView _orbView;

    [SerializeField]
    private RaceCompleteTimeTrialView _timeView;

    [SerializeField]
    private Text _header;

    public Signal ON_REPLAY = new Signal();
    public Signal ON_NEXT = new Signal();
    public Signal ON_HOME = new Signal();

    private CanvasGroup _group;

    // Start is called before the first frame update
    void Start()
    {
        _homeButton.onClick.AddListener(OnHome);
        _nextButton.onClick.AddListener(OnNext);
        _replayButton.onClick.AddListener(OnReplay);

        gameObject.SetActive(false);
    }

    public void Show(List<float> times)
    {
        ShowView();
        UpdateHeader();

        _orbView.Hide();

        _timeView.Show();
        _timeView.Draw(times);
    }

    private void UpdateHeader()
    {
        _header.text = "Results: <color=#FC00FF>" + GetWorldName(StaticEyeDriveSession.INSTANCE.world) + " 0" + (StaticEyeDriveSession.INSTANCE.track + 1).ToString() + "</color>";
    }

    private string GetWorldName(World world)
    {
        string n = "DORIN IV";
        switch(world)
        {
            case World.City:
                n = "LA CUNA";
                break;
            case World.Space:
                n = "SOL BASE";
                break;
        }

        return n;
    }

    public void Show(int collected, int total)
    {
        ShowView();
        UpdateHeader();

        _timeView.Hide();

        _orbView.Show();
        _orbView.Draw(collected, total);
    }

    void ShowView()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        
    }

    private void OnReplay()
    {
        ON_REPLAY.Dispatch();
    }

    private void OnNext()
    {
        ON_NEXT.Dispatch();
    }

    private void OnHome()
    {
        ON_HOME.Dispatch();   
    }
}
