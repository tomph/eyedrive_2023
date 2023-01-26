
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EyeDrive/World", order = 8)]
public class EyeDriveWorld : ScriptableObject
{
    public List<TrackController> tracks;
    public List<Sprite> thumbs;

    public Material skybox;
    public GameObject lights;
    public Cubemap cubemap;
    public Sprite screenshot;
}
