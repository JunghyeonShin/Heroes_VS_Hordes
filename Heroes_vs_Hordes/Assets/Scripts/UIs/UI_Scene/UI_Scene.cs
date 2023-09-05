using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Scene : UI_Base
{
    protected override void _SetCanvas()
    {
        Manager.Instance.UI.SetCanvas(gameObject, false);
    }
}
