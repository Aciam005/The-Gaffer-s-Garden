using System;
using UnityEngine;

namespace Game.Plants
{
    public class Carrot : MonoBehaviour
    {
        public PlantStates CurrentState { get; private set; }

        private void Start()
        {
            CurrentState = PlantStates.ReadyForHarvest;
        }

        public void Harvest()
        {
            Destroy(this.gameObject);
        }
    }
}