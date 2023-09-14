using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    private bool[] _loadCompletes;

    public HeroAbility HeroCommonAbility { get; private set; }
    public Dictionary<string, HeroAbility> HeroIndividualAbilityDic { get; private set; }
    public List<float> RequiredExpList { get; private set; }
    public List<ChapterInfo> ChapterInfoList { get; private set; }

    private const int INDEX_TOTAL_VALUE = 4;
    private const int INDEX_HERO_COMMON_ABILITY = 0;
    private const int INDEX_HERO_INDIVIDUAL_ABILITY = 1;
    private const int INDEX_REQUIRED_EXP = 2;
    private const int INDEX_CHAPTER_INFO = 3;
    private const int KEY_HERO_ABILITY = 0;
    private const int KEY_CHAPTER_INFO = 0;
    private const string URL_HERO_COMMON_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&range=A2:H";
    private const string URL_HERO_INDIVIDUAL_ABILITY = "https://docs.google.com/spreadsheets/d/12WFyu9JDe1fl3O3zK9XvCx2CUZ7UbsYBBLUWCyyKdT8/export?format=tsv&gid=915946808&range=A2:H";
    private const string URL_REQUIRED_EXP = "https://docs.google.com/spreadsheets/d/1sYr9S551mNMXqMTbALU21Y0b1bshLWUrDth8JqUnqU4/export?format=tsv&range=B2:B";
    private const string URL_CHAPTER_INFO = "https://docs.google.com/spreadsheets/d/1ptxHR2aiz_O8_7dHXWig4HZfOraFTpzJJWPBpDrn5Yw/export?format=tsv&range=A2:E";

    public void Init()
    {
        _loadCompletes = new bool[INDEX_TOTAL_VALUE];
        _LoadHeroCommonAbility().Forget();
        _LoadHeroIndividualAbility().Forget();
        _LoadRequiredExp().Forget();
        _loadChapterInfo().Forget();
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
        _loadCompletes[INDEX_HERO_COMMON_ABILITY] = true;
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
        _loadCompletes[INDEX_HERO_INDIVIDUAL_ABILITY] = true;
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
        _loadCompletes[INDEX_REQUIRED_EXP] = true;
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
        _loadCompletes[INDEX_CHAPTER_INFO] = true;
    }
    #endregion
}
