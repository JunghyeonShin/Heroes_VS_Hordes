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
    private ObjectManager _object;
    private ResourceManager _resource;
    private UIManager _uI;

    public IngameManager Ingame { get { return _ingame; } }
    public ObjectManager Object { get { return _object; } }
    public ResourceManager Resource { get { return _resource; } }
    public UIManager UI { get { return _uI; } }

    private void Awake()
    {
        CreateInstance();

        _resource = new ResourceManager();
        _object = new ObjectManager();
        _uI = new UIManager();
        _ingame = Utils.GetOrAddComponent<IngameManager>(gameObject);

        _resource.Init();
        _object.Init();
        _uI.Init();
    }
}
