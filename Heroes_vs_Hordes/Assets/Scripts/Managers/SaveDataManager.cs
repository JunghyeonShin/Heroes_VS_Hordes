using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private GameData _gameData;

    private const int INIT_CLEAR_CHAPTER = -1;
    private const int INIT_HERO_LEVEL = 1;
    private const int TOTAL_HEROES_COUNT = 2;
    private const int TOTAL_TALENT_COUNT = 5;
    private const int INDEX_HERO_ARCANE_MAGE = 0;

    private readonly string SAVE_DATA_PATH = Application.persistentDataPath + "/SaveData.json";

    public int ClearChapter
    {
        get
        {
            return _gameData.ClearChapter;
        }
        set
        {
            _gameData.ClearChapter = value;
            var lastChapter = Manager.Instance.Data.ChapterInfoDataList.Count - 1;
            if (_gameData.ClearChapter > lastChapter)
                _gameData.ClearChapter = lastChapter;
            SaveGameData();
        }
    }

    public int OwnedGold
    {
        get
        {
            return _gameData.OwnedGold;
        }
        set
        {
            _gameData.OwnedGold = value;
            SaveGameData();
        }
    }

    public string SelectHero
    {
        get
        {
            return _gameData.SelectHero;
        }
        set
        {
            _gameData.SelectHero = value;
            SaveGameData();
        }
    }

    public int[] OwnedHeroes { get { return _gameData.OwnedHeroes; } }
    public void SetOwnedHero(int index)
    {
        _gameData.OwnedHeroes[index] = INIT_HERO_LEVEL;
    }

    public int[] OwnedTalents { get { return _gameData.OwnedTalents; } }
    public int TotalOwnedTalentsLevel
    {
        get
        {
            var totalLevel = 0;
            for (int ii = 0; ii < _gameData.OwnedTalents.Length; ++ii)
                totalLevel += _gameData.OwnedTalents[ii];
            return totalLevel;
        }
    }
    public void SetOwnedTalent(int index)
    {
        ++_gameData.OwnedTalents[index];
    }

    public void Init()
    {
        if (_LoadGameData())
            return;

        _gameData = new GameData();
        _gameData.ClearChapter = INIT_CLEAR_CHAPTER;
        _gameData.SelectHero = Define.RESOURCE_HERO_ARCANE_MAGE;
        _gameData.OwnedHeroes = new int[TOTAL_HEROES_COUNT];
        _gameData.OwnedHeroes[INDEX_HERO_ARCANE_MAGE] = INIT_HERO_LEVEL;
        _gameData.OwnedTalents = new int[TOTAL_TALENT_COUNT];
        SaveGameData();
    }

    public void SaveGameData()
    {
        var json = JsonUtility.ToJson(_gameData);
        File.WriteAllText(SAVE_DATA_PATH, json);
    }

    private bool _LoadGameData()
    {
        if (false == File.Exists(SAVE_DATA_PATH))
            return false;

        var rawData = File.ReadAllText(SAVE_DATA_PATH);
        _gameData = JsonUtility.FromJson<GameData>(rawData);
        if (null == _gameData)
            return false;

        return true;
    }
}
