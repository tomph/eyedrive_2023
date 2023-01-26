using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private AIFollower[] Prefabs;

    private List<AIFollower> _followers = new List<AIFollower>();
    
    public void Init(SplineComputer computer)
    {

        int count = Mathf.Clamp(Mathf.RoundToInt(computer.CalculateLength() / 450), 0, 4);
        int padding = 1;
        
        for (int i = padding; i < count - padding; i++)
        {
            AIFollower ai = Instantiate<AIFollower>(Prefabs[Random.Range(0, Prefabs.Length-1)], Vector3.zero, Quaternion.identity, this.transform);
            ai.name = "AIFollower " + i.ToString();
            ai.computer = computer;

            float div = 1 / (float)count;
            float f = div * i;
            double percent = (double)(decimal)f;

            ai.Init(computer, percent);
            
            _followers.Add(ai);
        }
    }

    public void Tick(float delta)
    {
        for (int i = 0; i < _followers.Count; i++)
        {
            _followers[i].Tick(delta);
        }
    }

    public void Pause(float delta)
    {
        for (int i = 0; i < _followers.Count; i++)
        {
            _followers[i].Pause();
        }
    }
}
