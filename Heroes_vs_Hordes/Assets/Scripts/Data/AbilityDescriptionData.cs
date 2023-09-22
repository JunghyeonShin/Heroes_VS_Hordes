using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDescriptionData
{
    private string _abilityName;
    private string _abilityLevel;
    private string _abilityDescription;

    public string AbilityName { get { return _abilityName; } }
    public string AbilityLevel { get { return _abilityLevel; } }
    public string AbilityDescription { get { return _abilityDescription; } }

    private const int INDEX_ABILITY_NAME = 0;
    private const int INDEX_ABILITY_LEVEL = 1;
    private const int INDEX_ABILITY_DESCRIPTION = 2;

    public AbilityDescriptionData(string[] splitData)
    {
        _abilityName = splitData[INDEX_ABILITY_NAME].TrimEnd();
        _abilityLevel = splitData[INDEX_ABILITY_LEVEL].TrimEnd();
        _abilityDescription = splitData[INDEX_ABILITY_DESCRIPTION].TrimEnd().Replace('\\', '\n');
    }
}
