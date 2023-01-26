using EyegazeCore;
using System.Collections.Generic;
using UnityEngine;

public static class NextTrackSetter
{

    public static void SetNextTrack(EyeDriveSession session, GameVO game)
    {
        // TODO if orb collect mode, get next unlocked track from one played

        // default to last unlocked track as it should have moved on after game
        List<int> nextTrack = GetNext(game);

        session.world = (World)nextTrack[0] - 1;
        session.track = nextTrack[1] - 1;
    }

    private static List<int> GetNext(GameVO game)
    {
        List<int> result = new List<int>();

            result.Add(game.zoneBest);
            result.Add(game.levelBest + 1);

            Debug.Log("Get next track:" + "Zone" + result[0] + "Track" + result[1]);
            return result;
    }

}
