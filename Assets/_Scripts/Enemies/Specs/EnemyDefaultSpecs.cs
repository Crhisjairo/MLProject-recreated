using UnityEngine;

namespace _Scripts.Enemies.Specs
{
    [CreateAssetMenu(fileName = "DefaultEnemySpecs", menuName = "EnemySpecs/DefaultEnemySpecs", order = 0)]
    public class EnemyDefaultSpecs : ScriptableObject
    {
        //TODO: change that by a unique ID for each Item when SaveData Manager is created.
        public int id;
        
        public new string name = "Default Enemy Specs";

        public int life = 5;
        
        public int damage;
        public float forceImpulse;

        public EnemyDefaultSpecs GetCopy()
        {
            return (EnemyDefaultSpecs) this.MemberwiseClone();
        }
    }
}