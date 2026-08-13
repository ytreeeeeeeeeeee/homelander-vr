using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.NPC 
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Collider))]
    public class Npc : MonoBehaviour
    {
        [SerializeField] private float maxNextPointDistance = 20f;
        [SerializeField] private float maxNextPointCooldown = 5f;

        public GameObject model;
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private Collider _collider;
        private float _nextPointCooldown;
        private bool _isDead = false;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.destination = GetRandomPointOnNavMesh();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _animator = model.GetComponent<Animator>();
        }

        private void Update()
        {
            if (_isDead)
            {
                return;
            }
            
            if (0 <= _nextPointCooldown)
            {
                StayOnPlace();
            }
            else if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                StopMoving();
            }
            else
            {
                MoveToDestination();
            }
        }

        private void OnDestroy()
        {
            NpcManager.Instance.Remove(_collider);
        }

        public void Die(Vector3 placeShot)
        {
            if (_isDead)
            {
                return;
            }
            
            _isDead = true;

            _collider.enabled = false;
            _agent.isStopped = true;
            
            Vector3 toHit = (placeShot - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toHit);

            if (0f < dot)
            {
                _animator.SetTrigger("backward_fall");
            }
            else
            {
                _animator.SetTrigger("forward_fall");
            }
        }

        private Vector3 GetRandomPointOnNavMesh()
        {
            Vector3 randomPosition = Random.insideUnitSphere * maxNextPointDistance;
            randomPosition.y = 0;
            randomPosition += transform.position;
            
            return NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxNextPointDistance, NavMesh.AllAreas) ? hit.position : transform.position;
        }

        private void StopMoving()
        {
            _nextPointCooldown = Random.Range(0, maxNextPointCooldown);
        }

        private void MoveToDestination()
        {
            int speed = Mathf.Clamp((int) _agent.velocity.magnitude, 1, 2);
            
            _animator.SetInteger("move", speed);
        }

        private void StayOnPlace()
        {
            _animator.SetInteger("move", 0);
            _nextPointCooldown -= Time.deltaTime;

            if (0 >= _nextPointCooldown)
            {
                _agent.destination = GetRandomPointOnNavMesh();
            }
        }
    }
}
