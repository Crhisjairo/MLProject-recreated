using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Enemies.Specs;
using _Scripts.Enums;
using _Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Enemy: MonoBehaviour, IAttackable
    {
        [SerializeField] private EnemySpecs modelSpecs;
        protected EnemySpecs specs;
        
        [SerializeField] private Color flashingColor = Color.red;
        public float autoDestroyTime = 1f;

        protected SpriteRenderer EnemySpriteRenderer;
        protected Rigidbody2D Rb;
        
        private bool _isVulnerable;

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

            specs = modelSpecs.GetCopy();
        }
        
        public virtual void ReceiveDamage(Vector2 impulse, int damageAmount)
        {
            specs.life -= damageAmount;
            
            StartCoroutine(FlashSprite());
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
            StartCoroutine(AutoDestroy());
        }

        public bool IsDead()
        {
            return specs.life <= 0;
        }
        
        private IEnumerator AutoDestroy()
        {
            EnemySpriteRenderer.material.color = Color.red;
                
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

        private void PushPlayer(PlayerController playerController)
        {
            // Calculates the impulse to send to the player.
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.Player.ToString()))
            {
                //TODO: play attacking animation when player collides
                PushPlayer(
                    other.collider.GetComponent<PlayerController>()
                    );
            }
        }

        public float GetImpulseForce()
        {
            return specs.forceImpulse;
        }
    }
}