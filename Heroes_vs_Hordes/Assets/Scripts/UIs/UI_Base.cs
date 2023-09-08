using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    private void Awake()
    {
        _Init();
    }

    private void OnEnable()
    {

    }

    protected abstract void _Init();

    protected abstract void _SetCanvas();

    protected void _BindEvent(GameObject go, Action action)
    {
        var eventHandler = Utils.GetOrAddComponent<UI_EventHandler>(go);
        eventHandler.OnClickHandler -= action;
        eventHandler.OnClickHandler += action;
    }

    protected void _BindButton(Type type)
    {
        _Bind<Button>(type);
    }

    protected void _BindGameObject(Type type)
    {
        _Bind<GameObject>(type);
    }

    protected void _BindSlider(Type type)
    {
        _Bind<Slider>(type);
    }

    protected void _BindText(Type type)
    {
        _Bind<TextMeshProUGUI>(type);
    }

    protected Button _GetButton(int index)
    {
        return _Get<Button>(index);
    }

    protected GameObject _GetGameObject(int index)
    {
        return _Get<GameObject>(index);
    }

    protected Slider _GetSlider(int index)
    {
        return _Get<Slider>(index);
    }

    protected TextMeshProUGUI _GetText(int index)
    {
        return _Get<TextMeshProUGUI>(index);
    }

    private void _Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int ii = 0; ii < names.Length; ii++)
        {
            if (typeof(GameObject) == typeof(T))
                objects[ii] = Utils.FindChild(gameObject, names[ii]);
            else
                objects[ii] = Utils.FindChild<T>(gameObject, names[ii]);

            if (objects[ii] == null)
                Debug.LogError($"Failed to bind({names[ii]})");
        }
    }

    private T _Get<T>(int index) where T : UnityEngine.Object
    {
        if (_objects.TryGetValue(typeof(T), out var objects))
            return objects[index] as T;
        return null;
    }
}
