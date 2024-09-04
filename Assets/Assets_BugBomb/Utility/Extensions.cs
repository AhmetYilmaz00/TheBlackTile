using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class MonoBehaviourExtensions
{
    public static void Invoke(this MonoBehaviour me, Action theDelegate, float time)
    {
        me.StartCoroutine(ExecuteAfterTime(theDelegate, time));
    }

    private static IEnumerator ExecuteAfterTime(Action theDelegate, float delay)
    {
        yield return new WaitForSeconds(delay);
        theDelegate();
    }

    public static void InvokeNextFrame(this MonoBehaviour me, Action theDelegate)
    {
        me.StartCoroutine(ExecuteNextFrame(theDelegate));
    }

    private static IEnumerator ExecuteNextFrame(Action theDelegate)
    {
        yield return null;
        theDelegate();
    }

    public static T FindObjectOfTypeAll<T>(this MonoBehaviour me, bool includeInactive = false)
    {
        List<T> results = new List<T>();
        SceneManager.GetActiveScene().GetRootGameObjects().ToList().ForEach(g => results.AddRange(g.GetComponentsInChildren<T>(includeInactive)));
        return results.Any() ? results[0] : default;
    }

    public static List<T> FindObjectsOfTypeAll<T>(this MonoBehaviour me, bool includeInactive = false)
    {
        List<T> results = new List<T>();
        SceneManager.GetActiveScene().GetRootGameObjects().ToList().ForEach(g => results.AddRange(g.GetComponentsInChildren<T>(includeInactive)));
        return results;
    }
}

public static class CollectionsExtensions
{
    public static T RandomElement<T>(this T[] table)
    {
        return table[Random.Range(0, table.Length)];
    }

    public static T RandomElementLimited<T>(this T[] table, int maxElement)
    {
        return table[Random.Range(0, maxElement)];
    }

    public static T RandomElementLimited<T>(this List<T> table, int maxElement)
    {
        return table[Random.Range(0, maxElement)];
    }

    public static T RandomElement<T>(this ICollection<T> table)
    {
        return table.ElementAt(Random.Range(0, table.Count()));
    }

    public static T RandomOtherElement<T>(this T[] table, ref int excludedIndex)
    {
        if (table.Length == 1)
            return table[0];

        int index;
        do
        {
            index = UnityEngine.Random.Range(0, table.Length);
        } while (index == excludedIndex);

        excludedIndex = index;

        return table[index];
    }

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static void FillWith<T>(this T[] table, T value)
    {
        for (int i = 0; i < table.Length; i++)
        {
            table[i] = value;
        }
    }
}

public static class TransformExtensions
{
    public static Transform[] BB_GetFirstLevelChildren(this Transform parent, bool includeInactive)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
        List<Transform> firstChildren = new List<Transform>();

        foreach (Transform child in children)
        {
            if (child.parent == parent)
            {
                firstChildren.Add(child);
            }
        }
        return firstChildren.ToArray();
    }
}

public static class TransformEx
{
    public static Transform BB_DestroyAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }

    public static Transform BB_DestroyImmediateAllChildren(this Transform transform)
    {
        var children = transform.BB_GetFirstLevelChildren(includeInactive: true);
        foreach (Transform child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        return transform;
    }
}

public static class TilemapExtensions
{
    // TileBase[] = tilemap.GetTiles<RoadTile>();
    public static T[] GetTiles<T>(this Tilemap tilemap) where T : TileBase
    {
        List<T> tiles = new List<T>();

        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                T tile = tilemap.GetTile<T>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    tiles.Add(tile);
                }
            }
        }
        return tiles.ToArray();
    }

    // int n = tilemap.GetNumberOfTiles();
    public static int GetNumberOfTiles(this Tilemap tilemap)
    {
        int counter = 0;

        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                if (tile != null)
                {
                    counter++;
                }
            }
        }

        return counter;
    }
}

public static class Vector3Extensions
{
    public static Vector3Int RoundVector3ToInt(this Vector3 v)
    {
        Vector3Int vi = new Vector3Int();
        vi.x = Mathf.RoundToInt(v.x);
        vi.y = Mathf.RoundToInt(v.y);
        vi.z = Mathf.RoundToInt(v.z);
        return vi;
    }

    public static Vector3Int RoundVector3ToIntWithMinOffset(this Vector3 v, Vector3Int prevTilePos, float minOffset)
    {
        Vector3Int vi = new Vector3Int();
        float off = 0;
        if (v.x - prevTilePos.x > 0.5f)
            off = -minOffset;
        else if (v.x - prevTilePos.x < -0.5f)
            off = minOffset;
        vi.x = Mathf.RoundToInt(v.x + off);

        vi.y = Mathf.RoundToInt(v.y);

        off = 0;
        if (v.z - prevTilePos.z > 0.5f)
            off = -minOffset;
        else if (v.z - prevTilePos.z < -0.5f)
            off = minOffset;
        vi.z = Mathf.RoundToInt(v.z + off);
        return vi;
    }
}