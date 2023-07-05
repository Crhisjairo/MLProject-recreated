using System;
using UnityEngine;

namespace _Scripts.Characters
{
    
    //TODO move this class to Character.
    [Serializable]
    public class CharacterSpecs
    {
        [SerializeField] private static int currentCoins = 0;
        
        public int Id = 0;

        public string characterName;

        [SerializeField] int maxLife = 13;
        [SerializeField] int currentLife;

        [SerializeField] float speed = 2f;
        [SerializeField] float runningSpeed = 3f;
        [SerializeField] int attackDamage = 1;
        
        public void TakeDamage(int damage)
        {
            currentLife -= damage;
        }

        public bool HasNoLife() => currentLife <= 0;
        

        public void AddCoins(int amount)
        {
            currentCoins += amount;
        }
    
        public void TakeLife(int newLife)
        {
            if (currentLife >= maxLife)
            {
                return;
            }
        
            currentLife += newLife;
        }
    
        public void ExtendsMaxLife()
        {
            maxLife++;
        }

        public void SetFullLife()
        {
            currentLife = maxLife;
        }

        #region Getters

        public float GetRunningSpeed()
        {
            return runningSpeed;
        }

        public float GetSpeed()
        {
            return speed;
        }

        #endregion
       
        
    }
}
