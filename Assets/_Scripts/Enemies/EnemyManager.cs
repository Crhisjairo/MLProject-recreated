using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Enemy[] enemies;

        public void PauseAllEnemies()
        {
            foreach (var enemy in enemies)
            {
                if(enemy)
                    enemy.OnPauseAction();           
            }
        }

        public void ResumeAllEnemies()
        {
            foreach (var enemy in enemies)
            {
                if(enemy)
                    enemy.OnResumeAction();
            }
        }
    }
}
