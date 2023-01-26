
using Services.Constants;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsController : MonoBehaviour
{
    [SerializeField] private Toggle _aiCarsToggle;

    [SerializeField] private Toggle _musicToggle;

    [SerializeField] private Toggle _crashSFXToggle;

    [SerializeField] private Toggle _lanesToggle;

    [SerializeField] private Toggle _slowSteerToggle;
    
    [SerializeField] private Toggle _slowSpeedToggle;

    private EyeDriveGameSettingsVO _settings;
    

    public void Init(EyeDriveGameSettingsVO data, EyeDriveSession session)
    {
        _settings = data;

        _aiCarsToggle.isOn = data.AI_CARS;
        _musicToggle.isOn = !session.GetUser(0).musicMuted;
        _lanesToggle.isOn = (ModeType)data.MODE == ModeType.Lanes;
        _slowSteerToggle.isOn = data.STEERING_SENSITIVITY == GameDefaults.STEERING_SENSITIVITY_SLOW; 
        _slowSpeedToggle.isOn = data.SPEED == GameDefaults.SPEED_SLOW;
    }

    public Toggle.ToggleEvent ON_AI_CARS_TOGGLE => _aiCarsToggle.onValueChanged;
    public Toggle.ToggleEvent ON_CRASH_SFX_TOGGLE => _crashSFXToggle.onValueChanged;
    public Toggle.ToggleEvent ON_SLOW_STEER_TOGGLE => _slowSteerToggle.onValueChanged;
    public Toggle.ToggleEvent ON_SLOW_SPEED_TOGGLE => _slowSpeedToggle.onValueChanged;


}