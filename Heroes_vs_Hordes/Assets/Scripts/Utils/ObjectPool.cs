using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> _poolQueue = new Queue<GameObject>();
    private GameObject _prefab;
    private GameObject _parent;

    private const int EMPTY_VALUE = 0;

    public void InitPool(GameObject prefab, GameObject parent, int count)
    {
        _prefab = prefab;
        _parent = parent;

        for (int ii = 0; ii < count; ++ii)
            _poolQueue.Enqueue(_CreateObject());
    }

    public GameObject GetObject()
    {
        if (_poolQueue.Count > EMPTY_VALUE)
            return _poolQueue.Dequeue();
        else
            return _CreateObject();
    }

    public void ReturnObject(GameObject go)
    {
        Utils.SetActive(go, false);
        _poolQueue.Enqueue(go);
    }

    private GameObject _CreateObject()
    {
        var go = GameObject.Instantiate(_prefab);
        if (null != _parent)
            go.transform.SetParent(_parent.transform);
        Utils.SetActive(go, false);
        return go;
    }
}
