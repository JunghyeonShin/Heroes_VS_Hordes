namespace ProtoType
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class UI_TestScene : UI_Scene
    {
        private enum EButtons
        {
            Spawn,
            LevelUp,
            LevelDown,
            NormalAttack
        }

        private enum ETexts
        {
            WeaponName,
            Attack,
            AttackCooldown,
            Speed,
            EffectRange,
            EffectTime,
            ProjectileCount,
            PenetraitCount
        }

        public event Action SpawnMonsterHandler;
        public event Action LevelUpHandler;
        public event Action LevelDownHandler;
        public event Action NormalAttackHandler;

        private TextMeshProUGUI _weaponName;
        private TextMeshProUGUI _attack;
        private TextMeshProUGUI _attackCooldown;
        private TextMeshProUGUI _speed;
        private TextMeshProUGUI _effectRange;
        private TextMeshProUGUI _effectTime;
        private TextMeshProUGUI _projectileCount;
        private TextMeshProUGUI _penetraitCount;

        protected override void _Init()
        {
            _BindButton(typeof(EButtons));
            _BindText(typeof(ETexts));

            _BindEvent(_GetButton((int)EButtons.Spawn).gameObject, _SpawnMonster);
            _BindEvent(_GetButton((int)EButtons.LevelUp).gameObject, _LevelUp);
            _BindEvent(_GetButton((int)EButtons.LevelDown).gameObject, _LevelDown);
            _BindEvent(_GetButton((int)EButtons.NormalAttack).gameObject, _StartNormalAttack);
        }

        #region Event
        private void _SpawnMonster()
        {
            SpawnMonsterHandler?.Invoke();
        }

        private void _LevelUp()
        {
            LevelUpHandler?.Invoke();
        }

        private void _LevelDown()
        {
            LevelDownHandler?.Invoke();
        }

        private void _StartNormalAttack()
        {
            NormalAttackHandler?.Invoke();
        }
        #endregion

        public void SetWeaponName(string weaponName)
        {
            _weaponName.text = $"Weapon : {weaponName}";
        }

        public void SetAttack(float attack)
        {
            _attack.text = $"Attack : {attack}";
        }

        public void SetAttackCooldown(float attackCooldown)
        {
            _attackCooldown.text = $"AttackCooldown : {attackCooldown}";
        }

        public void SetSpeed(float speed)
        {
            _speed.text = $"Speed : {speed}";
        }

        public void SetEffectRange(float range)
        {
            _effectRange.text = $"EffectRange : {range}";
        }

        public void SeEffectTime(float effectTime)
        {
            _effectTime.text = $"EffectTime : {effectTime}";
        }

        public void SetProjectileCount(float projectileCount)
        {
            _projectileCount.text = $"ProjectileCount : {projectileCount}";
        }

        public void SetPenettraitCount(float penetraitCount)
        {
            _penetraitCount.text = $"PenetraitCount : {penetraitCount}";
        }
    }
}
