using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectionController : MenuPanel
{
    [SerializeField]
    private Button _tourButton;

    [SerializeField]
    private Button _quickButton;

    // Start is called before the first frame update
    void Start()
    {


        _tourButton.onClick.AddListener(OnSelectTour);

        _quickButton.onClick.AddListener(OnSelectQuick);

    }

    private void OnSelectTour()
    {
        Debug.Log("select tour");
    }

    private void OnSelectQuick()
    {
        Debug.Log("select quick");
    }
}
