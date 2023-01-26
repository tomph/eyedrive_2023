using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectionController : MenuPanel
{
    private Sprite[] _sprites;

    [SerializeField]
    public Button _goButton;

    private void Start()
    {
        _goButton.onClick.AddListener(Next);
    }
}
