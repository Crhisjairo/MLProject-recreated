using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Controllers.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private Enemy[] enemiesOnScene;
        [SerializeField] private Spawner[] spawners;

        
        public UnityEvent<int> onEnemiesSet;
        public UnityEvent<int> onEnemiesRemoveUpdate;
        public UnityEvent onEnemiesEmpty;

        private List<Enemy> _enemyList;

        private void Awake()
        {
            _enemyList = new List<Enemy>(enemiesOnScene);
        }

        private void Start()
        {
            AddSpawnersEnemiesReferences();
            SubscribeToDestroyEnemyEvents();
            
            onEnemiesSet?.Invoke(_enemyList.Count);
            
            Debug.Log(_enemyList.Count);
        }

        private void SubscribeToDestroyEnemyEvents()
        {
            foreach (var enemy in _enemyList)
            {
                enemy.onDestroyEvent.AddListener(RemoveEnemyReference);
            }
        }

        public void PauseAllEnemies()
        {
            foreach (var enemy in _enemyList)
            {
                if(enemy)
                    enemy.OnPauseAction();           
            }
        }

        public void ResumeAllEnemies()
        {
            foreach (var enemy in _enemyList)
            {
                if(enemy)
                    enemy.OnResumeAction();
            }
        }

        private void AddSpawnersEnemiesReferences()
        {
            foreach (var spawner in spawners)
            {
               Enemy[] enemies = spawner
                   .GetEntitiesReferences()
                   .Select(entity => {
                       return entity.GetComponent<Enemy>(); 
                   })
                   .ToArray();
                
                _enemyList.AddRange(enemies);
            }
            
        }

        private void RemoveEnemyReference(Enemy enemy)
        {
            _enemyList.Remove(enemy);
            onEnemiesRemoveUpdate?.Invoke(_enemyList.Count);

            if (_enemyList.Count <= 0)
            {
                onEnemiesEmpty?.Invoke();
            }
            
            Debug.Log("Enemies: " + _enemyList.Count);
        }
    }
}
