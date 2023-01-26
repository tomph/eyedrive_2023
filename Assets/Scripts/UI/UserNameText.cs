using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameText : MonoBehaviour
{
    private Text _tp;

    internal void Init(string username)
    {
        _tp = GetComponent<Text>();
        _tp.text = username;
    }
}