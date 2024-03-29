using System;
using _Scripts.Shared.Enums;

namespace _Scripts.GameManagerSystem.Models
{
    [Serializable]
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
