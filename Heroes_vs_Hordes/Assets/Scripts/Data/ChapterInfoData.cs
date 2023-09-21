using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterInfoData
{
    private string _chapterName;
    private float _time;
    private int[] _waveIndex;
    private string _mapType;
    private string[] _monsterTypes;

    public string ChapterName { get { return _chapterName; } }
    public float Time { get { return _time; } }
    public int[] WaveIndex { get { return _waveIndex; } }
    public int TotalWaveIndex { get { return _waveIndex.Length; } }
    public string MapType { get { return _mapType; } }
    public string[] MonsterTypes { get { return _monsterTypes; } }

    private const int INDEX_CHAPTER_NAME = 0;
    private const int INDEX_CHAPTER_INFO_TIME = 1;
    private const int INDEX_CHAPTER_INFO_WAVE_INDEX = 2;
    private const int INDEX_CHAPTER_INFO_MAP_TYPE = 3;
    private const int INDEX_CHAPTER_INFO_MONSTER_TYPES = 4;

    public ChapterInfoData(string[] splitData)
    {
        _chapterName = splitData[INDEX_CHAPTER_NAME].TrimEnd();
        float.TryParse(splitData[INDEX_CHAPTER_INFO_TIME].TrimEnd(), out _time);
        var waveIndexData = splitData[INDEX_CHAPTER_INFO_WAVE_INDEX].Split('/');
        _waveIndex = new int[waveIndexData.Length];
        for (int ii = 0; ii < waveIndexData.Length; ++ii)
            int.TryParse(waveIndexData[ii].TrimEnd(), out _waveIndex[ii]);
        _mapType = splitData[INDEX_CHAPTER_INFO_MAP_TYPE].TrimEnd();
        var monsterTypeDatas = splitData[INDEX_CHAPTER_INFO_MONSTER_TYPES].Split('/');
        _monsterTypes = new string[monsterTypeDatas.Length];
        for (int ii = 0; ii < monsterTypeDatas.Length; ++ii)
            _monsterTypes[ii] = monsterTypeDatas[ii].TrimEnd();
    }
}
