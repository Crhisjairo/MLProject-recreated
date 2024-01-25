using UnityEngine;
using _Scripts.Controllers.Enemies.Interfaces;

namespace _Scripts.Ambient
{
    public class Destructible: MonoBehaviour, IAttackable
    {
        [SerializeField] private int life;

        public void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            if (IsDead())
            {
                OnDead();
            }

            life -= damageAmount;
        }

        public bool IsVulnerable()
        {
            // TODO: implement a destructible objet by the current character.
            // e.g: a specific character can break a rock
            throw new System.NotImplementedException();
        }

        public void SetIsVulnerable(bool isVulnerable)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDead()
        {
            return life <= 0;
        }

        public void OnDead()
        {
            Destroy(gameObject);
        }
    }
}
