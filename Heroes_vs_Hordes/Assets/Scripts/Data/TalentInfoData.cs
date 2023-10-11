using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentInfoData
{
    private float _ability;
    private string _description;

    public float Ability { get { return _ability; } }
    public string Description { get { return _description; } }

    private const int INDEX_ABILITY = 0;
    private const int INDEX_DESCRIPTION = 1;

    public TalentInfoData(string[] splitData)
    {
        float.TryParse(splitData[INDEX_ABILITY].TrimEnd(), out _ability);
        _description = splitData[INDEX_DESCRIPTION].TrimEnd();
    }
}
