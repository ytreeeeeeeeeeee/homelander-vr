using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.NPC
{
    public class NpcManager : MonoBehaviour
    {
        [SerializeField] private Npc npcPrefab;
        [SerializeField] private List<GameObject> availableModels = new ();
        [SerializeField] private int npcCount = 50;
        [SerializeField] private float spawnRadius = 40f;

        public static NpcManager Instance;

        public event Action<Npc> OnNpcKilled;
        public event Action OnAllNpcKilled;
        public int KilledNpcCount => NpcCount - _npcList.Count;
        public int NpcCount { get; private set; } = 0;

        private readonly Dictionary<Collider, Npc> _npcList = new ();

        private void Awake()
        {
            Instance = this;
            
            for (int i = 0; i < npcCount; i++)
            {
                InstantiateNpc();
            }
        }

        private void Register(Collider col, Npc npc)
        {
            if (_npcList.TryAdd(col, npc))
            {
                NpcCount++;
            }
        }

        public void Remove(Collider col)
        {
            _npcList.Remove(col);
        }

        public void Kill(Collider col, Vector3 placeShot)
        {
            if (_npcList.TryGetValue(col, out Npc npc))
            {
                npc.Die(placeShot);
                _npcList.Remove(col);
            }
        
            OnNpcKilled?.Invoke(npc);

            if (0 == _npcList.Count)
            {
                OnAllNpcKilled?.Invoke();
            }
        }

        private void InstantiateNpc()
        {
            Npc newNpc = Instantiate(npcPrefab, GetRandomPositionOnNavMesh(),
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            GameObject model = Instantiate(availableModels[Random.Range(0, availableModels.Count)], newNpc.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
                
            newNpc.model = model;
                
            Register(newNpc.GetComponent<Collider>(), newNpc);
        }

        private Vector3 GetRandomPositionOnNavMesh()
        {
            Vector3 randomPosition = Vector3.zero + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0;
            
            return NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas) ?  hit.position : randomPosition;
        }
    }
}
