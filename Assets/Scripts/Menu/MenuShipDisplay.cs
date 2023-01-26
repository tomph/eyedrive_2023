using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShipDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject[] ships;

    private int _index = 0;
    
    public void SetShip(int index)
    {
        _index = index;

        for (int i = 0; i < ships.Length; i++)
        {
            if (_index == i)
            {
                ships[i].gameObject.SetActive(true);
            }
            else
            {
                ships[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ships[_index].transform.Rotate(0, Time.deltaTime * 20, 0);   
    }
}
