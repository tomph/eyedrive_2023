using EyegazeCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionController : MenuPanel
{
    [SerializeField]
    private StandardAccessibleButton _goButton;

    [SerializeField]
    private StandardAccessibleButton _orbCollect, _timeTrial;

    private EyeDriveSession _session;

    // Start is called before the first frame update
    public void Init(EyeDriveSession session)
    {
        _session = session;

        _orbCollect.ON_ANIMATED_ACTION.AddListener(() => { OnWorldSelected(GameType.Collect);});
        _timeTrial.ON_ANIMATED_ACTION.AddListener(() => { OnWorldSelected(GameType.TimeTrial); });

        _goButton.ON_ANIMATED_ACTION.AddListener(Next);

        OnWorldSelected(session.gameType); ;
    }

    private void OnWorldSelected(GameType obj)
    {
        if(_session)
            _session.gameType = obj;

        CanvasGroup orb = _orbCollect.GetComponent<CanvasGroup>();
        CanvasGroup time = _timeTrial.GetComponent<CanvasGroup>();

        orb.alpha = obj == GameType.Collect ? 1f : .5f;
        time.alpha = obj == GameType.TimeTrial ? 1f : .5f;
    }
}
