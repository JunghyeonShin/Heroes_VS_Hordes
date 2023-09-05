using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (null == component)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject go, string name) where T : Object
    {
        if (null == go)
            return null;

        foreach (var component in go.GetComponentsInChildren<T>(true))
        {
            if (component.name.Equals(name))
                return component;
        }
        return null;
    }

    public static GameObject FindChild(GameObject go, string name)
    {
        var transform = FindChild<Transform>(go, name);
        if (null != transform)
            return transform.gameObject;
        return null;
    }
}
