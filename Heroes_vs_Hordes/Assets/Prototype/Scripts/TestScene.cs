namespace ProtoType
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TestScene : MonoBehaviour
    {
        [SerializeField] private TestRepositionBackground[] _testRepositionBackgrounds;
        [SerializeField] private TestHeroController _testHeroController;

        private ObjectPool _testMonsterPool = new ObjectPool();
        private Queue<GameObject> _useTestMonsterQueue = new Queue<GameObject>();

        private const float MAX_RANDOM_POS_X = 12.5f;
        private const float MAX_RANDOM_POS_Y = 20f;
        private const int CREATE_MONSTER_COUNT = 50;
        private const int EMPTY_VALUE = 0;
        private const int MIN_CREATE_MONSTER_COUNT = 2;
        private const int MAX_CREATE_MONSTER_COUNT = 5;
        private const string NAME_ROOT_POOL = "[ROOT_POOL]";
        private const string RESOURCE_TEST_MONSTER = "Normal_Bat";
        private const string RESOURCE_UI_TEST_SCENE = "UI_TestScene";

        private void Awake()
        {
            Manager.CreateInstance();

            // Test UI 积己
            Manager.Instance.UI.ShowSceneUI<UI_TestScene>(RESOURCE_UI_TEST_SCENE, (testSceneUI) =>
            {
                testSceneUI.OnClickSpawnButton -= _SpawnMonster;
                testSceneUI.OnClickSpawnButton += _SpawnMonster;
                testSceneUI.OnClickReturnButton -= _ReturnAllMonster;
                testSceneUI.OnClickReturnButton += _ReturnAllMonster;
            });

            // Reposition Background 窍扁 困茄 技泼
            for (int ii = 0; ii < _testRepositionBackgrounds.Length; ++ii)
                _testRepositionBackgrounds[ii].HeroController = _testHeroController;

            // Test Monster 积己
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
                monsterController.Target = _testHeroController.transform;
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
