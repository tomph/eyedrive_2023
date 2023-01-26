using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGaze
{
    void OnGazeComplete();
    void OnPointerEnter(PointerEventData pointerEventData);
    void OnPointerExit(PointerEventData pointerEventData);

}
