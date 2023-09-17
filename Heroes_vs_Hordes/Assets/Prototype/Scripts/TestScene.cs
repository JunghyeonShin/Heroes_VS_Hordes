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
        private const float MIN_RANDOM_POS_X = 4f;
        private const float MAX_RANDOM_POS_X = 12.5f;
        private const float MIN_RANDOM_POS_Y = 5f;
        private const float MAX_RANDOM_POS_Y = 20f;
        private const int CREATE_TEST_MONSTER_COUNT = 50;
        private const int SPAWN_TEST_MONSTER_COUNT = 5;
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
            });

            // Loading UI 持失
            Manager.Instance.UI.ShowPopupUI<UI_Loading>(Define.RESOURCE_UI_LOADING, (loadingUI) =>
            {
                _completeLoadingHandler -= loadingUI.CompleteLoading;
                _completeLoadingHandler += loadingUI.CompleteLoading;
                loadingUI.StartLoading();

                _CheckLoadComplete().Forget();
            });

            _testMonsterPool.InitPool(_testMonster, _rootTestMonster, CREATE_TEST_MONSTER_COUNT);
        }

        private void _SpawnMonster()
        {
            for (int ii = 0; ii < SPAWN_TEST_MONSTER_COUNT; ++ii)
            {
                var randomPosX = _GetRandomPos(MIN_RANDOM_POS_X, MAX_RANDOM_POS_X);
                var randomPosY = _GetRandomPos(MIN_RANDOM_POS_Y, MAX_RANDOM_POS_Y);

                var testMonsterGO = _testMonsterPool.GetObject();
                testMonsterGO.transform.position = new Vector3(randomPosX, randomPosY, 0f);
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

        private float _GetRandomPos(float minRange, float maxRange)
        {
            var random = UnityEngine.Random.Range(-maxRange, maxRange);
            if (MathF.Abs(random) < minRange)
                _GetRandomPos(maxRange, minRange);
            return random;
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
