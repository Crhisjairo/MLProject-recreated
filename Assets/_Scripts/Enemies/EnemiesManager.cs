using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Ambient;
using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        [SerializeField] private Enemy[] enemiesOnScene;
        [SerializeField] private Spawner[] spawners;
        
        private List<Enemy> _enemyList;

        private void Awake()
        {
            _enemyList = new List<Enemy>(enemiesOnScene);
        }

        private void Start()
        {
            AddSpawnersEnemiesReferences();
            Debug.Log(_enemyList.Count);
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
    }
}
