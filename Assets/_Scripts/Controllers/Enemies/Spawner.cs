using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Controllers.Enemies
{
    public class Spawner : Enemy
    {
        [SerializeField] private GameObject modelToSpawn;

        [SerializeField] private bool spawnOneShot = true;

        [SerializeField] private float waitTimeBeforeNextSpawn = 1f;

        [SerializeField] private int maxEntities = 5;

        [SerializeField] private Vector2 spawnOffSet;

        private Queue<GameObject> _entities;
        
        [SerializeField] private bool isDamagable = true;
        [SerializeField] private bool canMakeDamage;

        protected override void Awake()
        {
            base.Awake();
            
            _entities = new Queue<GameObject>();

            for (int i = 0; i < maxEntities; i++)
            {
                Vector2 entityPosition = (Vector2) transform.position + spawnOffSet;
                
                var entity = Instantiate(modelToSpawn, entityPosition, transform.rotation, transform.parent);
                
                entity.SetActive(false);
                
                _entities.Enqueue(entity);
            }
        }

        private void Start()
        {
            if (spawnOneShot)
            {
                SpawnAllOneShot();
            }
            else
            {
                StartCoroutine(StartSpawning());
            }
        }

        private void SpawnAllOneShot()
        {
            while (_entities.Count > 0)
            {
                var entity = _entities.Dequeue();
                entity.SetActive(true);
            }
        }

        public GameObject[] GetEntitiesReferences()
        {
            return _entities.ToArray();
        }
        
        private IEnumerator StartSpawning()
        {
            while (_entities.Count > 0)
            {
                _entities.Dequeue().SetActive(true);
                
                yield return new WaitForSeconds(waitTimeBeforeNextSpawn);
            }
            
        }
        
        public override void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            if(!isDamagable)
                return;
            
            base.ReceiveDamage(impulseDirection, damageAmount);
            
            if (IsDead())
            {
                foreach (var entity in _entities)
                {
                    entity.SetActive(true);
                    entity.GetComponent<Enemy>().OnDead();
                }       
                
                OnDead();
            }
        }

        protected override void AttackEntityOnCollider(Collider2D entityCollider)
        {
            if(canMakeDamage)
                base.AttackEntityOnCollider(entityCollider);
        }
    }
}
