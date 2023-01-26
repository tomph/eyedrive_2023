using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GazeTickView : MonoBehaviour
{


    public float dwellTime = 1;

    public Image bgFill;
    public Image pieFill;
    public Image circleFill;

    public Color[] tintColours;


    private bool _colourTweening = false;
    private CanvasGroup _container;
    private RectTransform _circleTransForm;


    private bool _pie = true;

    public bool pie
    {
        get { return _pie; }
        set
        {
            _pie = value;

           
            pieFill.gameObject.SetActive(_pie);
            circleFill.gameObject.SetActive(!_pie);

            bgFill.color = _pie? tintColours[0] : Color.clear;
            

            _circleTransForm = Util.GetOrAddComponent<RectTransform>(circleFill.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _container = Util.GetOrAddComponent<CanvasGroup>(gameObject);
        _container.interactable = _container.blocksRaycasts = false;


        //put on to
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        _container.alpha = 0;
        StopColTween();
    }

    public void Draw(float perc, Vector3 dwellPosition)
    {
        transform.position = dwellPosition;

        if (perc == 0)
        {
            _container.alpha = 0;

            if (_pie)
            {
                pieFill.color = tintColours[1];
            }
            else
            {
                _circleTransForm.localScale = new Vector3(0, 0, 0);
            }
           
            
        }
        else
        {

            if (_pie)
            {
                ColTween();
                pieFill.fillAmount = perc;
            }
            else
            {
                _circleTransForm.localScale = new Vector3(1 - perc, 1 - perc, 1);
            }

            _container.alpha = 1;
           
        }
    }

    private void ColTween()
    {
        if (!_colourTweening)
        {
            pieFill.color = tintColours[1];
            iTween.ValueTo(
                gameObject,
                iTween.Hash("from", tintColours[1], "to", tintColours[3], "time", dwellTime, "easetype", "linear", "onUpdate", "UpdateColor")
            );
            _colourTweening = true;
        }
    }

    private void StopColTween()
    {
        if (_colourTweening)
        {
            iTween.Stop(gameObject);
            _colourTweening = false;
        }
    }

    public void UpdateColor(Color newColor)
    {

        pieFill.color = newColor;

    }
}
