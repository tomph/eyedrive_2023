using System.Collections;
using System.Collections.Generic;
using Services.Constants;
using UnityEngine;
using UnityEngine.UI;

public class DeadzoneView : MonoBehaviour
{
    private CanvasGroup _cg;

    private Vector3 _scale = Vector3.zero;
    private float _alpha = 0;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();

        _scale = new Vector3(GameDefaults.DEAD_ZONE_NORMAL, 1, 1);
    }

    public void Draw(float value)
    {
        _alpha = .3f;
        _scale = new Vector3(value, 1, 1);
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _scale, Time.deltaTime * 5);
        _alpha = Mathf.Lerp(_alpha, 0, Time.deltaTime*4);
        _cg.alpha = _alpha;
    }
}
