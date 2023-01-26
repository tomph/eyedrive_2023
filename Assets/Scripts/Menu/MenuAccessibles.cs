using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EyegazeCore;

public class MenuAccessibles : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<MenuState, List<StandardAccessibleButton>> _accessibles;

    public MenuState state
    {
        set
        {
            foreach (MenuState menuState in _accessibles.Keys)
            {
                if (_accessibles.ContainsKey(menuState) && _accessibles[menuState] != null)
                {
                    foreach (StandardAccessibleButton s in _accessibles[menuState])
                    {
                        s.interactable = (value == menuState);
                    }
                }
            }
        }
    }
}