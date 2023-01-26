using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueClicker : MonoBehaviour
{
    [SerializeField]
    private Button _minusButton;

    [SerializeField]
    private Button _addButton;

    [SerializeField]
    private Text _textValue;

    [SerializeField]
    private List<string> _labels = new List<string>();

    [Range(0, 1)]
    [SerializeField]
    private float rangeMin = 0;

    [Range(0, 1)]
    [SerializeField]
    private float rangeMax = 1;

    public Signal<float> ON_VALUE_CHANGE = new Signal<float>();

    private int _range = 0;
    private int _index = 0;

    void Awake()
    {
        _range = _labels.Count;

        _addButton.onClick.AddListener(OnAdd);
        _minusButton.onClick.AddListener(OnMinus);
    }

    private void Draw()
    {
        _textValue.text = _labels.Count < _index ? "null" : _labels[_index];
        _addButton.interactable = _index < _range - 1;
        _minusButton.interactable = _index > 0;
    }

    private void OnAdd()
    {
        Set(_index + 1);
    }

    private void OnMinus()
    {
        Set(_index - 1);
    }

    void Set(int v)
    {
        _index = Mathf.Clamp(v, 0, _range);
        Draw();

        ON_VALUE_CHANGE.Dispatch(Value);
    }

    public float Value
    {
        get
        {
            float v = (float)_index;
            return v.Remap(0, _range, rangeMin, rangeMax, true);
        }

        set
        {
            float v = value.Remap(rangeMin, rangeMax, 0, _range, true);
            Set(Mathf.RoundToInt(v));
        }
    }
}
