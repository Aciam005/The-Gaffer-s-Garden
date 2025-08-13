using UnityEngine;

namespace Game.AI.Genetics
{
    [System.Serializable]
    public class Genes
    {
        [Header("MOVEMENT")]
        [Tooltip("How fast the animal can move.")]
        [Range(1f, 10f)] public float movementSpeed;

        [Header("SENSES")]
        [Tooltip("The visual range of the animal for detecting food, mates, or threats.")]
        [Range(5f, 25f)] public float visionRange;
        
        [Header("DIET")]
        [Tooltip("The maximum satiation (fullness) of the animal.")]
        [Range(50f, 200f)] public float maxSatiation;
        [Tooltip("How close the animal needs to be to its food to eat it.")]
        [Range(1f, 5f)] public float eatDistance;
        [Tooltip("The amount of satiation gained from consuming one unit of food.")]
        [Range(5f, 50f)] public float satiationFromFood;
        
        // TODO: Add other inheritable traits here, like size, color, etc.
    }
}