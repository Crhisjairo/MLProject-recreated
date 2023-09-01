using _Scripts.Enemies.Specs;
using _Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.Enemies
{
    public class Enemy: MonoBehaviour, IAttackable
    {
        [SerializeField] private EnemySpecs specs;
        
        public void ReceiveAttack(int damage)
        {
            specs.damage -= damage;
        }

        public bool IsDead()
        {
            return specs.damage <= 0;
        }

        public void IsVulnerable()
        {
            throw new System.NotImplementedException();
        }

        public void SetIsVulnerable(bool isVulnerable)
        {
            throw new System.NotImplementedException();
        }
    }
}