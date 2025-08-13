using UnityEngine;

namespace Game.AI.Genetics
{
    public class GeneticTraits : MonoBehaviour
    {
        [Tooltip("The current active genes for this animal. For prefabs, these are the base stats for the species.")]
        [SerializeField] private Genes genes;
        public Genes CurrentGenes => genes;

        [Header("Mutation Settings")]
        [Tooltip("The probability (0-1) that a gene will mutate during inheritance.")]
        [Range(0f, 1f)]
        [SerializeField] private float mutationChance = 0.05f;

        [Tooltip("The maximum percentage (e.g., 0.1 for 10%) a gene can change by when it mutates.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float mutationAmount = 0.1f;
        
        public void InheritGenes(GeneticTraits parentA, GeneticTraits parentB)
        {
            genes = GeneticsUtility.Combine(parentA.CurrentGenes, parentB.CurrentGenes, mutationChance, mutationAmount);
        }
    }
}