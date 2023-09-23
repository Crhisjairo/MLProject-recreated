using System.Collections;
using _Scripts.Interfaces;
using UnityEngine;
namespace _Scripts.Ambient
{
    public class Destructible : MonoBehaviour, IAttackable
    {
        [SerializeField] private int currentLife = 10;
        [SerializeField] private float waitTimeBeforeDestruct = 1f;

        private bool _isVulnerable;

        public void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            currentLife -= damageAmount;
            if (IsDead())
            {
                OnDead();
            }
        }

        public bool IsVulnerable()
        {
            return _isVulnerable;
        }
        
        public void SetIsVulnerable(bool isVulnerable)
        {
            _isVulnerable = isVulnerable;
        }
        
        public bool IsDead()
        {
            return currentLife <= 0;
        }
        
        public void OnDead()
        {
            StartCoroutine(StartDie());
        }
        
        private IEnumerator StartDie()
        {
            yield return new WaitForSeconds(waitTimeBeforeDestruct);
            Destroy(gameObject);
        }
    }
}
