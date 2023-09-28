using System.Collections;
using _Scripts.Enemies;
using _Scripts.Interfaces;
using UnityEngine;
namespace _Scripts.Ambient
{
    public class Destructible : Enemy
    {
        public override void OnPauseAction()
        {
            throw new System.NotImplementedException();
        }
        public override void OnResumeAction()
        {
            throw new System.NotImplementedException();
        }
        public override void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            base.ReceiveDamage(impulseDirection, damageAmount);
            
            if (IsDead())
            {
                OnDead();
            }
        }
    }
}
