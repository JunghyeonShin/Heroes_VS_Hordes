using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WavePanelTransform
{
    public Vector2 PanelPosition { get; set; }
    public Vector2 PanelSize { get; set; }
    public Vector2 IconSize { get; set; }
}

public abstract class UI_Wave : UI_Element
{
    protected int _elementIndex;

    /// <summary>
    /// Wave UI 요소를 초기화할 때 호출
    /// </summary>
    /// <param name="elementIndex">UI 요소의 목차</param>
    /// <param name="parent">UI 요소의 부모</param>
    /// <param name="elementPosition">UI 요소의 위치</param>
    /// <param name="elementSize">UI 요소의 크기</param>
    /// <param name="iconSize">UI 요소의 아이콘 크기</param>
    public virtual void InitWaveUI(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        _elementIndex = elementIndex;
        transform.SetParent(parent);
        var rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = elementPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = elementSize;
    }

    /// <summary>
    /// Wave UI 요소를 업데이트할 때 호출
    /// </summary>
    public abstract void UpdateWaveUI();

    /// <summary>
    /// Wave UI 요소를 애니메이션 처리를 통해 업데이트할 때 호출
    /// </summary>
    /// <param name="completeAnimationAction">애니메이션 완료 콜백</param>
    public abstract void UpdateWaveUIAnimation(Action completeAnimationCallback);

    /// <summary>
    /// Wave UI를 반납할 때 호출
    /// </summary>
    public abstract void ReturnWaveUI();
}
