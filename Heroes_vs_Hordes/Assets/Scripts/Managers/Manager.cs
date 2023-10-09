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

    private DataManager _data;
    private IngameManager _ingame;
    private ObjectManager _object;
    private ResourceManager _resource;
    private SaveDataManager _saveData;
    private UIManager _uI;

    public DataManager Data { get { return _data; } }
    public IngameManager Ingame { get { return _ingame; } }
    public ObjectManager Object { get { return _object; } }
    public ResourceManager Resource { get { return _resource; } }
    public SaveDataManager SaveData { get { return _saveData; } }
    public UIManager UI { get { return _uI; } }
    public CameraController CameraController { get; set; }

    private void Awake()
    {
        CreateInstance();

        _data = Utils.GetOrAddComponent<DataManager>(gameObject);
        _ingame = Utils.GetOrAddComponent<IngameManager>(gameObject);
        _object = new ObjectManager();
        _resource = new ResourceManager();
        _saveData = new SaveDataManager();
        _uI = new UIManager();

        _saveData.Init();
        _resource.Init();
        _data.Init();
        _object.Init();
        _uI.Init();
    }

    public bool LoadComplete()
    {
        if (false == _resource.LoadComplete())
            return false;
        if (false == _data.LoadComplete())
            return false;
        if (false == _object.LoadComplete())
            return false;
        if (false == _uI.LoadComplete())
            return false;
        return true;
    }
}
