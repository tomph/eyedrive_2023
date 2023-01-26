using System;
using EyegazeCore;
using UnityEngine;

public class PreciseOrbIndicator : MonoBehaviour
{
    [SerializeField]
    private LayerMask _rayMask;
    

    private Ray _ray;
    
    [SerializeField]
    private Transform _orb;
    
    [SerializeField]
    private Camera _cam;


    private void Awake()
    {
        _ray = new Ray();
    }

    private void Update()
    {
        if (_orb.gameObject.activeSelf == false) return;
        _ray = _cam.ScreenPointToRay(Input.mousePosition);
    }

    public void Tick(float delta, ModeType mode)
    {
        if (_orb.gameObject.activeSelf && (mode != ModeType.Precise && mode != ModeType.Mouse))
            _orb.gameObject.SetActive(false);
        
        if(_orb.gameObject.activeSelf == false && (mode == ModeType.Precise || mode == ModeType.Mouse))
            _orb.gameObject.SetActive(true);
        
        if (_orb.gameObject.activeSelf == false) return;

        //FOR NOW
        RaycastHit hit;

        float maxRayLength = 100;

        if (Physics.Raycast(_ray, out hit, maxRayLength, _rayMask))
        {
            Vector3 target = hit.point + (hit.normal * .5f);

            _orb.transform.position = Vector3.Lerp(_orb.transform.position, target, delta * 8);
        }
        else
        {
            for (int i = 0; i < maxRayLength; i += 1)
            {
                _ray = new Ray(_ray.origin + (_ray.direction * (maxRayLength - i)), Vector3.down);
                
                if (Physics.Raycast(_ray, out hit, 100, _rayMask))
                {
                    Vector3 target = hit.point + (hit.normal * .5f);

                    _orb.transform.position = Vector3.Lerp(_orb.transform.position, target, delta * 8);
                }
            }
        }
    }
}
