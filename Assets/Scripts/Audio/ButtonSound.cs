using EyegazeCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{

    [SerializeField]
    private List<string> AdditionalClickSounds;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        //default sound
        SFX.PlayOneShot("Eyedrive_SFX_Menu_Click Select_v.1");

        if(AdditionalClickSounds != null && AdditionalClickSounds.Count > 0)
        {
            foreach(string s in AdditionalClickSounds) SFX.PlayOneShot(s);
        }
    }
}
