using Dreamteck.Splines;
using EyegazeCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrbController : MonoBehaviour
{
    private static List<Orb> _orbs;

    public UnityEvent ON_ORB_COLLECTED = new UnityEvent();

    [Range(0, 1)]
    [SerializeField]
    private float _orbCountMultiplyer = 1;

    [SerializeField]
    private Orb Prefab;

    public void GenerateOrbs(SplineComputer computer)
    {
        if (_orbs == null)
        {
            _orbs = new List<Orb>();
        }

        int orbCount = Mathf.RoundToInt((computer.CalculateLength() / 100)*_orbCountMultiplyer);
        int padding = 2;
        
        Debug.Log("init orbs: " + orbCount);

        for(int i = padding; i < orbCount-padding; i++)
        {
            Orb o = Instantiate<Orb>(Prefab, Vector3.zero, Quaternion.identity, this.transform);
            o.name = "Orb " + i.ToString();

            float div = 1 / (float)orbCount;
            float f = div * i;
            double dd = (double)(decimal)f;

            o.SplinePosition = dd;
            
            SplineResult result = computer.Evaluate(dd);
            
            Vector3 pos = result.position;
            o.computer = computer;
            o.transform.position = pos;
            
            o.transform.Translate(result.right * Random.Range(-8, 8));
            //o.motion.offset = new Vector2(Random.Range(-6, 6), 1);
            //o.position = dd;
            
            o.ON_HIT_BY_PLAYER.Add(OnOrbHit);

            _orbs.Add(o);
        }
    }
    
    void OnOrbHit(Orb o)
    {
        ON_ORB_COLLECTED.Invoke();
        o.gameObject.SetActive(false);

        SFX.PlayOneShot("orb_collect");
    }

    private void OnDestroy()
    {
        if (_orbs != null)
        {
            _orbs.Clear();
            _orbs = null;
        }
    }
    
    public List<Orb> orbs => _orbs;

    public int totalOrbs
    {
        get
        {
            return _orbs != null ? _orbs.Count : 0;
        }
        
    }



    public void Tick(Vector3 playerPosition, double percent)
    {
        foreach (Orb o in _orbs)
        {
            float dist = Vector3.Distance(playerPosition, o.transform.position);
        
            if (dist < 40)
            {
                o.Follow(playerPosition);
            }
            else
            {
                // o.Retreat();
                o.active = AbsoluteValueOf(percent - o.SplinePosition) < .075;
            }
        
              
        }
    }

    public double AbsoluteValueOf(double number)
    {
        double num = number;
        if (number < 0)
            num = -1 * number;
        return num;
    }


}
