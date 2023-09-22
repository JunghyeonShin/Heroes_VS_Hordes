using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAbilityTypes
{
    Weapon,
    Book
}

public class AbilityInfo
{
    public EAbilityTypes AbilityType { get; set; }
    public string SpriteName { get; set; }
}

public class Define
{
    // Map
    public const string RESOURCE_REPOSITION_AREA = "RepositionArea";
    // Hero
    public const string RESOURCE_HERO_ARCANE_MAGE = "ArcaneMage";
    // Weapon
    public const string RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE = "ArcaneMage_Projectile";
    public const string RESOURCE_WEAPON_BOMB_CONTROLLER = "BombController";
    public const string RESOURCE_BOOMERANG_CONTROLLER = "BoomerangController";
    public const string RESOURCE_WEAPON_CROSSBOW_CONTROLLER = "CrossbowController";
    public const string RESOURCE_DIVINE_AURA_CONTROLLER = "DivineAuraController";
    public const string RESOURCE_WEAPON_FIREBALL_CONTROLLER = "FireballController";
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
    public const string RESOURCE_UI_LEVEL_UP_HERO = "UI_LevelUpHero";
    // UI_Element
    public const string RESOURCE_UI_NORMAL_BATTLE_WAVE = "UI_NormalBattleWave";
    public const string RESOURCE_UI_GOLD_RUSH_WAVE = "UI_GoldRushWave";
    public const string RESOURCE_UI_ABILITY = "UI_Ability";
    public const string RESOURCE_UI_SELECT_ABILITY = "UI_SelectAbility";
    // Sprite
    public const string RESOURCE_SPRITES_SLIDER_YELLOW = "Slider_Yellow";
    public const string RESOURCE_SPRITES_SLIDER_RED = "Slider_Red";
    public const string RESOURCE_SPRITES_ICON_WEAPON_HERO_ARCANE_WAND = "Icon_Weapon_Hero_ArcaneWand";
    public const string RESOURCE_SPRITES_ICON_WEAPON_HERO_SWORD = "Icon_Weapon_Hero_Sword";
    public const string RESOURCE_SPRITES_ICON_WEAPON_BOMB = "Icon_Weapon_Bomb";
    public const string RESOURCE_SPRITES_ICON_WEAPON_BOOMERANG = "Icon_Weapon_Boomerang";
    public const string RESOURCE_SPRITES_ICON_WEAPON_CROSSBOW = "Icon_Weapon_Crossbow";
    public const string RESOURCE_SPRITES_ICON_WEAPON_DIVINE_AURA = "Icon_Weapon_DivineAura";
    public const string RESOURCE_SPRITES_ICON_WEAPON_FIREBALL = "Icon_Weapon_Fireball";
    public const string RESOURCE_SPRITES_ICON_BOOK_COOLDOWN = "Icon_Book_Cooldown";
    public const string RESOURCE_SPRITES_ICON_BOOK_HERO_MOVE_SPEED = "Icon_Book_Hero_Move_Speed";
    public const string RESOURCE_SPRITES_ICON_BOOK_HERO_RECOVERY = "Icon_Book_Hero_Recovery";
    public const string RESOURCE_SPRITES_ICON_BOOK_PROJECTILE_COPY = "Icon_Book_Projectile_Copy";
    public const string RESOURCE_SPRITES_ICON_BOOK_PROJECTILE_SPEED = "Icon_Book_Projectile_Speed";
    public const string RESOURCE_SPRITES_ICON_BOOK_RANGE = "Icon_Book_Range";

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
    public const string ANIMATOR_TRIGGER_EXPLODE = "Explode";
    #endregion

    #region IngameManager
    public const float INCREASE_HERO_EXP_VALUE = 1;
    public const float INCREASE_GOLD_VALUE = 1;
    public const int INDEX_NORMAL_BATTLE_WAVE = 0;
    public const int INDEX_GOLD_RUSH_WAVE = 1;
    public const int INDEX_TIME_ATTACK_MODE = 0;
    public const int INDEX_ANNIHILATION_MODE = 1;
    public const int INIT_HERO_LEVEL_UP_COUNT = 0;
    #endregion

    #region TEST
    public const int CURRENT_CHAPTER_INDEX = 0;
    #endregion

    #region Ability
    public const string WEAPON_ARCANE_MAGE_PROJECTILE = "ArcaneMage_Projectile";
    public const string WEAPON_KNIGHT_SWORD = "Knight_Sword";
    public const string WEAPON_BOMB = "Bomb";
    public const string WEAPON_BOOMERANG = "Boomerang";
    public const string WEAPON_CROSSBOW = "Crossbow";
    public const string WEAPON_DIVINE_AURA = "DivineAura";
    public const string WEAPON_FIREBALL = "Fireball";
    public const string BOOK_COOLDOWN = "CooldownBook";
    public const string BOOK_HERO_MOVE_SPEED = "HeroMoveSpeedBook";
    public const string BOOK_HERO_RECOVERY = "HeroRecoveryBook";
    public const string BOOK_PROJECTILE_COPY = "ProjectileCopyBook";
    public const string BOOK_PROJECTILE_SPEED = "ProjectileSpeedBook";
    public const string BOOK_RANGE = "RangeBook";

    public static readonly Dictionary<string, AbilityInfo> ABILITY_SPRITE_DIC = new Dictionary<string, AbilityInfo>()
    {
        {WEAPON_ARCANE_MAGE_PROJECTILE, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_HERO_ARCANE_WAND } },
        {WEAPON_BOMB, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_BOMB } },
        {WEAPON_BOOMERANG, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_BOOMERANG } },
        {WEAPON_CROSSBOW, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_CROSSBOW } },
        {WEAPON_DIVINE_AURA, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_DIVINE_AURA } },
        {WEAPON_FIREBALL, new AbilityInfo() { AbilityType = EAbilityTypes.Weapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_FIREBALL } },
        {BOOK_COOLDOWN, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_COOLDOWN } },
        {BOOK_HERO_MOVE_SPEED, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_HERO_MOVE_SPEED } },
        {BOOK_HERO_RECOVERY, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_HERO_RECOVERY } },
        {BOOK_PROJECTILE_COPY, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_PROJECTILE_COPY } },
        {BOOK_PROJECTILE_SPEED, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_PROJECTILE_SPEED } },
        {BOOK_RANGE, new AbilityInfo() { AbilityType = EAbilityTypes.Book, SpriteName = RESOURCE_SPRITES_ICON_BOOK_RANGE } }
    };
    #endregion
}
