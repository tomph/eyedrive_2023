using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SettingSlider : MonoBehaviour
{
    [SerializeField]
    private Text _label;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _label.text = _slider.value.ToString();
    }

    public void UpdateValLabel()
    {
        _label.text = _slider.value.ToString();
    }
}
