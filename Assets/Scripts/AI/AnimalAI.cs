using Game.AI.Genetics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.AI.Animals
{
    public enum AnimalState
    {
        Wandering,
        LookingForFood,
        Feeding,
        Cowering
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(GeneticTraits))]
    public abstract class AnimalAI : WorldAgent
    {
        [Header("GENERIC ANIMAL STATS")]
        [SerializeField] protected float satiationRegression;
        [SerializeField] protected float satiationWarningLevel;
        
        [Header("WANDERING")]
        [SerializeField] private float minWanderWaitTime = 1f;
        [SerializeField] private float maxWanderWaitTime = 3f;
        
        [Header("AVOIDANCE")]
        [SerializeField] private LayerMask avoidanceLayer;
        [SerializeField] private float avoidanceRadius = 2f;
        [SerializeField] private float avoidanceStrength = 1.5f;

        // --- COMPONENT & GENE REFERENCES ---
        protected NavMeshAgent Agent { get; private set; }
        protected GeneticTraits GeneticTraits { get; private set; }
        protected float VisionRange { get; private set; }
        protected float MaxSatiation { get; private set; }
        protected float EatDistance { get; private set; }
        protected float SatiationFromFood { get; private set; }


        // --- INTERNAL STATE ---
        [Header("DEBUG")]
        [SerializeField] protected float satiation;
        [SerializeField] protected AnimalState currentState;
        protected Component _targetFood;
        private float _wanderWaitTimer;
        private bool _isWaitingAtDestination;

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            GeneticTraits = GetComponent<GeneticTraits>();
        }

        protected void Init()
        {
            // Apply stats from our genes
            Agent.speed = GeneticTraits.CurrentGenes.movementSpeed;
            MaxSatiation = GeneticTraits.CurrentGenes.maxSatiation;
            VisionRange = GeneticTraits.CurrentGenes.visionRange;
            EatDistance = GeneticTraits.CurrentGenes.eatDistance;
            SatiationFromFood = GeneticTraits.CurrentGenes.satiationFromFood;
            
            // Initialize state
            satiation = this.MaxSatiation;
            _wanderWaitTimer = 0f;
            _isWaitingAtDestination = false;
            currentState = AnimalState.Wandering;
            _targetFood = null;
        }
        
        private void Update()
        {
            satiation -= satiationRegression * Time.deltaTime;
            
            if (Agent.hasPath)
            {
                ApplyAvoidanceSteering();
            }
            
            UpdateStateLogic();
        }
        
        protected virtual void UpdateStateLogic()
        {
            SetState();
             
            switch (currentState)
            {
                case AnimalState.Wandering:
                    Wander(false);
                    break;
                case AnimalState.LookingForFood:
                    LookForFood();
                    break;
                case AnimalState.Feeding:
                    Feed();
                    break;
            }
        }

        protected virtual void SetState()
        {
            // Don't change state if we're committed to eating.
            if (currentState == AnimalState.Feeding && _targetFood != null) return;
             
            if (satiation <= satiationWarningLevel)
            {
                currentState = AnimalState.LookingForFood;
            }
            else
            {
                currentState = AnimalState.Wandering;
            }
        }
        
        // --- ABSTRACT METHODS FOR DERIVED CLASSES ---
        protected abstract void LookForFood();
        protected abstract void Feed();
        

        // --- REUSABLE BEHAVIORS ---

        protected void Wander(bool isHungry)
        {
            if (_isWaitingAtDestination)
            {
                _wanderWaitTimer -= Time.deltaTime;
                if (_wanderWaitTimer <= 0)
                {
                    _isWaitingAtDestination = false;
                    FindNewWanderDestination();
                }
            }
            else if (!Agent.pathPending && Agent.hasPath && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (isHungry)
                {
                    // Don't wait, immediately find a new search location.
                    FindNewWanderDestination();
                }
                else
                {
                    // We're content, so wait around.
                    Agent.ResetPath();
                    _isWaitingAtDestination = true;
                    _wanderWaitTimer = Random.Range(minWanderWaitTime, maxWanderWaitTime);
                }
            }
            else if (!Agent.hasPath)
            {
                FindNewWanderDestination();
            }
        }
        
        private void FindNewWanderDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * VisionRange;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, VisionRange, NavMesh.AllAreas))
            {
                Agent.SetDestination(hit.position);
            }
        }
        
        private void ApplyAvoidanceSteering()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, avoidanceRadius, avoidanceLayer);
            if (nearby.Length <= 1) return;
            
            Vector3 avoidanceVector = Vector3.zero;
            foreach (var col in nearby)
            {
                if (col.gameObject != gameObject)
                {
                    Vector3 awayFromOther = transform.position - col.transform.position;
                    avoidanceVector += awayFromOther.normalized / awayFromOther.magnitude;
                }
            }
            
            avoidanceVector /= (nearby.Length - 1);
            Agent.velocity += avoidanceVector * (avoidanceStrength * Time.deltaTime);
        }

        protected T FindClosestTarget<T>(LayerMask layer, System.Func<T, bool> predicate = null) where T : Component
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, VisionRange, layer);
 
            T closestTarget = null;
            float minDistanceSqr = float.MaxValue;
 
            foreach (var col in collidersInRange)
            {
                if (col.gameObject == gameObject) continue;
                
                if (col.TryGetComponent(out T component))
                {
                    // If a predicate is provided, check if it's met.
                    // If no predicate, or if the predicate is true, proceed.
                    if (predicate == null || predicate(component))
                    {
                        float distanceSqr = (transform.position - col.transform.position).sqrMagnitude;
                        if (distanceSqr < minDistanceSqr)
                        {
                            minDistanceSqr = distanceSqr;
                            closestTarget = component;
                        }
                    }
                }
            }
             
            return closestTarget;
        }
        
        public void Die()
        {
            // TODO: Maybe spawn some particle effects or a ragdoll
            Destroy(gameObject);
        }

        protected void Flee(Transform threat)
        {
            if (threat == null) return;

            Vector3 runDirection = transform.position - threat.position;
            Vector3 targetPosition = transform.position + runDirection.normalized * 10f; // Flee further away

            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, VisionRange, NavMesh.AllAreas))
            {
                Agent.SetDestination(hit.position);
            }
        }
        
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, VisionRange);
        }
    }
}
