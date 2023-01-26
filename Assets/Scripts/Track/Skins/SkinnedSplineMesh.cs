using Dreamteck.Splines;
using System;
using UnityEngine;
using static Dreamteck.Splines.SplineMesh;

[ExecuteInEditMode]
[RequireComponent(typeof(SplineMesh))]
public class SkinnedSplineMesh : MonoBehaviour
{
    private MeshRenderer _renderer;
    private SplineMesh _splineMesh;
    private float _splineLength;

    private Material[] _materials;

    public World skin = World.Space;


    public void ApplySkin()
    {
        _splineMesh = GetComponent<SplineMesh>();
        _renderer = GetComponent<MeshRenderer>();
        _splineLength = GetComponent<SplineComputer>().CalculateLength();

        Clear();

        switch (skin)
        {
            case World.Space:
                ApplySpaceSkin();
                break;

            case World.City:
                ApplyCitySkin();
                break;
        }

        _splineMesh.RebuildImmediate(false);
    }

    private void ApplySpaceSkin()
    {
        //add materials
        _materials = new Material[3];

        Material roadMaterial = Resources.Load<Material>(SkinAssets.SPACE_ROAD_MATERIAL);
        _materials[0] = roadMaterial;

        Material accessoriesMaterial = Resources.Load<Material>(SkinAssets.SPACE_ACCESSORIES_MATERIAL);
        _materials[1] = accessoriesMaterial;

        Material gantryMaterial = Resources.Load<Material>(SkinAssets.SPACE_GANTRY_MATERIAL);
        _materials[2] = gantryMaterial;

        _renderer.sharedMaterials = _materials;

        //add channels
        Channel road = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.SPACE_ROAD_A), "Road");
        road.type = Channel.Type.Extrude;
        road.count = Mathf.FloorToInt(_splineLength / 150);
        road.overrideMaterialID = true;
        road.targetMaterialID = 0;
        road.GetMesh(0).rotation = new Vector3(-90, 0, 0);

        Channel post = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.SPACE_POST_A), "Post");
        post.type = Channel.Type.Extrude;
        post.count = Mathf.FloorToInt(_splineLength / 150);
        post.overrideMaterialID = true;
        post.targetMaterialID = 1;
        post.GetMesh(0).rotation = new Vector3(-90, 0, 0);

        Channel intersection = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.SPACE_INTERSECTION_A), "Intersection");
        intersection.type = Channel.Type.Place;
        intersection.count = Mathf.FloorToInt(_splineLength / 500);
        intersection.overrideMaterialID = true;
        intersection.targetMaterialID = 1;
        intersection.GetMesh(0).rotation = new Vector3(-90, 0, 0);

        Channel overhead = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.SPACE_OVERHEAD_A), "Overhead");
        overhead.type = Channel.Type.Place;
        overhead.count = Mathf.FloorToInt(_splineLength / 300);
        overhead.randomOrder = true;
        overhead.randomSeed = 16;
        overhead.overrideMaterialID = true;
        overhead.targetMaterialID = 1;
        overhead.GetMesh(0).rotation = new Vector3(-90, 0, 0);

        /*Channel gantries = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.SPACE_GANTRY_A), "Gantries");
        gantries.type = Channel.Type.Place;
        gantries.count = Mathf.FloorToInt(_splineLength / 1000);
        gantries.randomOrder = true;
        gantries.randomSeed = 16;
        gantries.overrideMaterialID = true;
        gantries.targetMaterialID = 2;
        gantries.GetMesh(0).rotation = new Vector3(0, 0, 0);
        gantries.minScale = gantries.maxScale = new Vector3(10, 10, 10);*/
    }

    void ApplyCitySkin()
    {
        //add materials
        _materials = new Material[4];

        Material roadAMaterial = Resources.Load<Material>(SkinAssets.CITY_ROAD_A_MATERIAL);
        _materials[0] = roadAMaterial;

        Material roadBMaterial = Resources.Load<Material>(SkinAssets.CITY_ROAD_B_MATERIAL);
        _materials[1] = roadBMaterial;

        Material roadCMaterial = Resources.Load<Material>(SkinAssets.CITY_ROAD_C_MATERIAL);
        roadCMaterial.renderQueue = 2000;
        _materials[2] = roadCMaterial;

        Material intersectionMaterial = Resources.Load<Material>(SkinAssets.CITY_INTERSECTION_MATERIAL);
        _materials[3] = intersectionMaterial;

        _renderer.sharedMaterials = _materials;

        //add channels
        Channel roadA = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.CITY_ROAD_A), "RoadA");
        roadA.type = Channel.Type.Extrude;
        roadA.count = Mathf.FloorToInt(_splineLength / 150);
        roadA.overrideMaterialID = true;
        roadA.targetMaterialID = 0;
        roadA.GetMesh(0).rotation = new Vector3(-90, 0, 0);

        Channel roadB = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.CITY_ROAD_B), "RoadB");
        roadB.type = Channel.Type.Extrude;
        roadB.count = Mathf.FloorToInt(_splineLength / 150);
        roadB.overrideMaterialID = true;
        roadB.targetMaterialID = 1;
        roadB.GetMesh(0).rotation = new Vector3(-90, 0, 0);
        roadB.clipFrom = 0;
        roadB.clipTo = 0;

        Channel roadC = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.CITY_ROAD_C), "RoadC");
        roadC.type = Channel.Type.Extrude;
        roadC.count = Mathf.FloorToInt(_splineLength / 150);
        roadC.overrideMaterialID = true;
        roadC.targetMaterialID = 2;
        roadC.GetMesh(0).rotation = new Vector3(-90, 0, 0);
        roadC.clipFrom = 0;
        roadC.clipTo = 0;

        Channel intersection = _splineMesh.AddChannel(SkinAssets.GetMesh(SkinAssets.CITY_INTERSECTION), "Intersection");
        intersection.type = Channel.Type.Place;
        intersection.count = Mathf.FloorToInt(_splineLength / 300);
        intersection.overrideMaterialID = true;
        intersection.targetMaterialID = 3;
        intersection.GetMesh(0).rotation = new Vector3(-90, 0, 0); 
    }

    private void Clear()
    {
        //clear channels
        while (_splineMesh.GetChannelCount() > 0) _splineMesh.RemoveChannel(0);


       // Array.Clear(_renderer.sharedMaterials, 0, _renderer.sharedMaterials.Length);
    }
}
