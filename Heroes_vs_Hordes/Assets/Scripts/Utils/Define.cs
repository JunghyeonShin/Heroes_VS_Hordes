using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAbilityTypes
{
    HeroWeapon,
    Weapon,
    Book
}

public interface IAbilityController
{
    public void SetAbilities();
    public void ReturnAbilities();
}

public class AbilityInfo
{
    public EAbilityTypes AbilityType { get; set; }
    public string SpriteName { get; set; }
}

public class OwnedAbilityInfo
{
    public int Level { get; set; }
    public List<IAbilityController> AbilityControllerList { get; set; }
}

public class Define
{
    // Map
    public const string RESOURCE_REPOSITION_AREA = "RepositionArea";
    // Hero
    public const string RESOURCE_HERO_ARCANE_MAGE = "ArcaneMage";
    public const string RESOURCE_HERO_KNIGHT = "Knight";
    public const string RESOURCE_HERO_HEALTH = "HeroHealth";
    public const string RESOURCE_HERO_DEATH = "HeroDeath";
    public const string RESOURCE_HERO_PORTRAIT_ARCANE_MAGE = "Hero_Portrait_ArcaneMage";
    public const string RESOURCE_HERO_PORTRAIT_KNIGHT = "Hero_Portrait_Knight";
    public const string RESOURCE_SELECTABLE_HERO_ARCANE_MAGE = "Selectable_Hero_ArcaneMage";
    public const string RESOURCE_SELECTABLE_HERO_KNIGHT = "Selectable_Hero_Knight";
    // Weapon
    public const string RESOURCE_WEAPON_ARCANE_MAGE_PROJECTILE = "ArcaneMage_Projectile";
    public const string RESOURCE_WEAPON_BOMB_CONTROLLER = "BombController";
    public const string RESOURCE_WEAPON_BOOMERANG_CONTROLLER = "BoomerangController";
    public const string RESOURCE_WEAPON_CROSSBOW_CONTROLLER = "CrossbowController";
    public const string RESOURCE_WEAPON_DIVINE_AURA_CONTROLLER = "DivineAuraController";
    public const string RESOURCE_WEAPON_FIREBALL_CONTROLLER = "FireballController";
    // Monster
    public const string RESOURCE_MONSTER_SPAWNER = "MonsterSpawner";
    public const string RESOURCE_MONSTER_NORMAL_BAT = "Normal_Bat";
    public const string RESOURCE_MONSTER_SWARM_BAT = "Swarm_Bat";
    public const string RESOURCE_MONSTER_NORMAL_GOBLIN = "Normal_Goblin";
    public const string RESOURCE_MONSTER_CLUB_GOBLIN = "Club_Goblin";
    public const string RESOURCE_MONSTER_ARMOR_GOBLIN = "Armor_Goblin";
    public const string RESOURCE_MONSTER_NORMAL_SKELETON = "Normal_Skeleton";
    public const string RESOURCE_MONSTER_ARMOR_SKELETON = "Armor_Skeleton";
    public const string RESOURCE_MONSTER_NORMAL_SPIDER = "Normal_Spider";
    public const string RESOURCE_MONSTER_CAVE_SPIDER = "Cave_Spider";
    public const string RESOURCE_MONSTER_BOSS_SPIDER = "Boss_Spider";
    public const string RESOURCE_BOSS_MAP_CAMERA_FOLLOWER = "BossMapCameraFollower";
    // Boss Monster Attack
    public const string RESOURCE_BOSS_SPIDER_WEB = "SpiderWeb";
    public const string RESOURCE_BOSS_SPIDER_WEB_BUNDLE = "WebBundle";
    public const string RESOURCE_SLOW_EFFECT = "SlowEffect";
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
    public const string RESOURCE_UI_DEFEAT_WAVE = "UI_DefeatWave";
    public const string RESOURCE_UI_PAUSE_INGAME = "UI_PauseIngame";
    public const string RESOURCE_UI_LEVEL_UP_HERO = "UI_LevelUpHero";
    public const string RESOURCE_UI_FADE = "UI_Fade";
    public const string RESOURCE_UI_SELECT_HERO = "UI_SelectHero";
    // UI_Element
    public const string RESOURCE_UI_NORMAL_BATTLE_WAVE = "UI_NormalBattleWave";
    public const string RESOURCE_UI_GOLD_RUSH_WAVE = "UI_GoldRushWave";
    public const string RESOURCE_UI_BOSS_BATTLE_WAVE = "UI_BossBattleWave";
    public const string RESOURCE_UI_ABILITY = "UI_Ability";
    public const string RESOURCE_UI_SELECT_ABILITY = "UI_SelectAbility";
    public const string RESOURCE_UI_SELECTABLE_HERO = "UI_SelectableHero";
    public const string RESOURCE_UI_BLANK_SELECTABLE_HERO = "UI_BlankSelectableHero";
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
    public const string LAYER_LAND_MONSTER = "LandMonster";
    public const string LAYER_SKY_MONSTER = "SkyMonster";
    public const string LAYER_BOSS_MONSTER = "BossMonster";
    public static readonly int LAYER_MASK_MONSTER = 1 << LayerMask.NameToLayer(LAYER_LAND_MONSTER) | 1 << LayerMask.NameToLayer(LAYER_SKY_MONSTER) | 1 << LayerMask.NameToLayer(LAYER_BOSS_MONSTER);
    #endregion

