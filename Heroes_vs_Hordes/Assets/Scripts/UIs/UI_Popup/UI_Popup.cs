using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Popup : UI_Base
{
    protected override void _SetCanvas()
    {
        Manager.Instance.UI.SetCanvas(gameObject, true);
    }

    public void ClosePopupUI()
    {
        Manager.Instance.UI.ClosePopupUI(this);
    }
}
