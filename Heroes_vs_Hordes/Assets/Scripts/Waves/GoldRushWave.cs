using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRushWave : Wave
{
    public override void StartWave()
    {

    }

    public override void ClearWave()
    {
        Manager.Instance.UI.ShowPopupUI<UI_ClearChapter>(Define.RESOURCE_UI_CLEAR_CHAPTER);
    }

    protected override void _ProgressWaveTime()
    {

    }
}
