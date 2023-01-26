using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTrackDisplay : MenuPanel
{

    [SerializeField] private Image _worldDisplay;
    [SerializeField] private Image _trackDisplay;
    
    [SerializeField] private Button _goButton;
    [SerializeField] private Button _selectTrackButton;


    public void Init(EyeDriveWorld world)
    {
        _goButton.onClick.AddListener(Next);

        Debug.Log("update next track display: " + StaticEyeDriveSession.INSTANCE.world + " index " + StaticEyeDriveSession.INSTANCE.track);
        string worldStr = System.Enum.GetName(typeof(World), StaticEyeDriveSession.INSTANCE.world);

        _worldDisplay.sprite = world.screenshot;

        _trackDisplay.sprite = world.thumbs[(int)StaticEyeDriveSession.INSTANCE.track];

        _worldDisplay.enabled = true;
        _trackDisplay.enabled = true;
    }
    
    
}
