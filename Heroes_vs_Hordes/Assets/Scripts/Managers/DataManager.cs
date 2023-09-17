using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    private bool[] _loadCompletes;

    public HeroAbility HeroCommonAbility { get; private set; }
    public Dictionary<string, HeroAbility> HeroIndividualAbilityDic { get; private set; }
    public List<float> RequiredExpList { get; private set; }
    public List<ChapterInfo> ChapterInfoList { get; private set; }
    public Dictionary<string, WeaponAbility> WeaponAbilityDic { get; private set; }
    public Dictionary<string, List<WeaponAbility>> WeaponLevelAbilityDic { get; private set; }
    public Dictionary<string, List<float>> BookAbilityDic { get; private set; }

    private const int INDEX_LOAD_TOTAL_VALUE = 7;
    private const int INDEX_LOAD_HERO_COMMON_ABILITY = 0;
    private const int INDEX_LOAD_HERO_INDIVIDUAL_ABILITY = 1;
    private const int INDEX_LOAD_REQUIRED_EXP = 2;
    private const int INDEX_LOAD_CHAPTER_INFO = 3;
    private const int INDEX_LOAD_WEAPON_ABILITY = 4;
    private const int INDEX_LOAD_WEAPON_LEVEL_ABILITY = 5;
    private const int INDEX_LOAD_BOOK_ABILITY = 6;

    private const int KEY_HERO_ABILITY = 0;
    private const int KEY_WEAPON_ABILITY = 0;
    private const int KEY_BOOK_ABILITY = 0;

    private const string URL_HERO_COMMON_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&range=A2:H";
    private const string URL_HERO_INDIVIDUAL_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&gid=915946808&range=A2:H";
    private const string URL_REQUIRED_EXP = "https://docs.google.com/spreadsheets/d/1sYr9S551mNMXqMTbALU21Y0b1bshLWUrDth8JqUnqU4/export?format=tsv&range=B2:B";
    private const string URL_CHAPTER_INFO = "https://docs.google.com/spreadsheets/d/1ptxHR2aiz_O8_7dHXWig4HZfOraFTpzJJWPBpDrn5Yw/export?format=tsv&range=A2:E";
    private const string URL_WEAPON_ABILITY = "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&range=A2:H";

    private readonly Dictionary<string, string> URL_WEAPON_LEVEL_ABILITY_DIC = new Dictionary<string, string>()
    {
        {Define.RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=2100824165&range=A2:H"},
        {Define.WEAPON_KNIGHT_SWORD, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=907954984&range=A2:H"},
        {Define.RESOURCE_WEAPON_CROSSBOW, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=2129410322&range=A2:H"},
        {Define.RESOURCE_WEAPON_FIREBALL, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=1253638460&range=A2:H"},
        {Define.RESOURCE_WEAPON_BOMB, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=1142576319&range=A2:H"},
        {Define.RESOURCE_WEAPON_DIVINE_AURA, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=38680372&range=A2:H"},
        {Define.RESOURCE_WEAPON_BOOMERANG, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=690681640&range=A2:H"}
    };
    private readonly Dictionary<string, string> URL_BOOK_ABILITY_DIC = new Dictionary<string, string>()
    {
        {Define.RESOURCE_BOOK_PROJECTILE_SPEED, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&range=B2:B"},
        {Define.RESOURCE_BOOK_PROJECTILE_COPY, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1016319567&range=B2:B"},
        {Define.RESOURCE_BOOK_COOLDOWN, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1305674535&range=B2:B"},
        {Define.RESOURCE_BOOK_RANGE, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1033079248&range=B2:B"},
        {Define.RESOURCE_BOOK_HERO_RECOVERY, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1649711332&range=B2:B"},
        {Define.RESOURCE_BOOK_HERO_MOVE_SPEED, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1231208175&range=B2:B"},
    };

    public void Init()
    {
        _loadCompletes = new bool[INDEX_LOAD_TOTAL_VALUE];
        _LoadHeroCommonAbility().Forget();
        _LoadHeroIndividualAbility().Forget();
        _LoadRequiredExp().Forget();
        _loadChapterInfo().Forget();
        _LoadWeaponAbility().Forget();
        _LoadWeaponLevelAbility().Forget();
        _LoadBookAbility().Forget();
    }

    public bool LoadComplete()
    {
        for (int ii = 0; ii < _loadCompletes.Length; ++ii)
        {
            if (false == _loadCompletes[ii])
                return false;
        }
        return true;
    }

    #region Load Hero Ability
    private async UniTaskVoid _LoadHeroCommonAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_HERO_COMMON_ABILITY);
        var op = await webRequest.SendWebRequest();

        var splitRawData = op.downloadHandler.text.Split('\t');
        HeroCommonAbility = new HeroAbility(splitRawData);
        _loadCompletes[INDEX_LOAD_HERO_COMMON_ABILITY] = true;
    }

    private async UniTaskVoid _LoadHeroIndividualAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_HERO_INDIVIDUAL_ABILITY);
        var op = await webRequest.SendWebRequest();

        HeroIndividualAbilityDic = new Dictionary<string, HeroAbility>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            HeroIndividualAbilityDic.Add(splitData[KEY_HERO_ABILITY], new HeroAbility(splitData));
        }
        _loadCompletes[INDEX_LOAD_HERO_INDIVIDUAL_ABILITY] = true;
    }
    #endregion

    #region Load Required Exp
    private async UniTaskVoid _LoadRequiredExp()
    {
        var webRequest = UnityWebRequest.Get(URL_REQUIRED_EXP);
        var op = await webRequest.SendWebRequest();

        RequiredExpList = new List<float>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            float.TryParse(data, out var value);
            RequiredExpList.Add(value);
        }
        _loadCompletes[INDEX_LOAD_REQUIRED_EXP] = true;
    }
    #endregion

    #region Chapter Info
    private async UniTaskVoid _loadChapterInfo()
    {
        var webRequest = UnityWebRequest.Get(URL_CHAPTER_INFO);
        var op = await webRequest.SendWebRequest();

        ChapterInfoList = new List<ChapterInfo>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            ChapterInfoList.Add(new ChapterInfo(splitData));
        }
        _loadCompletes[INDEX_LOAD_CHAPTER_INFO] = true;
    }
    #endregion

    #region Loat Weapon Ability
    private async UniTaskVoid _LoadWeaponAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_WEAPON_ABILITY);
        var op = await webRequest.SendWebRequest();

        WeaponAbilityDic = new Dictionary<string, WeaponAbility>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            WeaponAbilityDic.Add(splitData[KEY_WEAPON_ABILITY], new WeaponAbility(splitData));
        }
        _loadCompletes[INDEX_LOAD_WEAPON_ABILITY] = true;
    }

    private async UniTaskVoid _LoadWeaponLevelAbility()
    {
        WeaponLevelAbilityDic = new Dictionary<string, List<WeaponAbility>>();
        foreach (var weaponLevelAbilityURL in URL_WEAPON_LEVEL_ABILITY_DIC)
        {
            var webRequest = UnityWebRequest.Get(weaponLevelAbilityURL.Value);
            var op = await webRequest.SendWebRequest();

            var weaponAbilityList = new List<WeaponAbility>();
            var splitRawData = op.downloadHandler.text.Split('\n');
            foreach (var data in splitRawData)
            {
                var splitData = data.Split('\t');
                weaponAbilityList.Add(new WeaponAbility(splitData));
            }
            WeaponLevelAbilityDic.Add(weaponLevelAbilityURL.Key, weaponAbilityList);
        }
        _loadCompletes[INDEX_LOAD_WEAPON_LEVEL_ABILITY] = true;
    }
    #endregion

    #region Load Book Ability
    private async UniTaskVoid _LoadBookAbility()
    {
        BookAbilityDic = new Dictionary<string, List<float>>();
        foreach (var bookAbilityURL in URL_BOOK_ABILITY_DIC)
        {
            var webRequest = UnityWebRequest.Get(bookAbilityURL.Value);
            var op = await webRequest.SendWebRequest();

            var bookAbilityList = new List<float>();
            var splitRawData = op.downloadHandler.text.Split('\n');
            foreach (var data in splitRawData)
            {
                var splitData = data.Split('\t');
                float.TryParse(splitData[KEY_BOOK_ABILITY], out float abilityValue);
                bookAbilityList.Add(abilityValue);
            }
            BookAbilityDic.Add(bookAbilityURL.Key, bookAbilityList);
        }
        _loadCompletes[INDEX_LOAD_BOOK_ABILITY] = true;
    }
    #endregion
}
