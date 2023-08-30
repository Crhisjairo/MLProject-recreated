using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enums;
using _Scripts.Utils;
using SuperTiled2Unity;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class NpcMovement : MonoBehaviour
    {
        public bool MoveBetweenPoints;
        public Transform[] PointsToMove;

        [Range(-1f, -10f)] public float Speed = 0.5f;

        public float TimeToWaitBeforeNextPoint = 2;

        public bool LookAtFixedDirection = false;
        public LookingDirection LookingDirection = LookingDirection.Down;

        private Rigidbody2D _rb;
        private Animator _animator;

        private float _currentTime;
        private bool _moving;

        private List<Vector2> _pointsToMoveList;
        private int _nextPointIndex = 1;
        
        private Vector2 _startPos, _endPos;
        private Vector2 _velocity;

        private void Awake()
        {
            SetComponents();
        }

        private void Start()
        {
            if (!MoveBetweenPoints) return;
            
            PreparePointsToMoveList();
            StartAutoMoving();
        }

        private void StartAutoMoving()
        {
            if (PointsToMove.IsEmpty())
            {
                var message = string.Format(ConsoleMessages.NoPointsAssignedToNpc, name);
                Debug.LogWarning(message);
                return;
            }

            DefineNewVelocity();
            _moving = true;
        }
        
        private void Update()
        {
            if (!_moving) return;

            Vector2 currentPos = transform.position;
            Vector2 nextPos = _pointsToMoveList[_nextPointIndex];
            
            Debug.Log((currentPos - nextPos).sqrMagnitude < 0.01f);

            if ((currentPos - nextPos).sqrMagnitude < 0.01f)
            {
                Debug.Log("Llegamos al punto! Definimos nueva velocidad");

                _nextPointIndex++;
                _nextPointIndex = Mathf.Clamp(_nextPointIndex, 0, _pointsToMoveList.Count);
                
                DefineNewVelocity();
            }
        }

        private void DefineNewVelocity()
        {
            Vector2 currentPos = transform.position;
            Vector2 nextPos = _pointsToMoveList[_nextPointIndex];
                
            var newVelocity = (currentPos - nextPos).normalized * Speed;
            _rb.velocity = newVelocity;
        }

        private void PreparePointsToMoveList()
        {
            _pointsToMoveList.Add(transform.position);

            foreach (var transform in PointsToMove)
            {
                _pointsToMoveList.Add(transform.position);
            }
        }

        private void SetComponents()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            _pointsToMoveList = new List<Vector2>();
        }
    }
}
