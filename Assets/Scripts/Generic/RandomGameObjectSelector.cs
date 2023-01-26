using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomGameObjectSelector : MonoBehaviour
{
    private void Awake()
    {
        List<Transform> objects = GetComponentsInChildren<Transform>().ToList<Transform>();

        //remove self
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                objects.RemoveAt(i);
                break;
            }
        }

        int random = Random.Range(0, objects.Count - 1);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].gameObject.SetActive(i == random);
        }

    }
}
