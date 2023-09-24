using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LevelUpHero : UI_Popup
{
    private enum EGameObjects
    {
        SelectAbilityContents,
        OwnedWeaponContents,
        OwnedBookContents
    }

    private GameObject _selectAbilityContents;
    private GameObject _ownedWeaponContents;
    private GameObject _ownedBookContents;

    private List<UI_SelectAbility> _selectAbilityList = new List<UI_SelectAbility>();
    private List<UI_Ability> _weaponAbilityList = new List<UI_Ability>();
    private List<UI_Ability> _bookAbilityList = new List<UI_Ability>();

    private const int CREATE_SELECT_ABILITY_UI_COUNT = 3;
    private const int CREATE_ABILITY_UI_COUNT = 5;

    protected override void _Init()
    {
        _BindGameObject(typeof(EGameObjects));

        _selectAbilityContents = _GetGameObject((int)EGameObjects.SelectAbilityContents);
        _ownedWeaponContents = _GetGameObject((int)EGameObjects.OwnedWeaponContents);
        _ownedBookContents = _GetGameObject((int)EGameObjects.OwnedBookContents);
    }

    public void InitSelectAbilityPanel()
    {
        for (int ii = 0; ii < _selectAbilityList.Count; ++ii)
            _selectAbilityList[ii].ReturnSelectAbilityUI();
        _selectAbilityList.Clear();

        for (int ii = 0; ii < CREATE_SELECT_ABILITY_UI_COUNT; ++ii)
        {
            var selectAbilityUIGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_SELECT_ABILITY);
            var selectAbilityUI = Utils.GetOrAddComponent<UI_SelectAbility>(selectAbilityUIGO);
            selectAbilityUI.InitSelectAbilityUI(_selectAbilityContents.transform);
            selectAbilityUI.OnSelectAbilityHandler -= _ClosePopupUI;
            selectAbilityUI.OnSelectAbilityHandler += _ClosePopupUI;
            _selectAbilityList.Add(selectAbilityUI);
        }
    }

    public void UpdateSelectAbilityPanel()
    {
        for (int ii = 0; ii < _selectAbilityList.Count; ++ii)
            //_selectAbilityList[ii].UpdateSelectAbilityUI(Manager.Instance.Ingame.DrawAbilityList[ii]);
            _selectAbilityList[ii].UpdateSelectAbilityUI(Define.WEAPON_DIVINE_AURA);
    }

    public void InitAbilityUI()
    {
        for (int ii = 0; ii < _weaponAbilityList.Count; ++ii)
            _weaponAbilityList[ii].ReturnAbilityUI();
        _weaponAbilityList.Clear();

        for (int ii = 0; ii < CREATE_ABILITY_UI_COUNT; ++ii)
        {
            var abilityUIGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityUIGO);
            abilityUI.InitAbilityUI(_ownedWeaponContents.transform);
            _weaponAbilityList.Add(abilityUI);
        }

        for (int ii = 0; ii < _bookAbilityList.Count; ++ii)
            _bookAbilityList[ii].ReturnAbilityUI();
        _bookAbilityList.Clear();

        for (int ii = 0; ii < CREATE_ABILITY_UI_COUNT; ++ii)
        {
            var abilityUIGO = Manager.Instance.UI.GetElementUI(Define.RESOURCE_UI_ABILITY);
            var abilityUI = Utils.GetOrAddComponent<UI_Ability>(abilityUIGO);
            abilityUI.InitAbilityUI(_ownedBookContents.transform);
            _bookAbilityList.Add(abilityUI);
        }
    }

    public void UpdateWeaponAbilityUI()
    {
        var ownedAllWeaponList = Manager.Instance.Ingame.OwnedWeaponList;
        for (int ii = 0; ii < ownedAllWeaponList.Count; ++ii)
            _weaponAbilityList[ii].UpdateAbilityUI(ownedAllWeaponList[ii]);
    }

    public void UpdateBookAbilityUI()
    {
        var ownedAllBookList = Manager.Instance.Ingame.OwnedBookList;
        for (int ii = 0; ii < ownedAllBookList.Count; ++ii)
            _bookAbilityList[ii].UpdateAbilityUI(ownedAllBookList[ii]);
    }
}
