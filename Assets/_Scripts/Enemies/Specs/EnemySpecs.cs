using UnityEngine;

namespace _Scripts.Enemies.Specs
{
    [CreateAssetMenu(fileName = "BaseEnemy", menuName = "EnemySpecs/BaseEnemy", order = 0)]
    public class EnemySpecs : ScriptableObject
    {
        public int id;
        
        public new string name;

        public int life = 5;
        
        public int damage;
        public float forceImpulse;

        public EnemySpecs GetCopy()
        {
            return (EnemySpecs) this.MemberwiseClone();
        }
    }
}