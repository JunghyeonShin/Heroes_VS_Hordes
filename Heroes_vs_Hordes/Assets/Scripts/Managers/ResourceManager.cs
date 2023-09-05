using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    // ���ҽ��� �ڵ� ĳ��
    private Dictionary<string, UnityEngine.Object> _resourceDic = new Dictionary<string, UnityEngine.Object>();
    private Dictionary<string, AsyncOperationHandle> _handleDic = new Dictionary<string, AsyncOperationHandle>();

    public void LoadAsync<T>(string key, Action<T> callback) where T : UnityEngine.Object
    {
        // ĳ�� Ȯ��
        if (_resourceDic.TryGetValue(key, out var resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        // ���ҽ��� �ε� ���� ���� �ݹ鸸 �߰�
        if (_handleDic.ContainsKey(key))
        {
            _handleDic[key].Completed += (resource) =>
            {
                callback?.Invoke(resource.Result as T);
            };
            return;
        }

        // �ε�
        _handleDic.Add(key, Addressables.LoadAssetAsync<T>(key));
        _handleDic[key].Completed += (resource) =>
        {
            _resourceDic.Add(key, resource.Result as UnityEngine.Object);
            callback?.Invoke(resource.Result as T);
        };
    }

    public void Release(string key)
    {
        if (false == _resourceDic.ContainsKey(key))
            return;

        _resourceDic.Remove(key);

        if (_handleDic.TryGetValue(key, out var handle))
            Addressables.Release(handle);
        _handleDic.Remove(key);
    }

    public void Instantiate(string key, Transform parent, Action<GameObject> callback)
    {
        LoadAsync<GameObject>(key, (prefab) =>
        {
            var go = GameObject.Instantiate(prefab, parent);
            callback?.Invoke(go);
        });
    }
}
