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
    /// UI ��Ҹ� �ʱ�ȭ�� �� ȣ��
    /// </summary>
    /// <param name="elementIndex">UI ����� ����</param>
    /// <param name="parent">UI ����� �θ�</param>
    /// <param name="elementPosition">UI ����� ��ġ</param>
    /// <param name="elementSize">UI ����� ũ��</param>
    /// <param name="iconSize">UI ����� ������ ũ��</param>
    public abstract void InitUIElement(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize);

    /// <summary>
    /// UI ��Ҹ� ������Ʈ�� �� ȣ��
    /// </summary>
    public abstract void UpdateUIElement();
}
