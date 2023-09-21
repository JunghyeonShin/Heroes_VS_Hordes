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

    public void Init()
    {
        _LoadEssentialResource();
    }

    #region InitResource
    private bool[] _loadCompletes;

    private const int INDEX_TOTAL_VALUE = 3;
    private const int INDEX_SPRITE_SLIDER_YELLOW = 0;
    private const int INDEX_SPRITE_SLIDER_RED = 1;
    private const int INDEX_SPRITE_ICON_WEAPON_HERO_ARCANE_WAND = 2;

    public bool LoadComplete()
    {
        for (int ii = 0; ii < _loadCompletes.Length; ++ii)
        {
            if (false == _loadCompletes[ii])
                return false;
        }
        return true;
    }

    private void _LoadEssentialResource()
    {
        _loadCompletes = new bool[INDEX_TOTAL_VALUE];
        LoadAsync<Sprite>(Define.RESOURCE_SPRITES_SLIDER_YELLOW, (sprite) =>
        {
            _loadCompletes[INDEX_SPRITE_SLIDER_YELLOW] = true;
        });
        LoadAsync<Sprite>(Define.RESOURCE_SPRITES_SLIDER_RED, (sprite) =>
        {
            _loadCompletes[INDEX_SPRITE_SLIDER_RED] = true;
        });
        LoadAsync<Sprite>(Define.RESOURCE_SPRITES_ICON_WEAPON_HERO_ARCANE_WAND, (sprite) =>
        {
            _loadCompletes[INDEX_SPRITE_ICON_WEAPON_HERO_ARCANE_WAND] = true;
        });
    }
    #endregion

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
