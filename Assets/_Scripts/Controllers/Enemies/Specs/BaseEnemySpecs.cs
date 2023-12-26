using System;
using UnityEngine;

namespace _Scripts.Enemies.Specs
{
    [System.Serializable]
    public class BaseEnemySpecs
    {
        //TODO: change that by a unique ID for each Item when SaveData Manager is created.
        public int id;
        
        public string name = "Default Enemy Name";

        public int life = 5;
        
        public int damage = 1;
        public float forceImpulse = 500;
    }
}