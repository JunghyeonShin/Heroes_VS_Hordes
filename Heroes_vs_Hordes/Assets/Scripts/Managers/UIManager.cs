using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private GameObject _rootUI;
    private int _sortingOrder = 10;

    private Dictionary<string, UI_Base> _uIDic = new Dictionary<string, UI_Base>();
    private Stack<UI_Popup> _currentPopupUIStack = new Stack<UI_Popup>();
    private Dictionary<string, ObjectPool> _uIElementDic = new Dictionary<string, ObjectPool>();

    public UI_Scene CurrentSceneUI { get; set; }

    private const int DEFAULT_SORTING_ORDER = 0;
    private const int EMPTY_VALUE = 0;
    private const string NAME_ROOT_UI = "[ROOT_UI]";

    public void Init()
    {
        _rootUI = new GameObject(NAME_ROOT_UI);
        _InstantiateEssentialUI();
    }

    #region InitUI
    private void _InstantiateEssentialUI()
    {
        _InstantiateUI<UI_PauseIngame>(Define.RESOURCE_UI_PAUSE_INGAME);
        _InstantiateUI<UI_ClearWave>(Define.RESOURCE_UI_CLEAR_WAVE);

        _InstantiateUIElement(Define.RESOURCE_UI_NORMAL_BATTLE_WAVE, 8);
        _InstantiateUIElement(Define.RESOURCE_UI_COIN_RUSH_WAVE, 2);
    }

    private void _InstantiateUI<T>(string key) where T : UI_Base
    {
        Manager.Instance.Resource.Instantiate(key, _rootUI.transform, (go) =>
        {
            var uI = Utils.GetOrAddComponent<T>(go);
            _uIDic.Add(key, uI);
            Utils.SetActive(go, false);
        });
    }

    private void _InstantiateUIElement(string key, int count)
    {
        var pool = new ObjectPool();
        Manager.Instance.Resource.LoadAsync<GameObject>(key, (elementUI) =>
        {
            pool.InitPool(elementUI, _rootUI, count);
            _uIElementDic.Add(key, pool);
        });
    }
    #endregion

    #region UI
    public T FindUI<T>(string key) where T : UI_Base
    {
        if (_uIDic.TryGetValue(key, out var uI))
            return uI as T;
        return null;
    }

    public void SetCanvas(GameObject go, bool sort)
    {
        var canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _sortingOrder;
            ++_sortingOrder;
        }
        else
            canvas.sortingOrder = DEFAULT_SORTING_ORDER;
    }

    public void ShowSceneUI<T>(string key, Action<T> callback = null) where T : UI_Scene
    {
        // 기존에 활성화 되어있는 UI들을 모두 닫음
        while (_currentPopupUIStack.Count > EMPTY_VALUE)
            _ClosePopupUI();
        if (null != CurrentSceneUI)
        {
            Utils.SetActive(CurrentSceneUI.gameObject, false);
            CurrentSceneUI = null;
        }

        // 캐시 확인
        if (_uIDic.TryGetValue(key, out var uI))
        {
            T sceneUI = uI as T;
            Utils.SetActive(sceneUI.gameObject, true);
            CurrentSceneUI = sceneUI;
            callback?.Invoke(sceneUI);
            return;
        }

        // UI 생성 후 캐싱
        Manager.Instance.Resource.Instantiate(key, _rootUI.transform, (go) =>
        {
            T sceneUI = Utils.GetOrAddComponent<T>(go);
            _uIDic.Add(key, sceneUI);
            CurrentSceneUI = sceneUI;
            callback?.Invoke(sceneUI);
        });
    }

    public void ShowPopupUI<T>(string key, Action<T> callback = null) where T : UI_Popup
    {
        // 캐시 확인
        if (_uIDic.TryGetValue(key, out var uI))
        {
            T popupUI = uI as T;
            Utils.SetActive(popupUI.gameObject, true);
            _currentPopupUIStack.Push(popupUI);
            callback?.Invoke(popupUI);
            return;
        }

        // UI 생성 후 캐싱
        Manager.Instance.Resource.Instantiate(key, _rootUI.transform, (go) =>
        {
            T popupUI = Utils.GetOrAddComponent<T>(go);
            _uIDic.Add(key, popupUI);
            _currentPopupUIStack.Push(popupUI);
            callback?.Invoke(popupUI);
        });
    }

    public void ClosePopupUI(UI_Popup popupUI)
    {
        if (_currentPopupUIStack.Count <= EMPTY_VALUE)
        {
            Debug.LogError("No open PopupUI!");
            return;
        }

        if (popupUI != _currentPopupUIStack.Peek())
        {
            Debug.LogError("Close PopupUI isn't match!");
            return;
        }

        _ClosePopupUI();
    }

    private void _ClosePopupUI()
    {
        var popupUI = _currentPopupUIStack.Pop();
        Utils.SetActive(popupUI.gameObject, false);
        --_sortingOrder;
    }
    #endregion

    #region UI_Element
    public GameObject GetElementUI(string key)
    {
        if (_uIElementDic.TryGetValue(key, out var pool))
            return pool.GetObject();
        return null;
    }

    public void ReturnElementUI(string key, GameObject elementUI)
    {
        if (_uIElementDic.TryGetValue(key, out var pool))
            pool.ReturnObject(elementUI);
    }
    #endregion
}
