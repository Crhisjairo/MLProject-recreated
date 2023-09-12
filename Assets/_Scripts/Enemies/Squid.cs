using System;
using System.Collections;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
namespace _Scripts.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Squid : Enemy
    {
        [SerializeField] private Slider lifeSlider;
        [SerializeField] private SpriteRenderer exclamationSpriteRenderer;

        [SerializeField] private float movementSpeed = 2;

        [SerializeField] private float nextDirectionTime = 1f;

        [SerializeField] private bool randomMove = true;
        
        private CircleCollider2D _rangeCollider;
        
        private Vector2 _startPoint, _nextDirection;

        private const int MinLifeSliderValue = 0;

        private Coroutine _nextRandomMovementCoroutine;
        
        protected override void Awake()
        {
            base.Awake();
            
            SetComponents();
            
            SetSliderValues();
            exclamationSpriteRenderer.enabled = false;
        }

        private void Start()
        {
            if (randomMove)
                _nextRandomMovementCoroutine = StartCoroutine(CalculateNextRandomMovementCoroutine());
        }

        private void FixedUpdate()
        {
            if (_inImpulse)
            {
                Rb.AddForce(_impulseDirection, ForceMode2D.Force);
            }
            

            Rb.MovePosition(Rb.position + _nextDirection * (movementSpeed * Time.fixedDeltaTime));
        }

        private void SetSliderValues()
        {
            lifeSlider.maxValue = CurrentSpecs.life;
            lifeSlider.minValue = MinLifeSliderValue;
      
            lifeSlider.value = CurrentSpecs.life;
        }

        public override void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            base.ReceiveDamage(impulseDirection, damageAmount);
            
            lifeSlider.value = CurrentSpecs.life;

            if (IsDead())
            {
                movementSpeed = 0;
                
                OnDead();
            }
        }

        public override void OnPauseAction()
        {
            throw new NotImplementedException();
        }
        public override void OnResumeAction()
        {
            throw new NotImplementedException();
        }
        
        private IEnumerator CalculateNextRandomMovementCoroutine()
        {
            while (true)
            {
                CalculateNextRandomDirection();
                yield return new WaitForSeconds(nextDirectionTime);
            }
        }
        
        private void CalculateNextRandomDirection()
        {
            float x = Random.Range(_startPoint.x - 2, _startPoint.x + 2);
            float y = Random.Range(_startPoint.y - 2, _startPoint.y + 2);
        
            Vector2 ranPos = new Vector2(x, y);
        
            _nextDirection = _startPoint - ranPos;
            _nextDirection.Normalize();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player.ToString()))
            {
                exclamationSpriteRenderer.enabled = true;
                   
                if(_nextRandomMovementCoroutine is not null)
                    StopCoroutine(_nextRandomMovementCoroutine); 
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player.ToString()))
            {
                //Seguimos al jugador
                Vector2 playerPos = other.transform.position;

                _nextDirection = -(Rb.position - playerPos);
                _nextDirection.Normalize();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player.ToString()))
            {
                _nextDirection = new Vector2();
                
                exclamationSpriteRenderer.enabled = false;
                //Volvemos a movernos aleatoriamente ignorando al jugador
                if (randomMove)
                    _nextRandomMovementCoroutine = StartCoroutine(CalculateNextRandomMovementCoroutine());
            }
        }
        
        private void SetComponents()
        {
            _rangeCollider = GetComponent<CircleCollider2D>();
        }
    }
}
