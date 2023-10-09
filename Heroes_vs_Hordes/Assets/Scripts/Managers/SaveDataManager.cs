using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private GameData _gameData;

    private const int INIT_CLEAR_CHAPTER = -1;

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

    public void Init()
    {
        if (_LoadGameData())
            return;

        _gameData = new GameData();
        _gameData.ClearChapter = INIT_CLEAR_CHAPTER;
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
