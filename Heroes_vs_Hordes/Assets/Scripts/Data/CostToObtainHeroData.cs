using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostToObtainHeroData
{
    private int _needGold;

    public int NeedGold { get { return _needGold; } }

    private const int INDEX_NEED_GOLD = 0;

    public CostToObtainHeroData(string[] splitData)
    {
        int.TryParse(splitData[INDEX_NEED_GOLD], out _needGold);
    }
}
