using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    public Transform Target { get; set; }

    public void Spawn(string key, int count)
    {
        for (int ii = 0; ii < count; ++ii)
        {
            Manager.Instance.Object.GetMonster(key, (monster) =>
            {
                var monsterController = Utils.GetOrAddComponent<MonsterController>(monster);
                monsterController.Target = Target;

                var randomPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length - 1)];
                monsterController.transform.position = randomPoint.position;
                Utils.SetActive(monsterController.gameObject, true);
            });
        }
    }
}