namespace ProtoType
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_TestScene : UI_Scene
    {
        private enum EButtons
        {
            SpawnButton,
            ReturnButton
        }

        public event Action OnClickSpawnButton;
        public event Action OnClickReturnButton;

        protected override void _Init()
        {
            _BindButton((typeof(EButtons)));

            _BindEvent(_GetButton((int)EButtons.SpawnButton).gameObject, _SpawnMonster);
            _BindEvent(_GetButton((int)EButtons.ReturnButton).gameObject, _ReturnMonster);
        }

        private void _SpawnMonster()
        {
            OnClickSpawnButton?.Invoke();
        }

        private void _ReturnMonster()
        {
            OnClickReturnButton?.Invoke();
        }
    }
}
