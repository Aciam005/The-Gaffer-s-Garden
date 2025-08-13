using UnityEngine;

namespace Game.AI.Genetics
{
    public static class GeneticsUtility
    {
        public static Genes Combine(Genes parentA, Genes parentB, float mutationChance, float mutationAmount)
        {
            var offspringGenes = new Genes
            {
                movementSpeed = InheritTrait(parentA.movementSpeed, parentB.movementSpeed, mutationChance, mutationAmount),
                visionRange   = InheritTrait(parentA.visionRange, parentB.visionRange, mutationChance, mutationAmount),
                maxSatiation  = InheritTrait(parentA.maxSatiation, parentB.maxSatiation, mutationChance, mutationAmount)
            };

            return offspringGenes;
        }

        private static float InheritTrait(float parentAValue, float parentBValue, float chance, float amount)
        {
            // 50/50 chance to inherit from either parent
            float inheritedValue = Random.value < 0.5f ? parentAValue : parentBValue;

            // Apply mutation
            if (Random.value < chance) inheritedValue *= 1f + Random.Range(-amount, amount);
            return inheritedValue;
        }
    }
}