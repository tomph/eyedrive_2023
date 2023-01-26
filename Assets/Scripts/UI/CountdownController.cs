using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    public Signal ON_COUNTDOWN_COMPLETE = new Signal();

    [SerializeField]
    private Text _textField;

    private void Awake()
    {
        CommenceCountdown();
        _textField.text = "3";
    }

    public void CommenceCountdown()
    {
        GetComponent<Animator>().Play("Countdown");
    }

    public void OnCountdownComplete()
    {
        ON_COUNTDOWN_COMPLETE.Dispatch();
    }

    public void OnThree()
    {
        _textField.text = "3";
    }

    public void OnTwo()
    {
        _textField.text = "2";

    }

    public void OnOne()
    {
        _textField.text = "1";
    }

    public void OnZero()
    {
        _textField.text = "GO!";
    }
}
