using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    #region Instance
    private static Manager _instance;

    public static Manager Instance { get { return _instance; } }

    private const string NAME_MANAGER = "[MANAGER]";

    public static void CreateInstance()
    {
        if (null != _instance)
            return;

        _instance = FindObjectOfType<Manager>();
        if (null == _instance)
        {
            var go = new GameObject(NAME_MANAGER);
            _instance = Utils.GetOrAddComponent<Manager>(go);
        }
        DontDestroyOnLoad(_instance);
    }
    #endregion

    private IngameManager _ingame;
    private ResourceManager _resource;
    private UIManager _uI;

    public IngameManager Ingame { get { return _ingame; } }
    public ResourceManager Resource { get { return _resource; } }
    public UIManager UI { get { return _uI; } }

    private void Awake()
    {
        CreateInstance();

        _ingame = Utils.GetOrAddComponent<IngameManager>(gameObject);
        _resource = new ResourceManager();
        _uI = new UIManager();

        _uI.Init();
    }
}
