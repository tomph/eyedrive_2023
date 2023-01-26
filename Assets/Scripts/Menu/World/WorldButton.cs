using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : Button
{
    [SerializeField]
    private World _world;

    [SerializeField]
    public Image lockImage;

    public Signal<World> ON_SELECTED = new Signal<World>();

    private static Color DeselectedColor = new Color(.60f, .60f,60f,1f);

    protected override void Awake()
    {
        onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        ON_SELECTED.Dispatch(_world);
    }

    public bool selected
    {
        set
        {
            GetComponent<Image>().color = value == true ? Color.white : DeselectedColor;
            if(value == true) GetComponent<Animator>().Play("Selected");
        }
    }

    public World world
    {
        get
        {
            return _world;
        }

        set
        {
            _world = value;
        }
    }
}
