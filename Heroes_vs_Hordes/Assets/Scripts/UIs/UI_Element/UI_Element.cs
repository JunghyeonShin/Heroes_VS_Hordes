using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Element : UI_Base
{
    protected int _elementIndex;

    protected override void _SetCanvas()
    {

    }

    /// <summary>
    /// UI 요소를 초기화할 때 호출
    /// </summary>
    /// <param name="elementIndex">UI 요소의 목차</param>
    /// <param name="parent">UI 요소의 부모</param>
    /// <param name="elementPosition">UI 요소의 위치</param>
    /// <param name="elementSize">UI 요소의 크기</param>
    /// <param name="iconSize">UI 요소의 아이콘 크기</param>
    public abstract void InitUIElement(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize);

    /// <summary>
    /// UI 요소를 업데이트할 때 호출
    /// </summary>
    public abstract void UpdateUIElement();
}
