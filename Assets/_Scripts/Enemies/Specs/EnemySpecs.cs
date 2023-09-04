using UnityEngine;

namespace _Scripts.Enemies.Specs
{
    [CreateAssetMenu(fileName = "DefaultEnemySpecs", menuName = "EnemySpecs/DefaultEnemySpecs", order = 0)]
    public class EnemySpecs : ScriptableObject
    {
        public int id;
        
        public new string name = "Default Enemy Specs";

        public int life = 5;
        
        public int damage;
        public float forceImpulse;

        public EnemySpecs GetCopy()
        {
            return (EnemySpecs) this.MemberwiseClone();
        }
    }
}