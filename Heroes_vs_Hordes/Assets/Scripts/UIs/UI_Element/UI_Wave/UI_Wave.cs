using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Wave : UI_Element
{
    protected int _elementIndex;

    /// <summary>
    /// ���̺� UI ��Ҹ� �ʱ�ȭ�� �� ȣ��
    /// </summary>
    /// <param name="elementIndex">UI ����� ����</param>
    /// <param name="parent">UI ����� �θ�</param>
    /// <param name="elementPosition">UI ����� ��ġ</param>
    /// <param name="elementSize">UI ����� ũ��</param>
    /// <param name="iconSize">UI ����� ������ ũ��</param>
    public virtual void InitWaveUIElement(int elementIndex, Transform parent, Vector2 elementPosition, Vector2 elementSize, Vector2 iconSize)
    {
        _elementIndex = elementIndex;
        transform.SetParent(parent);
        var rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = elementPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = elementSize;
    }

    /// <summary>
    /// ���̺� UI ��Ҹ� ������Ʈ�� �� ȣ��
    /// </summary>
    public abstract void UpdateWaveUIElement();

    /// <summary>
    /// ���̺� UI�� �ݳ��� �� ȣ��
    /// </summary>
    public abstract void ReturnWaveUI();
}
