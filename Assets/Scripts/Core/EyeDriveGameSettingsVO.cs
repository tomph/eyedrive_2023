using System;

[Serializable]
public class EyeDriveGameSettingsVO
{
    //[JsonProperty(Required = Required.Always)]
    public bool MUSIC_ON = true;

    //[JsonProperty(Required = Required.Always)]
    public bool sfx_on = true;

    public string server_url = "https://eyegazegamesbeta.openode.io/user/";
    public bool dev_version = true;

    public int[] progress = new int[] {
    0,0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,0,};

    public bool AI_CARS = true;
    public ModeType MODE = ModeType.Precise;
    public float STEERING_SENSITIVITY = 1f;
    public float SPEED = 0.5f;
    public bool CRASH_SFX = true;
}

public class StaticEyeDriveSettingsVO
{
    public static EyeDriveGameSettingsVO INSTANCE; //for now
}