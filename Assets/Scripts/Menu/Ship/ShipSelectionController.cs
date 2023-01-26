using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionController : MenuPanel
{
    private LoopedImageFader _fader;

    [SerializeField]
    private Button _goButton;

    private void Start()
    {
        _fader = GetComponentInChildren<LoopedImageFader>();
        _goButton.onClick.AddListener(Next);
    }

    public void Init(List<Sprite> sprites)
    {
        _fader.Init(sprites);
    }
}
