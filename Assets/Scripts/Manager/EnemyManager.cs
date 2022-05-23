using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Entropy.TD.Tower;

namespace Entropy.TD.Entities
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        [BoxGroup("Spawn Settings")] public List<Enemy> enemyPrefabs;
        [BoxGroup("Spawn Settings")] public AnimationCurve spawnCooldownCurve, spawnAmountCurve;

        [BoxGroup("Read Only")] [SerializeField] [ReadOnly] float _cooldown;


        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            GameManager.Instance.Wave = 0;
        }
        public void Update()
        {
            _cooldown -= Time.deltaTime;

            if (_cooldown <= 0f)
            {
                _cooldown = spawnCooldownCurve.Evaluate(GameManager.Instance.GameTime);
                GameManager.Instance.Wave++;
                StartCoroutine(SpawnEnemies());
            }
        }
        IEnumerator SpawnEnemies()
        {
            for (int i = 0; i < Mathf.RoundToInt(spawnAmountCurve.Evaluate(GameManager.Instance.GameTime)); i++)
            {
                yield return new WaitForSeconds(0.4f);
                Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], GameManager.Instance.startPath.position, Quaternion.identity);
            }
        }
    }
}