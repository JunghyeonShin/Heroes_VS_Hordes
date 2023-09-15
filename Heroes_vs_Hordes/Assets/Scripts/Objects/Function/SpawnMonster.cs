using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    public HeroController HeroController { get; set; }

    public void Spawn(string key, int count)
    {
        for (int ii = 0; ii < count; ++ii)
        {
            Manager.Instance.Object.GetMonster(key, (monsterGO) =>
            {
                var monster = Utils.GetOrAddComponent<Monster>(monsterGO);
                monster.Target = HeroController.transform;

                var randomPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length - 1)];
                monster.transform.position = randomPoint.position;
                Utils.SetActive(monster.gameObject, true);

                var repositionMonster = Utils.GetOrAddComponent<RepositionMonster>(monsterGO);
                repositionMonster.HeroController = HeroController;

                Manager.Instance.Ingame.EnqueueUsedMonster(monster);
            });
        }
    }
}
