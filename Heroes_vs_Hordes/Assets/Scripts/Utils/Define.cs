using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    #region Resource
    #region Prefab
    // Map
    public const string RESOURCE_REPOSITION_AREA = "RepositionArea";
    // Hero
    public const string RESOURCE_HERO_ARCANE_MAGE = "ArcaneMage";
    // Weapon
    public const string RESOURCE_ARCANE_MAGE_PROJECTILE = "ArcaneMage_Projectile";
    // Monster
    public const string RESOURCE_MONSTER_SPAWNER = "MonsterSpawner";
    public const string RESOURCE_MONSTER_NORMAL_BAT = "Normal_Bat";
    // Text
    public const string RESROUCE_DAMAGE_TEXT = "DamageText";
    public const string RESROUCE_LEVEL_UP_TEXT = "LevelUpText";
    // Item
    public const string RESOURCE_EXP_GEM = "ExpGem";
    public const string RESOURCE_GOLD = "Gold";
    // UI_Scene
    public const string RESOURCE_UI_INGAME_SCENE = "UI_IngameScene";
    public const string RESOURCE_UI_MAIN_SCENE = "UI_MainScene";
    // UI_Popup
    public const string RESOURCE_UI_LOADING = "UI_Loading";
    public const string RESOURCE_UI_CLEAR_CHAPTER = "UI_ClearChapter";
    public const string RESOURCE_UI_CLEAR_WAVE = "UI_ClearWave";
    public const string RESOURCE_UI_PAUSE_INGAME = "UI_PauseIngame";
    public const string RESOURCE_UI_ENHANCE_HERO_ABILITY = "UI_EnhanceHeroAbility";
    // UI_Element
    public const string RESOURCE_UI_NORMAL_BATTLE_WAVE = "UI_NormalBattleWave";
    public const string RESOURCE_UI_COIN_RUSH_WAVE = "UI_CoinRushWave";
    #endregion

    #region Sprite
    public const string RESOURCE_SPRITES_SLIDER_YELLOW = "Slider_Yellow";
    public const string RESOURCE_SPRITES_SLIDER_RED = "Slider_Red";
    #endregion
    #endregion

    #region TAG
    public const string TAG_REPOSITION_AREA = "RepositionArea";
    public const string TAG_HERO = "Hero";
    public const string TAG_MONSTER = "Monster";
    #endregion

    #region Layer
    public const string LAYER_MONSTER = "Monster";
    #endregion

    #region Animator
    public const string ANIMATOR_TRIGGER_ATTACK = "Attack";
    #endregion

    #region Objects
    public const float INCREASE_HERO_EXP_VALUE = 1;
    #endregion

    #region IngameManager
    public const int INDEX_NORMAL_BATTLE_WAVE = 0;
    public const int INDEX_GOLD_RUSH_WAVE = 1;
    public const int INDEX_TIME_ATTACK_MODE = 0;
    public const int INDEX_ANNIHILATION_MODE = 1;
    public const int INIT_HERO_LEVEL_UP_COUNT = 0;
    #endregion

    #region TEST
    public const int CURRENT_CHAPTER_INDEX = 0;
    #endregion
}
