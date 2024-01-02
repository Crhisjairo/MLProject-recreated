using System;
using System.Collections;
using _Scripts.Controllers.Enemies.Interfaces;
using _Scripts.Enemies.Specs;
using _Scripts.Enums;
using _Scripts.Shared.Enums;
using _Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Controllers.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SoundFXEmitter))]
    public abstract class Enemy: MonoBehaviour, IAttackable, IPausable
    {
        [SerializeField] protected BaseEnemySpecs baseSpecs;
        protected SoundFXEmitter SoundFXEmitter;

        // TODO: used to know if item must be spawned or nor when load save data.
        
        [SerializeField] private Color flashingColor = Color.red;
        public float autoDestroyTime = 1f;

        [SerializeField] protected SpriteRenderer EnemySpriteRenderer;
        protected Rigidbody2D Rb;
        protected BoxCollider2D BoxCollider2D;

        private bool _isVulnerable;

        public UnityEvent<Enemy> onDestroyEvent;
        
        protected Vector2 _impulseDirection;
        protected bool _inImpulse = false;
        private const float ImpulseTime = .15f;

        protected bool IsMovementPaused = false;

        protected virtual void Awake()
        {
            SetComponents();
        }

        public virtual void OnPauseAction()
        {
            IsMovementPaused = true;
        }

        public virtual void OnResumeAction()
        {
            IsMovementPaused = false;
        }
        
        private void SetComponents()
        {
            Rb = GetComponent<Rigidbody2D>();
            BoxCollider2D = GetComponent<BoxCollider2D>();
            SoundFXEmitter = GetComponent<SoundFXEmitter>();
        }
        
        public virtual void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            baseSpecs.life -= damageAmount;
            PlaySoundSfx(SoundsFX.Damaged);
            
            StartCoroutine(FlashSprite());
            StartCoroutine(ActivateImpulseCounter(impulseDirection));
        }

        protected void PlaySoundSfx(SoundsFX sound)
        {
            SoundFXEmitter.PlayOneShot(sound.ToString());
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
            PlaySoundSfx(SoundsFX.Died);
            
            StartCoroutine(AutoDestroy());
        }

        public bool IsDead()
        {
            return baseSpecs.life <= 0;
        }
        
        private IEnumerator AutoDestroy()
        {
            EnemySpriteRenderer.material.color = Color.red;
            BoxCollider2D.enabled = false;

            //TODO: Add dead sound effect here.
            //onDestroyEvent?.Invoke(this);
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
                AttackEntityOnCollider(other.collider);
            }
        }

        //TODO: Optimize this fonction
        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.Player.ToString()))
            {
                AttackEntityOnCollider(other.collider);
            }
        }

        protected virtual void AttackEntityOnCollider(Collider2D collider)
        {
            //TODO: play attacking animation when player collides
            PlayerController playerController = collider.GetComponentInParent<PlayerController>();
                
            //Vector opuesto para el jugador
            Vector2 playerImpulseDir = playerController.transform.position - transform.position;
            playerImpulseDir = playerImpulseDir.normalized * baseSpecs.forceImpulse;

//            Debug.Log(playerImpulseDir);
            
            playerController.ReceiveDamage(playerImpulseDir, baseSpecs.damage);
        }

        private void OnDestroy()
        {
            onDestroyEvent?.Invoke(this);
        }
    }
}