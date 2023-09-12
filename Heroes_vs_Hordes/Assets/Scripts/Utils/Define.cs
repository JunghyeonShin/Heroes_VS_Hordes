using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    #region Resource
    #region Prefab
    // Map
    public const string RESOURCE_REPOSITION_AREA = "RepositionArea";
    public const string RESOURCE_MAP_00 = "Map_00";
    // Hero
    public const string RESOURCE_HERO_ARCANE_MAGE = "ArcaneMage";
    // Weapon
    public const string RESOURCE_ARCANE_MAGE_PROJECTILE = "ArcaneMage_Projectile";
    // Monster
    public const string RESOURCE_MONSTER_SPAWNER = "MonsterSpawner";
    public const string RESOURCE_MONSTER_NORMAL_BAT = "Normal_Bat";
    // DamageText
    public const string RESROUCE_DAMAGE_TEXT = "DamageText";
    // ExperienceGem
    public const string RESOURCE_EXPERIENCE_GEM = "ExperienceGem";
    // UI_Scene
    public const string RESOURCE_UI_INGAME_SCENE = "UI_IngameScene";
    public const string RESOURCE_UI_MAIN_SCENE = "UI_MainScene";
    // UI_Popup
    public const string RESOURCE_UI_CLEAR_CHAPTER = "UI_ClearChapter";
    public const string RESOURCE_UI_CLEAR_WAVE = "UI_ClearWave";
    public const string RESOURCE_UI_PAUSE_INGAME = "UI_PauseIngame";
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
    public const string TAG_MONSTER = "Monster";
    #endregion

    #region Layer
    public const string LAYER_MONSTER = "Monster";
    #endregion

    #region Ability
    // Hero
    public const int HERO_ABILITY_HEALTH = 1;
    public const int HERO_ABILITY_DEFENCE = 2;
    public const int HERO_ABILITY_ATTACK = 3;
    public const int HERO_ABILITY_ATTACK_COOLDOWN = 4;
    public const int HERO_ABILITY_CRITICAL = 5;
    public const int HERO_ABILITY_MOVE_SPEED = 6;
    public const int HERO_ABILITY_PROJECTILE_SPEED = 7;
    #endregion

    #region Animator
    public const string ANIMATOR_TRIGGER_ATTACK = "Attack";
    #endregion

    // IngameManager
    public const int TIME_ATTACK_MODE = 0;

    #region TEST
    public const int MAX_WAVE_INDEX = 4;
    #endregion
}
