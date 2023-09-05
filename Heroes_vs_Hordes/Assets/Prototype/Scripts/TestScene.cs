namespace ProtoType
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TestScene : MonoBehaviour
    {
        private GameObject _testHero;
        private ObjectPool _testMonsterPool = new ObjectPool();
        private Queue<GameObject> _useTestMonsterQueue = new Queue<GameObject>();

        private const float MAX_RANDOM_POS_X = 12.5f;
        private const float MAX_RANDOM_POS_Y = 20f;
        private const int CREATE_MONSTER_COUNT = 50;
        private const int EMPTY_VALUE = 0;
        private const int MIN_CREATE_MONSTER_COUNT = 2;
        private const int MAX_CREATE_MONSTER_COUNT = 5;
        private const string NAME_ROOT_POOL = "[ROOT_POOL]";
        private const string RESOURCE_TEST_HERO = "ArcaneMage";
        private const string RESOURCE_TEST_MONSTER = "Normal_Bat";
        private const string RESOURCE_UI_TEST_SCENE = "UI_TestScene";

        private void Awake()
        {
            Manager.CreateInstance();

            // Test UI 持失
            Manager.Instance.UI.ShowSceneUI<UI_TestScene>(RESOURCE_UI_TEST_SCENE, (testSceneUI) =>
            {
                testSceneUI.OnClickSpawnButton -= _SpawnMonster;
                testSceneUI.OnClickSpawnButton += _SpawnMonster;
                testSceneUI.OnClickReturnButton -= _ReturnAllMonster;
                testSceneUI.OnClickReturnButton += _ReturnAllMonster;
            });

            // Test Hero 持失
            Manager.Instance.Resource.Instantiate(RESOURCE_TEST_HERO, null, (arcaneMage) =>
            {
                _testHero = arcaneMage;
                var heroController = Utils.GetOrAddComponent<TestHeroController>(_testHero);
            });

            // Test Monster 持失
            var poolGO = new GameObject(NAME_ROOT_POOL);

            Manager.Instance.Resource.LoadAsync<GameObject>(RESOURCE_TEST_MONSTER, (normalBat) =>
            {
                _testMonsterPool.InitPool(normalBat, poolGO, CREATE_MONSTER_COUNT);
            });
        }

        private void _SpawnMonster()
        {
            var randomCreateCount = Random.Range(MIN_CREATE_MONSTER_COUNT, MAX_CREATE_MONSTER_COUNT);

            for (int ii = 0; ii < randomCreateCount; ++ii)
            {
                var randomPosX = Random.Range(-MAX_RANDOM_POS_X, MAX_RANDOM_POS_X);
                var randomPosY = Random.Range(-MAX_RANDOM_POS_Y, MAX_RANDOM_POS_Y);

                var monster = _testMonsterPool.GetObject();
                monster.transform.localPosition = new Vector3(randomPosX, randomPosY, 0f);
                _useTestMonsterQueue.Enqueue(monster);

                var monsterController = Utils.GetOrAddComponent<TestMonsterController>(monster);
                monsterController.Target = _testHero.transform;
                monsterController.ReturnMonsterHandler -= _ReturnMonster;
                monsterController.ReturnMonsterHandler += _ReturnMonster;

                Utils.SetActive(monster, true);
            }
        }

        private void _ReturnAllMonster()
        {
            while (_useTestMonsterQueue.Count > EMPTY_VALUE)
            {
                var monster = _useTestMonsterQueue.Dequeue();
                if (monster.activeSelf)
                    _ReturnMonster(monster);
            }
        }

        private void _ReturnMonster(GameObject monster)
        {
            _testMonsterPool.ReturnObject(monster);
        }
    }
}
