using UnityEngine;
namespace _Scripts.Interfaces
{
    public interface IAttackable
    {
        public void ReceiveDamage(Vector2 impulse, int damageAmount);
        public bool IsVulnerable();
        public void SetIsVulnerable(bool isVulnerable);
        public bool IsDead();
        public void OnDead();
    }
}