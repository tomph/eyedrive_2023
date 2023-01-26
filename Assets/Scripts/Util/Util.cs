using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Util : MonoBehaviour
{

    private static Util _instance;
    public static Util instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject g = new GameObject();
                g.name = "Util";
                _instance = g.AddComponent<Util>();

            }
            return _instance;
        }
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float GetAnimationLength(Animator animator, string name)
    {
        if (animator != null)
        {
            int l = animator.runtimeAnimatorController.animationClips.Length;
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == name) return clip.length;
            }
            Debug.LogError("GetAnimationLength: clip " + name + " was not found!");
            return float.NaN;
        }
        Debug.LogError("GetAnimationLength: animator paramter was null!");
        return float.NaN;
    }

    public static string FormatTimeSS(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}", timeSpan.Seconds);
    }

    public static string FormatTimeMMSS(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        
    }

    public static string FormatTimeMMSSMSMS(float seconds = 0)
    {
        seconds = Mathf.Clamp(seconds, 0f, 10000f);
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        string formatted = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        return formatted.Remove(formatted.Length - 1);
    }

    public static void DeactivateGameObjectOnNextFrame(GameObject target)
    {
        if (target.activeSelf)
            instance.StartCoroutine(DoDeactivateGameObjectOnNextFrame(target));
    }

    static IEnumerator DoDeactivateGameObjectOnNextFrame(GameObject target)
    {
        yield return new WaitForEndOfFrame();
        target.SetActive(false);
    }

    public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
            component = gameObject.AddComponent<T>() as T;

        return component;

    }

    public static void EnsureDirectoryExists(string filePath)
    {
        FileInfo fi = new FileInfo(filePath);
        if (!fi.Directory.Exists)
        {
            Directory.CreateDirectory(fi.DirectoryName);
        }
    }

    public static void MoveChildrenToNewTransform(Transform fromTransform, Transform toTransform)
    {
        var tempList = fromTransform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            child.SetParent(toTransform);
        }
    }

    public static void RemoveChildrenFromTransform(Transform transform)
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject, true);
        }
    }

    public static void RemoveChildrenFromTransformOfType<T>(Transform transform)
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            if (child.GetComponent<T>() != null)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Converts two comma separated floats into a Vector2
    /// </summary>
    /// <param name="value">e.g. "1.5,2"</param>
    /// <returns></returns>
    public static Vector2 Vector2FromString(string value)
    {
        Debug.Log("Vector2FromString " + value);
        return new Vector2(float.Parse(value.Substring(0, value.IndexOf(","))), float.Parse(value.Substring(value.IndexOf(",") + 1)));
    }

    public static Vector3 Vector3FromString(string value)
    {
        string[] vals = value.Split(',');
        float x = float.Parse(vals[0]);
        float y = float.Parse(vals[1]);
        float z = float.Parse(vals[2]);

        return new Vector3(x, y, z);
    }

    public static string SanitizeFilePath(string filePath)
    {
        string modified = filePath;
        string invalid = new string(Path.GetInvalidPathChars()) + ":";

        foreach (char c in invalid)
        {
            modified = modified.Replace(c.ToString(), "");
        }
        return modified;
    }

    private static float sign(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public static bool PointInTriangle(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        bool b1, b2, b3;

        b1 = sign(pt, v1, v2) < 0.0f;
        b2 = sign(pt, v2, v3) < 0.0f;
        b3 = sign(pt, v3, v1) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p)
    {
        float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

        return determinant;
    }

    public class ClockwiseVector2Comparer : IComparer<Vector2>
    {
        public int Compare(Vector2 v1, Vector2 v2)
        {
            if (v1.x >= 0)
            {
                if (v2.x < 0)
                {
                    return -1;
                }
                return -Comparer<float>.Default.Compare(v1.y, v2.y);
            }
            else
            {
                if (v2.x >= 0)
                {
                    return 1;
                }
                return Comparer<float>.Default.Compare(v1.y, v2.y);
            }
        }
    }

    public static List<Vector2> GetConvexHull(List<Vector2> points)
    {
        //If we have just 3 points, then they are the convex hull, so return those
        if (points.Count == 3)
        {
            //These might not be ccw, and they may also be colinear
            return points;
        }

        //If fewer points, then we cant create a convex hull
        if (points.Count < 3)
        {
            return null;
        }

        Vector2[] arr = points.ToArray<Vector2>();
        Array.Sort(arr, new ClockwiseVector2Comparer());
        points = arr.ToList<Vector2>();


        //The list with points on the convex hull
        List<Vector2> convexHull = new List<Vector2>();

        //Step 1. Find the vertex with the smallest x coordinate
        //If several have the same x coordinate, find the one with the smallest z
        Vector2 startVertex = points[0];

        Vector3 startPos = startVertex;

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 testPos = points[i];

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.z < startPos.z))
            {
                startVertex = points[i];

                startPos = startVertex;
            }
        }

        //This vertex is always on the convex hull
        convexHull.Add(startVertex);

        points.Remove(startVertex);



        //Step 2. Loop to generate the convex hull
        Vector2 currentPoint = convexHull[0];

        //Store colinear points here - better to create this list once than each loop
        List<Vector2> colinearPoints = new List<Vector2>();

        int counter = 0;

        while (points.Count > 0)
        {
            //After 2 iterations we have to add the start position again so we can terminate the algorithm
            //Cant use convexhull.count because of colinear points, so we need a counter
            if (counter == 2)
            {
                points.Add(convexHull[0]);
            }

            //Pick next point randomly
            Vector2 nextPoint = points[UnityEngine.Random.Range(0, points.Count)];

            //To 2d space so we can see if a point is to the left is the vector ab
            Vector2 a = currentPoint;

            Vector2 b = nextPoint;

            //Test if there's a point to the right of ab, if so then it's the new b
            for (int i = 0; i < points.Count; i++)
            {
                //Dont test the point we picked randomly
                if (points[i].Equals(nextPoint))
                {
                    continue;
                }

                Vector2 c = points[i];

                //Where is c in relation to a-b
                // < 0 -> to the right
                // = 0 -> on the line
                // > 0 -> to the left
                float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

                //Colinear points
                //Cant use exactly 0 because of floating point precision issues
                //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
                float accuracy = 0.00001f;

                if (relation < accuracy && relation > -accuracy)
                {
                    colinearPoints.Add(points[i]);
                }
                //To the right = better point, so pick it as next point on the convex hull
                else if (relation < 0f)
                {
                    nextPoint = points[i];

                    b = nextPoint;

                    //Clear colinear points
                    colinearPoints.Clear();
                }
                //To the left = worse point so do nothing
            }



            //If we have colinear points
            if (colinearPoints.Count > 0)
            {
                colinearPoints.Add(nextPoint);

                //Sort this list, so we can add the colinear points in correct order
                colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n - currentPoint)).ToList();

                convexHull.AddRange(colinearPoints);

                currentPoint = colinearPoints[colinearPoints.Count - 1];

                //Remove the points that are now on the convex hull
                for (int i = 0; i < colinearPoints.Count; i++)
                {
                    points.Remove(colinearPoints[i]);
                }

                colinearPoints.Clear();
            }
            else
            {
                convexHull.Add(nextPoint);

                points.Remove(nextPoint);

                currentPoint = nextPoint;
            }

            //Have we found the first point on the hull? If so we have completed the hull
            if (currentPoint.Equals(convexHull[0]))
            {
                //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
                convexHull.RemoveAt(convexHull.Count - 1);

                break;
            }

            counter += 1;
        }

        return convexHull;
    }

    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public string FloatToTime(float toConvert, string format)
    {
        switch (format)
        {
            case "00.0":
                return string.Format("{0:00}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "#0.0":
                return string.Format("{0:#0}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "00.00":
                return string.Format("{0:00}:{1:00}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "00.000":
                return string.Format("{0:00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#00.000":
                return string.Format("{0:#00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#0:00":
                return string.Format("{0:#0}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
                break;
            case "#00:00":
                return string.Format("{0:#00}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
                break;
            case "0:00.0":
                return string.Format("{0:0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "#0:00.0":
                return string.Format("{0:#0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
                break;
            case "0:00.00":
                return string.Format("{0:0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "#0:00.00":
                return string.Format("{0:#0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
                break;
            case "0:00.000":
                return string.Format("{0:0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
            case "#0:00.000":
                return string.Format("{0:#0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                break;
        }
        return "error";
    }
}
