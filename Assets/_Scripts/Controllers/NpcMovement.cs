using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enums;
using _Scripts.Utils;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class NpcMovement : MonoBehaviour
    {
        public bool moveBetweenPoints;
        public Transform[] pointsToMove;

        [Range(-1f, -10f)] public float speed = 0.5f;

        public float timeToWaitBeforeNextPoint = 2;

        public bool lookAtFixedDirection = false;
        public LookingDirection fixedLookingDirection = LookingDirection.Down;

        public bool loopMovement = false;
        
        private Rigidbody2D _rb;
        [SerializeField] private Animator animator;

        private float _currentTime;
        private bool _moving;

        private List<Vector2> _pointsToMoveList;
        private int _nextPointIndex = 1;
        
        private Vector2 _startPos, _endPos;
        private Vector2 _calculatedVelocity;

        private void Awake()
        {
            SetComponents();
        }

        private void Start()
        {
            if (!moveBetweenPoints) return;
            
            PreparePointsToMoveList();
            StartAutoMoving();
        }

        private void StartAutoMoving()
        {
            if (pointsToMove.IsEmpty())
            {
                var message = string.Format(ConsoleMessages.NoPointsAssignedToNpc, name);
                Debug.LogWarning(message);
                return;
            }

            _moving = true;
            
            NextMove();
        }
        
        private void Update()
        {
            if (!_moving) return;

            Vector2 currentPos = transform.position;
            var nextPos = _pointsToMoveList[_nextPointIndex];

            if ((currentPos - nextPos).sqrMagnitude < 0.01f)
            {
                _nextPointIndex++;

                if (_nextPointIndex > _pointsToMoveList.Count - 1)
                {
                    if (!loopMovement)
                    {
                        PauseMovement();
                        Debug.Log("no loopeamos. Paramos!");
                        return;
                    }
                    
                    _nextPointIndex = 0;
                }
                
                NextMove();
            }
        }
        
        private void SetSpeedAnimationValue(bool isActive)
        {
            float value = 0;

            if (isActive)
                value = 1;
            
            animator.SetFloat(CharacterAnimationParameters.Speed.ToString(), value);
        }

        private Vector2 CalculateNextDirectionNormalized()
        {
            Vector2 currentPos = transform.position;
            Vector2 nextPos = _pointsToMoveList[_nextPointIndex];

           return (currentPos - nextPos).normalized;
        }
        
        private void SetVelocity(Vector2 directionNormalized)
        {
            _calculatedVelocity = directionNormalized * speed;
            
            _rb.velocity = _calculatedVelocity;
        }

        private void SetLookingDirection(Vector2 directionNormalized = new Vector2(), 
            LookingDirection defaultLookingDirection = LookingDirection.None)
        {
            float horizontalValue = 0, verticalValue = 0;
            
            if (directionNormalized.x > 0.05 || defaultLookingDirection == LookingDirection.Left)
            {
                horizontalValue = -1;
                verticalValue = 0;
            }
        
            if (directionNormalized.x < -0.05 || defaultLookingDirection == LookingDirection.Right)
            {
                horizontalValue = 1;
                verticalValue = 0;
            }
            
            if (directionNormalized.y > 0.05 || defaultLookingDirection == LookingDirection.Down)
            {
                horizontalValue = 0;
                verticalValue = -1;
            }
            
            if (directionNormalized.y < -0.05 || defaultLookingDirection == LookingDirection.Up)
            {
                horizontalValue = 0;
                verticalValue = 1; 
            }
            
            animator.SetFloat(CharacterAnimationParameters.LastHorizontal.ToString(), horizontalValue);
            animator.SetFloat(CharacterAnimationParameters.LastVertical.ToString(), verticalValue);
        }
        
        public void PauseMovement()
        {
            _moving = false;
            _rb.velocity = new Vector2();
            
            SetSpeedAnimationValue(_moving);
        }

        public void ResumeMovement()
        {
            _moving = true;

            NextMove();
        }

        /// <summary>
        /// Make the next NPC move applying its own rigidbody velocity and animation.
        ///
        /// Just calle once, not every frame.
        /// </summary>
        private void NextMove()
        {
            var directionNormalized = CalculateNextDirectionNormalized();
            
            SetVelocity(directionNormalized);

            if (!lookAtFixedDirection)
                SetLookingDirection(directionNormalized: directionNormalized);
            else
                SetLookingDirection(defaultLookingDirection: fixedLookingDirection);


            SetSpeedAnimationValue(true);
        }
        
        private void PreparePointsToMoveList()
        {
            _pointsToMoveList.Add(transform.position);

            foreach (var transform in pointsToMove)
            {
                _pointsToMoveList.Add(transform.position);
                Debug.Log(transform);
            }
        }

        private void SetComponents()
        {
            _rb = GetComponent<Rigidbody2D>();

            _pointsToMoveList = new List<Vector2>();
        }
    }
}
