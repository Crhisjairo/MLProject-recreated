using UnityEngine;

namespace _Scripts.Enemies.Specs
{
    [CreateAssetMenu(fileName = "BaseEnemy", menuName = "EnemySpecs/BaseEnemy", order = 0)]
    public class EnemySpecs : ScriptableObject
    {
        public int id;
        
        public string name;
        
        public int damage;
        public float forceImpulse;
    }
}