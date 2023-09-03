using System;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Squid : Enemy
    {
        [SerializeField] private Slider lifeSlider;
        [SerializeField] private SpriteRenderer exclamationSpriteRenderer;

        [SerializeField] private float movementSpeed = 2;
        
        private CircleCollider2D _rangeCollider;
        
        private Vector2 _startPoint, _nextDirection;

        private const int MinLifeSliderValue = 0;

        private bool _isMoving = true;
        
        protected override void Awake()
        {
            base.Awake();
            
            SetComponents();
        }

        private void Start()
        {
            SetSliderValues();
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;

            Rb.MovePosition(Rb.position + _nextDirection * (movementSpeed * Time.fixedDeltaTime));
        }

        private void SetSliderValues()
        {
            lifeSlider.maxValue = specs.life;
            lifeSlider.minValue = MinLifeSliderValue;
      
            lifeSlider.value = specs.life;
        }

        public override void ReceiveDamage(Vector2 impulse, int damageAmount)
        {
            base.ReceiveDamage(impulse, damageAmount);
            
            lifeSlider.value = specs.life;

            if (IsDead())
            {
                _isMoving = false;
                
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
        
        private void SetComponents()
        {
            _rangeCollider = GetComponent<CircleCollider2D>();
        }
    }
}
