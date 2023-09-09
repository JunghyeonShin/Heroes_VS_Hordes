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
    /// Wave UI ��Ҹ� �ʱ�ȭ�� �� ȣ��
    /// </summary>
    /// <param name="elementIndex">UI ����� ����</param>
    /// <param name="parent">UI ����� �θ�</param>
    /// <param name="elementPosition">UI ����� ��ġ</param>
    /// <param name="elementSize">UI ����� ũ��</param>
    /// <param name="iconSize">UI ����� ������ ũ��</param>
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
    /// Wave UI ��Ҹ� ������Ʈ�� �� ȣ��
    /// </summary>
    public abstract void UpdateWaveUI();

    /// <summary>
    /// Wave UI ��Ҹ� �ִϸ��̼� ó���� ���� ������Ʈ�� �� ȣ��
    /// </summary>
    /// <param name="completeAnimationAction">�ִϸ��̼� �Ϸ� �ݹ�</param>
    public abstract void UpdateWaveUIAnimation(Action completeAnimationCallback);

    /// <summary>
    /// Wave UI�� �ݳ��� �� ȣ��
    /// </summary>
    public abstract void ReturnWaveUI();
}
