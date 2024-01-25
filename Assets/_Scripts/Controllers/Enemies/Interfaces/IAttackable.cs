using UnityEngine;

namespace _Scripts.Controllers.Enemies.Interfaces
{
    public interface IAttackable
    {
        public void ReceiveDamage(Vector2 impulseDirection, int damageAmount);
        public bool IsVulnerable();
        public void SetIsVulnerable(bool isVulnerable);
        public bool IsDead();
        public void OnDead();
    }
}