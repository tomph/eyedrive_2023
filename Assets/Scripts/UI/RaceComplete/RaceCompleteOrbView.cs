using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCompleteOrbView : MonoBehaviour
{
    private CanvasGroup _group;

    [SerializeField]
    private Text _collected;

    [SerializeField]
    private Text _total;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
       gameObject.SetActive(true);
    }

    public void Hide()
    {
       gameObject.SetActive(false);
    }

    public void Draw(int collected, int total)
    {
        _collected.text = collected.ToString() + " ORBS";
        _total.text = "/" + total.ToString();
    }
}
