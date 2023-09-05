using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler
{
    public event Action OnClickHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke();
    }
}
