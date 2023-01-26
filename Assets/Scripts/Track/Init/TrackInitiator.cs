
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TrackInitiator : SerializedMonoBehaviour
{
    [SerializeField]
    private TrackController track;

    [SerializeField]
    private Dictionary<World, EyeDriveWorld> _worlds;
    
    public TrackController Init(EyeDriveSession session, EyeDriveGameSettingsVO settings)
    {
        EyeDriveWorld world = _worlds[session.world];

        //clear
        track = Instantiate<TrackController>(world.tracks[session.track], transform, false).Init(settings, session);

        GameObject lights = Instantiate<GameObject>(world.lights, transform);
        RenderSettings.skybox = Instantiate<Material>(world.skybox);
        RenderSettings.customReflection = Instantiate<Cubemap>(world.cubemap);

        return track;
    }
}
