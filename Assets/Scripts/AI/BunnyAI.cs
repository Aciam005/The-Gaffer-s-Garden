using Game.AI.Genetics;
using Game.Plants;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Animals
{
    [RequireComponent(typeof(NavMeshAgent), typeof(GeneticTraits))]
    public class BunnyAI : AnimalAI
    {
        [Header("BUNNY-SPECIFIC")]
        [SerializeField] private LayerMask foodLayer;
        [SerializeField] private LayerMask predatorLayer;
        [SerializeField] private float safeDistance = 15f;

        private AnimalAI _predator;

        private void Start()
        {
            Init();
        }

        protected override void UpdateStateLogic()
        {
            base.UpdateStateLogic();

            if (currentState == AnimalState.Cowering)
            {
                Flee(_predator.transform);
            }
        }

        protected override void SetState()
        {
            _predator = FindClosestTarget<AnimalAI>(predatorLayer);

            if (_predator != null)
            {
                currentState = AnimalState.Cowering;
                return;
            }

            if (currentState == AnimalState.Cowering && _predator == null)
            {
                currentState = AnimalState.Wandering;
            }

            base.SetState();
        }

        protected override void LookForFood()
        {
            _targetFood = FindClosestTarget<Carrot>(foodLayer, carrot => carrot.CurrentState == PlantStates.ReadyForHarvest);

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
                (_targetFood as Carrot).Harvest();
                satiation += SatiationFromFood;
                _targetFood = null;
            }
        }
    }
}
