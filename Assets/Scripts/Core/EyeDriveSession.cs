using EyegazeCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "EyeDrive/Session", order = 7)]
public class EyeDriveSession : ScriptableObject, ISession
{
    public UserVO user;
    private GameVO _game;

  
    public World world;
    public GameType gameType;
    public PlayType playType;

    public GameState state;
    public int ship;
    public int track;
    public float bestTime;
    public float lapTime;
    public int lap;
    public int orbs;
    public int totalOrbs = 0;
    

    public List<float> lapTimes;

    public UserVO GetActiveUser()
    {
        return user;
    }

    public void SetUser(UserVO data, int id)
    {
        user = data;
    }

    public UserVO GetUser(int id)
    {
        return user;
    }

    public void Reset()
    {
        lapTimes = new List<float>();
        ship = 0;
        orbs = 0;
        lap = 0;
        totalOrbs = 0;
        world = World.Space;
        track = 0;
        
        state = GameState.Countdown;
        bestTime = 0;
    }

    public GameVO GetGame()
    {
        return _game;
    }

    public void SetGame(GameVO data)
    {
        _game = data;
    }
}

public class StaticEyeDriveSession
{
    public static EyeDriveSession INSTANCE; //for now
}