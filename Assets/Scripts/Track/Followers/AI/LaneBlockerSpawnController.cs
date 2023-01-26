using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneBlockerSpawnController : MonoBehaviour
{
    [SerializeField]
    private SplineComputer _computer;

    [SerializeField]
    private LaneBlocker _prefabToSpawn;

    [SerializeField]
    private float _laneWidth;

    [SerializeField]
    private float _laneSwitchFrequency;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private double _spread;

    private void Awake()
    {

    }
}
