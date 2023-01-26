using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinAssets : MonoBehaviour
{
    public static string SPACE_ROAD_A = "Skins/Space/Space_RoadA";
    public static string SPACE_POST_A = "Skins/Space/Space_PostA";
    public static string SPACE_RAIL_A = "Skins/Space/Space_RailA";
    public static string SPACE_LIGHT_A = "Skins/Space/Space_LightA";
    public static string SPACE_OVERHEAD_A = "Skins/Space/Space_OverheadA";
    public static string SPACE_INTERSECTION_A = "Skins/Space/Space_Intersection_A";
    public static string SPACE_GANTRY_A = "Skins/Space/Space_gantry_v05";
    public static string SPACE_ROAD_MATERIAL = "Skins/Space/Materials/Space_RoadA";
    public static string SPACE_ACCESSORIES_MATERIAL = "Skins/Space/Materials/Space_RoadA_accessories";
    public static string SPACE_GANTRY_MATERIAL = "Skins/Space/Materials/Space_Gantry";

    public static string CITY_ROAD_A = "Skins/City/City_Road_A_low";
    public static string CITY_ROAD_B = "Skins/City/city_road_B_low_s_v2";
    public static string CITY_ROAD_C = "Skins/City/city_road_C_low_s_v2";
    public static string CITY_ROAD_TUNNEL = "Skins/City/City_tunnel_low";
    public static string CITY_INTERSECTION = "Skins/City/city_intersection_low_s_v2";
    public static string CITY_ROAD_A_MATERIAL = "Skins/City/Materials/City_Road_A";
    public static string CITY_ROAD_B_MATERIAL = "Skins/City/Materials/City_Road_B";
    public static string CITY_ROAD_C_MATERIAL = "Skins/City/Materials/City_Road_C";
    public static string CITY_INTERSECTION_MATERIAL = "Skins/City/Materials/City_Intersection";


    public static Mesh GetMesh(string asset)
    {
        Debug.Log("get mesh: " + asset);
        return Resources.Load<MeshFilter>(asset).sharedMesh;
    }

}
