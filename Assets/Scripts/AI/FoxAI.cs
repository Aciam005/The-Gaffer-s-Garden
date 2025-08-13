using Game.AI.Genetics;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Animals
{
    [RequireComponent(typeof(NavMeshAgent), typeof(GeneticTraits))]
    public class FoxAI : AnimalAI
    {
        [Header("FOX-SPECIFIC")]
        [SerializeField] private LayerMask foodLayer; // Bunnies are on this layer

        private void Start()
        {
            Init();
        }

        protected override void LookForFood()
        {
            _targetFood = FindClosestTarget<BunnyAI>(foodLayer);

            if (_targetFood != null)
            {
                currentState = AnimalState.Feeding;
                Agent.SetDestination(_targetFood.transform.position);
            }
            else
            {
                Wander(true);
            }
        }

        protected override void Feed()
        {
            if (_targetFood != null && Vector3.Distance(transform.position, _targetFood.transform.position) < EatDistance)
            {
                (_targetFood as AnimalAI).Die(); 
                satiation += SatiationFromFood;
                _targetFood = null;
            }
        }
    }
}
