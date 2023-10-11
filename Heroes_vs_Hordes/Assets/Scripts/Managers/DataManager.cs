using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    private bool[] _loadCompletes;

    public HeroAbilityData HeroCommonAbilityData { get; private set; }
    public Dictionary<string, HeroAbilityData> HeroIndividualAbilityDataDic { get; private set; }
    public List<float> RequiredExpDataList { get; private set; }
    public List<ChapterInfoData> ChapterInfoDataList { get; private set; }
    public Dictionary<string, WeaponAbilityData> WeaponAbilityDataDic { get; private set; }
    public Dictionary<string, List<WeaponAbilityData>> WeaponLevelAbilityDataDic { get; private set; }
    public Dictionary<string, List<float>> BookAbilityDataDic { get; private set; }
    public Dictionary<string, List<AbilityDescriptionData>> AbilityDescriptionDataDic { get; private set; }
    public Dictionary<string, MonsterInfoData> MonsterInfoDic { get; private set; }
    public List<CostToObtainHeroData> CostToObtainHeroDataList { get; private set; }

    private const int INDEX_LOAD_TOTAL_VALUE = 10;
    private const int INDEX_LOAD_HERO_COMMON_ABILITY = 0;
    private const int INDEX_LOAD_HERO_INDIVIDUAL_ABILITY = 1;
    private const int INDEX_LOAD_REQUIRED_EXP = 2;
    private const int INDEX_LOAD_CHAPTER_INFO = 3;
    private const int INDEX_LOAD_WEAPON_ABILITY = 4;
    private const int INDEX_LOAD_WEAPON_LEVEL_ABILITY = 5;
    private const int INDEX_LOAD_BOOK_ABILITY = 6;
    private const int INDEX_LOAD_ABILITY_DESCRIPTION = 7;
    private const int INDEX_LOAD_MONSTER_INFO = 8;
    private const int INDEX_LOAD_COST_TO_OBTAIN_HERO = 9;

    private const int KEY_HERO_ABILITY = 0;
    private const int KEY_WEAPON_ABILITY = 0;
    private const int KEY_BOOK_ABILITY = 0;
    private const int KEY_MONSTER_INFO = 0;

    private const string URL_HERO_COMMON_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&range=A2:H";
    private const string URL_HERO_INDIVIDUAL_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&gid=915946808&range=A2:H";
    private const string URL_REQUIRED_EXP = "https://docs.google.com/spreadsheets/d/1sYr9S551mNMXqMTbALU21Y0b1bshLWUrDth8JqUnqU4/export?format=tsv&range=B2:B";
    private const string URL_CHAPTER_INFO = "https://docs.google.com/spreadsheets/d/1ptxHR2aiz_O8_7dHXWig4HZfOraFTpzJJWPBpDrn5Yw/export?format=tsv&range=A2:G";
    private const string URL_WEAPON_ABILITY = "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&range=A2:H";
    private const string URL_MONSTER_INFO = "https://docs.google.com/spreadsheets/d/1IoSyWsESeudzCxpTH7C9OuVO4DSK9dxACgsTze3T64g/export?format=tsv&range=A2:D";
    private const string URL_COST_TO_OBTAIN_HERO = "https://docs.google.com/spreadsheets/d/1CD6C-4wPzAcKiNm6oVV5L8NXekxnrYair2ZzAd8fvdg/export?format=tsv&range=B2:B";

    private readonly Dictionary<string, string> URL_WEAPON_LEVEL_ABILITY_DIC = new Dictionary<string, string>()
    {
        {Define.WEAPON_ARCANE_MAGE_WAND, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=2100824165&range=A2:H"},
        {Define.WEAPON_KNIGHT_SWORD, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=907954984&range=A2:H"},
        {Define.WEAPON_BOMB, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=1142576319&range=A2:H"},
        {Define.WEAPON_BOOMERANG, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=690681640&range=A2:H"},
        {Define.WEAPON_CROSSBOW, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=2129410322&range=A2:H"},
        {Define.WEAPON_DIVINE_AURA, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=38680372&range=A2:H"},
        {Define.WEAPON_FIREBALL, "https://docs.google.com/spreadsheets/d/1xWD3vUbZbW5n3M9wpjq1kzT2R3nonKkUXNb1lIu4dmY/export?format=tsv&gid=1253638460&range=A2:H"}
    };
    private readonly Dictionary<string, string> URL_BOOK_ABILITY_DIC = new Dictionary<string, string>()
    {
        {Define.BOOK_PROJECTILE_SPEED, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&range=B2:B"},
        {Define.BOOK_PROJECTILE_COPY, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1016319567&range=B2:B"},
        {Define.BOOK_COOLDOWN, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1305674535&range=B2:B"},
        {Define.BOOK_RANGE, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1033079248&range=B2:B"},
        {Define.BOOK_HERO_RECOVERY, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1649711332&range=B2:B"},
        {Define.BOOK_HERO_MOVE_SPEED, "https://docs.google.com/spreadsheets/d/1LTrv5qwXc163K81dVZuo8T0xc6jPGvnJMEOJTfBDKxw/export?format=tsv&gid=1231208175&range=B2:B"},
    };
    private readonly Dictionary<string, string> URL_ABILITY_DESCRIPTION_DIC = new Dictionary<string, string>()
    {
        {Define.WEAPON_ARCANE_MAGE_WAND,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&range=B2:D" },
        {Define.WEAPON_KNIGHT_SWORD,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=960709216&range=B2:D" },
        {Define.WEAPON_BOMB,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=1495373022&range=B2:D" },
        {Define.WEAPON_BOOMERANG,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=681918787&range=B2:D" },
        {Define.WEAPON_CROSSBOW,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=722212876&range=B2:D" },
        {Define.WEAPON_DIVINE_AURA,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=1198127096&range=B2:D" },
        {Define.WEAPON_FIREBALL,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=1311998993&range=B2:D" },
        {Define.BOOK_COOLDOWN,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=1645064844&range=B2:D" },
        {Define.BOOK_HERO_MOVE_SPEED,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=823477494&range=B2:D" },
        {Define.BOOK_HERO_RECOVERY,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=468300514&range=B2:D" },
        {Define.BOOK_PROJECTILE_COPY,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=572090125&range=B2:D" },
        {Define.BOOK_PROJECTILE_SPEED,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=1592589412&range=B2:D" },
        {Define.BOOK_RANGE,"https://docs.google.com/spreadsheets/d/1xUKd9Z1qmH8t1cuHQzwhaMA1ecgDtNoaiNK2lgxpFBQ/export?format=tsv&gid=789511474&range=B2:D" }
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
        _LoadAbilityDescription().Forget();
        _LoadMonsterInfo().Forget();
        _LoadCostToObtainHero().Forget();
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
        HeroCommonAbilityData = new HeroAbilityData(splitRawData);
        _loadCompletes[INDEX_LOAD_HERO_COMMON_ABILITY] = true;
    }

    private async UniTaskVoid _LoadHeroIndividualAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_HERO_INDIVIDUAL_ABILITY);
        var op = await webRequest.SendWebRequest();

        HeroIndividualAbilityDataDic = new Dictionary<string, HeroAbilityData>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            HeroIndividualAbilityDataDic.Add(splitData[KEY_HERO_ABILITY], new HeroAbilityData(splitData));
        }
        _loadCompletes[INDEX_LOAD_HERO_INDIVIDUAL_ABILITY] = true;
    }
    #endregion

    #region Load Required Exp
    private async UniTaskVoid _LoadRequiredExp()
    {
        var webRequest = UnityWebRequest.Get(URL_REQUIRED_EXP);
        var op = await webRequest.SendWebRequest();

        RequiredExpDataList = new List<float>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            float.TryParse(data, out var value);
            RequiredExpDataList.Add(value);
        }
        _loadCompletes[INDEX_LOAD_REQUIRED_EXP] = true;
    }
    #endregion

    #region Chapter Info
    private async UniTaskVoid _loadChapterInfo()
    {
        var webRequest = UnityWebRequest.Get(URL_CHAPTER_INFO);
        var op = await webRequest.SendWebRequest();

        ChapterInfoDataList = new List<ChapterInfoData>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            ChapterInfoDataList.Add(new ChapterInfoData(splitData));
        }
        _loadCompletes[INDEX_LOAD_CHAPTER_INFO] = true;
    }
    #endregion

    #region Loat Weapon Ability
    private async UniTaskVoid _LoadWeaponAbility()
    {
        var webRequest = UnityWebRequest.Get(URL_WEAPON_ABILITY);
        var op = await webRequest.SendWebRequest();

        WeaponAbilityDataDic = new Dictionary<string, WeaponAbilityData>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            WeaponAbilityDataDic.Add(splitData[KEY_WEAPON_ABILITY], new WeaponAbilityData(splitData));
        }
        _loadCompletes[INDEX_LOAD_WEAPON_ABILITY] = true;
    }

    private async UniTaskVoid _LoadWeaponLevelAbility()
    {
        WeaponLevelAbilityDataDic = new Dictionary<string, List<WeaponAbilityData>>();
        foreach (var weaponLevelAbilityURL in URL_WEAPON_LEVEL_ABILITY_DIC)
        {
            var webRequest = UnityWebRequest.Get(weaponLevelAbilityURL.Value);
            var op = await webRequest.SendWebRequest();

            var weaponAbilityList = new List<WeaponAbilityData>();
            var splitRawData = op.downloadHandler.text.Split('\n');
            foreach (var data in splitRawData)
            {
                var splitData = data.Split('\t');
                weaponAbilityList.Add(new WeaponAbilityData(splitData));
            }
            WeaponLevelAbilityDataDic.Add(weaponLevelAbilityURL.Key, weaponAbilityList);
        }
        _loadCompletes[INDEX_LOAD_WEAPON_LEVEL_ABILITY] = true;
    }
    #endregion

    #region Load Book Ability
    private async UniTaskVoid _LoadBookAbility()
    {
        BookAbilityDataDic = new Dictionary<string, List<float>>();
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
            BookAbilityDataDic.Add(bookAbilityURL.Key, bookAbilityList);
        }
        _loadCompletes[INDEX_LOAD_BOOK_ABILITY] = true;
    }
    #endregion

    #region AbilityDescription
    private async UniTaskVoid _LoadAbilityDescription()
    {
        AbilityDescriptionDataDic = new Dictionary<string, List<AbilityDescriptionData>>();
        foreach (var abilityDescriptionURL in URL_ABILITY_DESCRIPTION_DIC)
        {
            var webRequest = UnityWebRequest.Get(abilityDescriptionURL.Value);
            var op = await webRequest.SendWebRequest();

            var abilityDescriptionList = new List<AbilityDescriptionData>();
            var splitRawData = op.downloadHandler.text.Split('\n');
            foreach (var data in splitRawData)
            {
                var splitData = data.Split('\t');
                abilityDescriptionList.Add(new AbilityDescriptionData(splitData));
            }
            AbilityDescriptionDataDic.Add(abilityDescriptionURL.Key, abilityDescriptionList);
        }
        _loadCompletes[INDEX_LOAD_ABILITY_DESCRIPTION] = true;
    }
    #endregion

    #region MonsterInfo
    private async UniTaskVoid _LoadMonsterInfo()
    {
        var webRequest = UnityWebRequest.Get(URL_MONSTER_INFO);
        var op = await webRequest.SendWebRequest();

        MonsterInfoDic = new Dictionary<string, MonsterInfoData>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            MonsterInfoDic.Add(splitData[KEY_MONSTER_INFO], new MonsterInfoData(splitData));
        }
        _loadCompletes[INDEX_LOAD_MONSTER_INFO] = true;
    }
    #endregion

    #region CostToObtainHero
    private async UniTaskVoid _LoadCostToObtainHero()
    {
        var webRequest = UnityWebRequest.Get(URL_COST_TO_OBTAIN_HERO);
        var op = await webRequest.SendWebRequest();

        CostToObtainHeroDataList = new List<CostToObtainHeroData>();
        var splitRawData = op.downloadHandler.text.Split('\n');
        foreach (var data in splitRawData)
        {
            var splitData = data.Split('\t');
            CostToObtainHeroDataList.Add(new CostToObtainHeroData(splitData));
        }
        _loadCompletes[INDEX_LOAD_COST_TO_OBTAIN_HERO] = true;
    }
    #endregion
}
