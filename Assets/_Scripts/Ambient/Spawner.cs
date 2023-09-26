using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Scripts.Ambient
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject modelToSpawn;

        [SerializeField] private bool spawnOneShot = true;

        [SerializeField] private float waitTimeBeforeNextSpawn = 1f;

        [SerializeField] private int maxEntities = 5;

        [SerializeField] private Vector2 spawnOffSet = new Vector2();
        
        private Queue<GameObject> _entities;

        private void Awake()
        {
            _entities = new Queue<GameObject>();

            for (int i = 0; i < maxEntities; i++)
            {
                Vector2 entityPosition = (Vector2) transform.position + spawnOffSet;
                
                var entity = Instantiate(modelToSpawn, entityPosition, transform.rotation, transform);
                
                entity.SetActive(false);
                
                _entities.Enqueue(entity);
            }
        }

        private void Start()
        {
            if (spawnOneShot)
            {
                SpawnOneShot();
            }
            else
            {
                StartCoroutine(StartSpawning());
            }
        }

        private void SpawnOneShot()
        {
            
            while (_entities.Count > 0)
            {
                var entity = _entities.Dequeue();
                entity.SetActive(true);
            }
        }

        private IEnumerator StartSpawning()
        {
            while (_entities.Count > 0)
            {
                _entities.Dequeue().SetActive(true);
                
                yield return new WaitForSeconds(waitTimeBeforeNextSpawn);
            }
            
            yield break;
        }
    }
}