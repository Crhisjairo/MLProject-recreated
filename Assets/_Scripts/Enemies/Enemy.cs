using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Enemies.Specs;
using _Scripts.Enums;
using _Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Enemy: MonoBehaviour, IAttackable
    {
        [SerializeField] private EnemyDefaultSpecs modelDefaultSpecs;
        protected EnemyDefaultSpecs CurrentSpecs;

        private int worldId = 0; // TODO: used to know if item must be spawned or nor when load save data.
        
        [SerializeField] private Color flashingColor = Color.red;
        public float autoDestroyTime = 1f;

        protected SpriteRenderer EnemySpriteRenderer;
        protected Rigidbody2D Rb;
        protected BoxCollider2D BoxCollider2D;
        
        private bool _isVulnerable;
        
        protected Vector2 _impulseDirection;
        protected bool _inImpulse = false;
        private const float ImpulseTime = .15f;

        protected virtual void Awake()
        {
            SetComponents();
        }
        
        public abstract void OnPauseAction();

        public abstract void OnResumeAction();
        
        private void SetComponents()
        {
            Rb = GetComponent<Rigidbody2D>();
            EnemySpriteRenderer = GetComponent<SpriteRenderer>();
            BoxCollider2D = GetComponent<BoxCollider2D>();

            CurrentSpecs = modelDefaultSpecs.GetCopy();
        }
        
        public virtual void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            CurrentSpecs.life -= damageAmount;
            
            StartCoroutine(FlashSprite());
            StartCoroutine(ActivateImpulseCounter(impulseDirection));
        }

        public bool IsVulnerable()
        {
            return _isVulnerable;
        }

        public void SetIsVulnerable(bool isVulnerable)
        {
            _isVulnerable = isVulnerable;
        }
        
        public void OnDead()
        {
            _inImpulse = false;
            
            StartCoroutine(AutoDestroy());
        }

        public bool IsDead()
        {
            return CurrentSpecs.life <= 0;
        }
        
        private IEnumerator AutoDestroy()
        {
            EnemySpriteRenderer.material.color = Color.red;
            BoxCollider2D.enabled = false;

            //TODO: Add dead sound effect here.
            
            yield return new WaitForSeconds(autoDestroyTime);
            Destroy(gameObject);
        }
        
        private IEnumerator FlashSprite()
        {
            for (int i = 0; i < 4; i++)
            {
                var material = EnemySpriteRenderer.material;
                
                material.color = flashingColor;
                yield return new WaitForSeconds(0.09f);
                
                material.color = Color.white;
                yield return new WaitForSeconds(0.09f);
            }
        }
        
        private IEnumerator ActivateImpulseCounter(Vector2 impulseDirection)
        {
            _inImpulse = true;
            _impulseDirection = impulseDirection;
            
            yield return new WaitForSeconds(ImpulseTime);
            
            _inImpulse = false;
            _impulseDirection = new Vector2(0,0);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.Player.ToString()))
            {
                //TODO: play attacking animation when player collides
                PlayerController playerController = other.collider.GetComponentInParent<PlayerController>();
                
                //Vector opuesto para el jugador
                Vector2 playerImpulseDir = playerController.transform.position - transform.position;
                playerImpulseDir = playerImpulseDir.normalized * CurrentSpecs.forceImpulse;

                Debug.Log(playerImpulseDir);
            
                playerController.ReceiveDamage(playerImpulseDir, CurrentSpecs.damage);
            }
        }
    }
}