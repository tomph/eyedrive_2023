//This script handles the audio and visual effects (i.e., cosmetics) for the ship. 

using EyegazeCore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAVFX : MonoBehaviour
{
    public ParticleSystem[] thrusters;

    public GameObject lightTrails;          //The game object holding the light trail renderers
    public ParticleSystem wallGrind;        //The wall grind particles
    public float minTrailSpeed = 33f;       //The speed needed to enable the trails
    public Light exhaustLight;              //The light that appears from thrusters
    public Color thrustColor;               //The color of the thruster light
    public Color brakeColor;                //The color of the brake light
    
    public bool player = false;             //is this a player or AI car

    ParticleSystem.MainModule mainModule;   //A module for storing and changing the thruster particles

    float thrusterStartLife;                //The start life that the thrusters normally have
    float lightStartIntensity;				//The intensity that the exhaust light normally has

    private BaseFollower _follower;
    private BaseFollowerAI _followerAI;

    private float _velocity = 0;
    private bool _braking = false;

    [Header("Sounds")]
    public string engineSoundKey;

    private AudioSource _sparksSound;
    private AudioSource _engineSound;
    private AudioSource _accelerateSound;
    private AudioSource _breakSound;

    private float _accelerateCooldownVolume = 1;
    private float _fadeVolume = 0;
    private float _bumpCooldown = 0;
    private bool _lastThrusterState = false;
    private bool _lastBrakeState = false;

    private List<string> _wallLowImpactSounds = new List<string>();
    private List<string> _wallHighImpactSounds = new List<string>();
    private List<string> _carImpactSounds = new List<string>();

    //The Reset method is called by Unity whenever a script it added to an object or when a component
    //is reset in the inspector. This is a good place to put initialization code since it happens in
    //the editor and won't cause performance decreases at runtime. Putthing this here means that we 
    //won't have to drag a bunch of items onto the script in the inspector
    void Reset()
    {
        _follower = GetComponent<BaseFollower>();

        //References and values for the various ship parts
        thrustColor = new Color(.3f, .6f, .8f);
        brakeColor = new Color(1f, 0f, 0f);
    }

    void Start()
    {
        _follower = GetComponent<BaseFollower>();
        if (_follower is PreciseFollower)
        {
            PreciseFollower m = _follower as PreciseFollower;
            m.ON_BUMP.Add(OnBump);
        }

        _followerAI = GetComponent<BaseFollowerAI>();

        //Record the thruster's particle start life property
        if (thrusters.Length > 0)
        {
            mainModule = thrusters[0].main;
            thrusterStartLife = mainModule.startLifetime.constant;
        }
        

        //Record the exhaust light's starting intensity and then set it to 0
        lightStartIntensity = exhaustLight.intensity;
        exhaustLight.intensity = 0f;

        //Disable the light trails
        lightTrails.SetActive(false);

        //Stop the wall grind particles of they happen to be playing
        wallGrind.Stop();

        //init sounds

        _engineSound = SFX.GetSource(engineSoundKey, true);
        if (player)
        {
            _engineSound.spatialBlend = 0;
            _engineSound.reverbZoneMix = 0;
            _engineSound.bypassEffects = true;
            _engineSound.bypassReverbZones = true;
            _engineSound.bypassListenerEffects = true;
        }
        _engineSound.reverbZoneMix = 0;
        _engineSound.Play();
        _engineSound.loop = true;

        _sparksSound = SFX.GetSource("Eyedrive_SFX_In Game_Wall Grinding_v.1 (loop)", true);
        _accelerateSound = SFX.GetSource("Eyedrive_SFX_In Game_Boost_Accelerate_v.1", false);
        _breakSound = SFX.GetSource("Eyedrive_SFX_In Game_Break (long)_v.1", false);


        _wallLowImpactSounds.Add("Eyedrive_SFX_In Game_C2W_Low Speed Impact 1_v.1");
        _wallLowImpactSounds.Add("Eyedrive_SFX_In Game_C2W_Low Speed Impact 2_v.1");

        _wallHighImpactSounds.Add("Eyedrive_SFX_In Game_C2W_High Speed Impact 1_v.1");
        _wallHighImpactSounds.Add("Eyedrive_SFX_In Game_C2W_High Speed Impact 2_v.1");
        _wallHighImpactSounds.Add("Eyedrive_SFX_In Game_C2W_High Speed Impact 3_v.1");

        _carImpactSounds.Add("Eyedrive_SFX_In Game_C2C_Impact 1_v.1");
        _carImpactSounds.Add("Eyedrive_SFX_In Game_C2C_Impact 2_v.1");
        _carImpactSounds.Add("Eyedrive_SFX_In Game_C2C_Impact 3_v.1");

        _fadeVolume = 0;
    }

    private float speed
    {
        get
        {
            return _follower? _follower.speed : _followerAI.speed;
        }
    }

    private float velocity
    {
        get
        {
            return _velocity; 
        }
    }

    public void Tick(float delta, float velocity, bool braking = false)
    {
        _velocity = velocity;
        _braking = braking;
        
        //Update the thruster particles and light
        if(thrusters.Length>0) UpdateThrusterParticles(delta);
        UpdateExhaustLight(delta);
        UpdateSounds(delta, velocity);

        //If the ship is going faster than the minimum trail speed then enable them...
        if (speed > minTrailSpeed)
        {
            if (lightTrails.activeSelf == false)
            {
                lightTrails.SetActive(true);
            }
        }
        else
        {
            lightTrails.SetActive(false);
        }

        //Get the percentage of speed the ship is traveling
        float speedPercent = speed * 100;

        if (StaticEyeDriveSession.INSTANCE.state == GameState.Paused) wallGrind.Stop();
    }

    private void UpdateSounds(float delta, float velocity)
    {
        _fadeVolume = Mathf.Clamp01(Mathf.Lerp(_fadeVolume, velocity/50f, delta));

        _bumpCooldown -= delta;

        float semitones = _fadeVolume.Remap(0, 1, 0, 12, true);
        if (StaticEyeDriveSession.INSTANCE.state == GameState.Paused || StaticEyeDriveSession.INSTANCE.state == GameState.RaceComplete) semitones = 0;

        _engineSound.pitch = Mathf.Lerp(_engineSound.pitch, Mathf.Pow(1.05946f, semitones), delta * 2);

        float vol = StaticEyeDriveSession.INSTANCE.state == GameState.Paused || StaticEyeDriveSession.INSTANCE.state == GameState.RaceComplete ? .2f : _fadeVolume;
        _engineSound.volume = Mathf.Lerp(_engineSound.volume, vol, delta * 2);

        float vol2 = StaticEyeDriveSession.INSTANCE.state == GameState.Paused || StaticEyeDriveSession.INSTANCE.state == GameState.RaceComplete ? 0f : wallGrind.isPlaying ? .5f : 0;
        _sparksSound.volume = vol2;
    }

    void OnCollisionEnter(Collision collision)
    {
        float force = collision.relativeVelocity.magnitude;
        
        //collided with track
        if (collision.gameObject.layer == LayerMask.NameToLayer("AI"))
        {
            if (force > 0 && StaticEyeDriveSettingsVO.INSTANCE.CRASH_SFX)
            {
                SFX.PlayRandomOneShot(_carImpactSounds);
            }
        }
    }

    //Called then the ship collides with something solid
    void OnCollisionStay(Collision collision)
    {

        //If the ship did not collide with a wall then exit
        if (collision.gameObject.layer != LayerMask.NameToLayer("Spline") || StaticEyeDriveSession.INSTANCE.state == GameState.Paused)
            return;

        //Move the wallgrind particle effect to the point of collision and play it
        wallGrind.transform.position = collision.contacts[0].point;
        wallGrind.Play(true);
    }

    //Called when the ship stops colliding with something solid
    void OnCollisionExit(Collision collision)
    {
        //Stop playing the wallgrind particles
        wallGrind.Stop(true);
    }

    void UpdateThrusterParticles(float delta)
    {
        //Calculate the lifetime we want the thruster's particles to have
        float currentLifeTime = thrusterStartLife * Mathf.Clamp(Mathf.Clamp01(velocity/100f) * 2, 0, 1);

        //If the thrusters are powered on at all...
        if (currentLifeTime > 0f)
        {

            foreach (ParticleSystem thruster in thrusters)
            {
                thruster.Play();
                mainModule = thruster.main;
                mainModule.startLifetime = currentLifeTime;
            }

            
            OnThrusterUpdate(delta,true);

        }
        //...Otherwise stop the particle effects
        else
        {
            foreach (ParticleSystem thruster in thrusters)
            {
                thruster.Stop();
                OnThrusterUpdate(delta, false);
            }
        }
    }

    void OnThrusterUpdate(float delta, bool state)
    {
        if (state == true && _lastThrusterState == false)
        {
            _accelerateSound.volume = _accelerateCooldownVolume;
            _accelerateSound.Stop();
            _accelerateSound.Play();

            _accelerateCooldownVolume = 0;
        }

        _accelerateCooldownVolume = Mathf.Clamp(_accelerateCooldownVolume + delta / 3, 0, 1);

        _lastThrusterState = state;
    }

    void UpdateExhaustLight(float delta)
    {
        //Calculate the intensity we want the light to have
        float currentIntensity = lightStartIntensity * Mathf.Clamp01(velocity/100f); 

        //If the ship is moving forward and not braking...
        if (_braking == false)
        {
            //... set the light's color and intensity
            exhaustLight.color = thrustColor;
            exhaustLight.intensity = currentIntensity;

            OnBrakeUpdate(delta, false);

        }
        //...Otherwise...
        else
        {
            //...set the color to the brake color...
            exhaustLight.color = brakeColor;
            //...and give it max intensity
            exhaustLight.intensity = lightStartIntensity;

            OnBrakeUpdate(delta, true);
        }
    }

    private void OnBrakeUpdate(float delta, bool state)
    {
        if (state == true && _lastBrakeState == false)
        {
            _breakSound.Stop();
            _breakSound.Play();
        }
        else if (state == false && _breakSound.isPlaying)
        {
            _breakSound.Stop();
            _breakSound.volume = 0;
        }

        _breakSound.volume = Mathf.Clamp(_breakSound.volume + delta/ 2, 0, 1);
        _lastBrakeState = state;
    }


    public void OnBump(float mag)
    {

        if (_bumpCooldown < 0 )
        {
            if (StaticEyeDriveSettingsVO.INSTANCE.CRASH_SFX)
            {
                if (mag < 12)
                {
                    SFX.PlayRandomOneShot(_wallLowImpactSounds);
                }
                else
                {
                    SFX.PlayRandomOneShot(_wallHighImpactSounds);
                }
            }

            _bumpCooldown = .6f;
        }

    }
}
