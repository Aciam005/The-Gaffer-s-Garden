using UnityEngine;

namespace Game.Plants
{
    public abstract class Plant : MonoBehaviour
    {
        
    }

     public enum PlantStates
    {
        Barren,
        Growing,
        ReadyForHarvest,
    }
}