    #region Animator
    public const string ANIMATOR_TRIGGER_ATTACK = "Attack";
    public const string ANIMATOR_TRIGGER_EXPLODE = "Explode";
    #endregion

    #region IngameManager
    public const float INCREASE_HERO_EXP_VALUE = 1;
    public const int INCREASE_GOLD_VALUE = 1;
    public const int INDEX_NORMAL_BATTLE_WAVE = 0;
    public const int INDEX_GOLD_RUSH_WAVE = 1;
    public const int INDEX_BOSS_BATTLE_WAVE = 2;
    public const int INDEX_TIME_ATTACK_MODE = 0;
    public const int INDEX_ANNIHILATION_MODE = 1;
    public const int INIT_HERO_LEVEL_UP_COUNT = 0;
    #endregion

    #region Wave Panel Transform
    public const int FOUR_WAVE = 4;
    public const int FIVE_WAVE = 5;

    public static readonly WavePanelTransform[] FOUR_WAVE_PANEL_TRANSFORMS = new WavePanelTransform[]
    {
        new WavePanelTransform() { PanelPosition = new Vector2(-125f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = Vector2.zero, PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(125f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(187.5f, 30f), PanelSize = new Vector2(100f, 100f), IconSize = new Vector2(50f, 55f) }
    };
    public static readonly WavePanelTransform[] FIVE_WAVE_PANEL_TRANSFORMS = new WavePanelTransform[]
    {
        new WavePanelTransform() { PanelPosition = new Vector2(-185f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(-60f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(65f, 0f), PanelSize = new Vector2(125f, 40f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(190f, 0f), PanelSize = new Vector2(100f, 100f), IconSize = new Vector2(50f, 55f) },
        new WavePanelTransform() { PanelPosition = new Vector2(252.5f, 30f), PanelSize = new Vector2(100f, 100f), IconSize = new Vector2(50f, 55f) }
    };
    #endregion

    #region Ability
    public const string WEAPON_ARCANE_MAGE_WAND = "ArcaneMage_Wand";
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

    public static readonly List<string> ABILITY_LIST = new List<string>()
    {
        WEAPON_ARCANE_MAGE_WAND,
        WEAPON_KNIGHT_SWORD,
        WEAPON_BOMB,
        WEAPON_BOOMERANG,
        WEAPON_CROSSBOW,
        WEAPON_DIVINE_AURA,
        WEAPON_FIREBALL,
        BOOK_COOLDOWN,
        BOOK_HERO_MOVE_SPEED,
        BOOK_HERO_RECOVERY,
        BOOK_PROJECTILE_COPY,
        BOOK_PROJECTILE_SPEED,
        BOOK_RANGE
    };
    public static readonly Dictionary<string, AbilityInfo> ABILITY_INFO_DIC = new Dictionary<string, AbilityInfo>()
    {
        {WEAPON_ARCANE_MAGE_WAND, new AbilityInfo() { AbilityType = EAbilityTypes.HeroWeapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_HERO_ARCANE_WAND } },
        {WEAPON_KNIGHT_SWORD, new AbilityInfo() { AbilityType = EAbilityTypes.HeroWeapon, SpriteName = RESOURCE_SPRITES_ICON_WEAPON_HERO_SWORD } },
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

    public const int ADJUSE_CHAPTER_INDEX = 1;
}
