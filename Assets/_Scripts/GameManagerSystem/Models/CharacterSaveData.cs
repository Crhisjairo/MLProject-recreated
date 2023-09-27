using System;
using UnityEngine.Serialization;
namespace _Scripts.GameManagerSystem.Models
{
    [System.Serializable]
    public class CharacterSaveData
    {
        public int maxLife;
        public int currentLife;
        public float speed;
        public float runningSpeed;
        public int attackDamage;
        public float forceImpulse;
    }
}
