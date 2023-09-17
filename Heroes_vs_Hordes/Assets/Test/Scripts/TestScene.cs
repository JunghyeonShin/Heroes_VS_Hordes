namespace ProtoType
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public class TestScene : MonoBehaviour
    {
        [SerializeField] TestHeroController _testHeroController;
        [SerializeField] GameObject _rootTestMonster;
        [SerializeField] GameObject _testMonster;

        private Action _completeLoadingHandler;
        private ObjectPool _testMonsterPool = new ObjectPool();

        private const float DELAY_LOADING_TIME = 3f;
        private const float MAX_RANDOM_POS_X = 12.5f;
        private const float MAX_RANDOM_POS_Y = 20f;
        private const float MIN_DISTANCE_RANGE = 3f;
        private const int CREATE_TEST_MONSTER_COUNT = 50;
        private const int SPAWN_TEST_MONSTER_COUNT = 20;
        private const string RESOURCE_UI_TEST_SCENE = "UI_TestScene";

        private void Awake()
        {
            Manager.CreateInstance();

            // Test UI 持失
            Manager.Instance.UI.ShowSceneUI<UI_TestScene>(RESOURCE_UI_TEST_SCENE, (testSceneUI) =>
            {
                testSceneUI.SpawnMonsterHandler -= _SpawnMonster;
                testSceneUI.SpawnMonsterHandler += _SpawnMonster;
                testSceneUI.NormalAttackHandler -= _testHeroController.StartNormalAttack;
                testSceneUI.NormalAttackHandler += _testHeroController.StartNormalAttack;
                testSceneUI.LevelUpHandler -= _testHeroController.SetLevelUp;
                testSceneUI.LevelUpHandler += _testHeroController.SetLevelUp;
                testSceneUI.LevelDownHandler -= _testHeroController.SetLevelDown;
                testSceneUI.LevelDownHandler += _testHeroController.SetLevelDown;
            });

            // Loading UI 持失
            Manager.Instance.UI.ShowPopupUI<UI_Loading>(Define.RESOURCE_UI_LOADING, (loadingUI) =>
            {
                _completeLoadingHandler -= loadingUI.CompleteLoading;
                _completeLoadingHandler += loadingUI.CompleteLoading;
                loadingUI.StartLoading();

                _CheckLoadComplete().Forget();
            });

            _completeLoadingHandler -= _testHeroController.SetAbility;
            _completeLoadingHandler += _testHeroController.SetAbility;

            _testMonsterPool.InitPool(_testMonster, _rootTestMonster, CREATE_TEST_MONSTER_COUNT);
        }

        private void _SpawnMonster()
        {
            for (int ii = 0; ii < SPAWN_TEST_MONSTER_COUNT; ++ii)
            {
                var testMonsterGO = _testMonsterPool.GetObject();
                testMonsterGO.transform.position = _GetRandomPos();
                var testMonster = Utils.GetOrAddComponent<TestMonster>(testMonsterGO);
                testMonster.OnDieHandler -= _ReturnMonster;
                testMonster.OnDieHandler += _ReturnMonster;
                Utils.SetActive(testMonsterGO, true);
                ++ii;
            }
        }

        private void _ReturnMonster(GameObject testMonster)
        {
            _testMonsterPool.ReturnObject(testMonster);
        }

        private Vector3 _GetRandomPos()
        {
            var randomPosX = UnityEngine.Random.Range(-MAX_RANDOM_POS_X, MAX_RANDOM_POS_X);
            var randomPosY = UnityEngine.Random.Range(-MAX_RANDOM_POS_Y, MAX_RANDOM_POS_Y);
            var randomPos = new Vector3(randomPosX, randomPosY, 0f);
            var distance = Vector3.Distance(_testHeroController.transform.position, randomPos);
            if (distance <= MIN_DISTANCE_RANGE)
                return _GetRandomPos();
            return randomPos;
        }

        private async UniTaskVoid _CheckLoadComplete()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_LOADING_TIME));
            while (false == Manager.Instance.LoadComplete())
                await UniTask.Yield();

            _completeLoadingHandler?.Invoke();
        }
    }
}
