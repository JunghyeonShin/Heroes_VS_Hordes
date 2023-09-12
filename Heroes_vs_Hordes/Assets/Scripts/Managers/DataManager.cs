using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public HeroAbility HeroCommonAbility { get; private set; }
    public Dictionary<string, HeroAbility> HeroIndividualAbility { get; private set; }

    private const int HERO_ABILITY_KEY = 0;
    private const string URL_HERO_COMMON_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&range=A2:H";
    private const string URL_HERO_INDIVIDUAL_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&gid=915946808&range=A2:H";

    public void Init()
    {
        _loadHeroCommonAbility().Forget();
        _LoadHeroIndividualAbility().Forget();
    }

    #region Load Hero Ability
    private async UniTaskVoid _loadHeroCommonAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_HERO_COMMON_ABILITY);
        var op = await webRequest.SendWebRequest();

        var splitRawData = op.downloadHandler.text.Split('\t');
        HeroCommonAbility = _LoadHeroAbility(splitRawData);
    }

    private async UniTaskVoid _LoadHeroIndividualAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_HERO_INDIVIDUAL_ABILITY);
        var op = await webRequest.SendWebRequest();

        HeroIndividualAbility = new Dictionary<string, HeroAbility>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            HeroIndividualAbility.Add(splitData[HERO_ABILITY_KEY], _LoadHeroAbility(splitData));
        }
    }

    private HeroAbility _LoadHeroAbility(string[] splitData)
    {
        float.TryParse(splitData[Define.HERO_ABILITY_HEALTH].TrimEnd(), out var health);
        float.TryParse(splitData[Define.HERO_ABILITY_DEFENCE].TrimEnd(), out var defense);
        float.TryParse(splitData[Define.HERO_ABILITY_ATTACK].TrimEnd(), out var attack);
        float.TryParse(splitData[Define.HERO_ABILITY_ATTACK_COOLDOWN].TrimEnd(), out var attackCooldown);
        float.TryParse(splitData[Define.HERO_ABILITY_CRITICAL].TrimEnd(), out var critical);
        float.TryParse(splitData[Define.HERO_ABILITY_MOVE_SPEED].TrimEnd(), out var moveSpeed);
        float.TryParse(splitData[Define.HERO_ABILITY_PROJECTILE_SPEED].TrimEnd(), out var projectileSpeed);

        return new HeroAbility(health, defense, attack, attackCooldown, critical, moveSpeed, projectileSpeed);
    }
    #endregion
}
