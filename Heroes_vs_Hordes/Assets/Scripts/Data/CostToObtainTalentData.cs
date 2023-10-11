using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostToObtainTalentData
{
    private int _needGold;

    public int NeedGold { get { return _needGold; } }

    private const int INDEX_NEED_GOLD = 0;

    public CostToObtainTalentData(string[] splitData)
    {
        int.TryParse(splitData[INDEX_NEED_GOLD].TrimEnd(), out _needGold);
    }
}
