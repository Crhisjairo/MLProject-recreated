using System;
using _Scripts.Enums;
using UnityEngine.Serialization;
namespace _Scripts.GameManagerSystem.Models
{
    [System.Serializable]
    public class CharacterSaveData
    {
        public CharacterNames characterName;
        
        public int maxLife;
        public int currentLife;
        public float speed;
        public float runningSpeed;
        public int attackDamage;
        public float forceImpulse;
    }
}
