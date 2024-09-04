using System.Linq;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Configuration : ScriptableObject
{
    public static T Load<T>() where T : Configuration
    {
        return Resources.LoadAll<T>(@"_Configuration").FirstOrDefault();
    }

    public static ResourceRequest LoadAsync<T>() where T : Configuration
    {
        return Resources.LoadAsync<T>(@"_Configuration/" + typeof(T).Name.Replace("Configuration", ""));
    }

    public static Configuration Load(Type type)
    {
        return Resources.LoadAll(@"_Configuration", type).FirstOrDefault() as Configuration;
    }
}