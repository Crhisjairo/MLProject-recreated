using System.Collections;
using _Scripts.Enemies;
using UnityEngine;
namespace _Scripts.Ambient
{
    public class Destructible : Enemy
    {
        [SerializeField] private bool isDamagable = true;
        [SerializeField] private bool canMakeDamage = false;
        
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
            if(!isDamagable)
                return;
            
            base.ReceiveDamage(impulseDirection, damageAmount);
            
            if (IsDead())
            {
                OnDead();
            }
        }

        protected override void AttackEntityOnCollider(Collider2D collider2D)
        {
            if(canMakeDamage)
                base.AttackEntityOnCollider(collider2D);
            
        }
    }
}
