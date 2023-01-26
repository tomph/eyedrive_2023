using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeButton : Button
{
    [SerializeField]
    private GameType _gameType;

    public Signal<GameType> ON_SELECTED = new Signal<GameType>();

    private static Color DeselectedColor = new Color(.42f, .42f, .42f, 1f);

    private void Awake()
    {
        onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        ON_SELECTED.Dispatch(_gameType);
    }

    public bool selected
    {
        set
        {
            GetComponent<Image>().color = value == true ? Color.white : DeselectedColor;
            if (value == true) GetComponent<Animator>().Play("Selected");
        }
    }

    public GameType mode
    {
        get
        {
            return _gameType;
        }

        set
        {
            _gameType = value;
        }
    }
}
