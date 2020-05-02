using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Buildings
{

    [ExecuteInEditMode]
    public class StorageUnit : MonoBehaviour, IResourceStorage
    {
        private Inventory _inventory;

        public int StorageCapacity;
        public int Stored;
        private int[] amounts = new int[3];
        private List<int> resourceTypes = new List<int> {0, 1, 2};

        public GameObject[] barrels;
        
        // Start is called before the first frame update
        void Start()
        {
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            resourceTypes.ForEach(resourceType => _inventory.RegisterStorage(resourceType, this));
            for (var i = 0; i < barrels.Length; i++) {
                barrels[i].transform.RotateAround(barrels[i].transform.position, barrels[i].transform.up, Random.Range(0, 360));
                barrels[i].transform.Translate(Random.Range(-0.04f,0.04f), 0, Random.Range(-0.04f,0.04f));
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetState(); // For development only
        }

        private void OnDestroy()
        {
            resourceTypes.ForEach(resourceType => _inventory.UnregisterStorage(resourceType, this));
        }

        public bool Accepts(int resource)
        {
            return resourceTypes.Contains(resource);
        }

        public bool AddResource(int resource, int amount)
        {
            int space = StorageCapacity - Stored;
            if (amount > space) return false;
            amounts[resource] += amount;
            Stored += amount;
            return true;
        }

        public bool TakeResource(int resource, int amount)
        {
            int available = amounts[resource];
            if (available < amount) return false;
            amounts[resource] -= amount;
            Stored -= amount;
            return true;
        }

        private void SetState()
        {
            var available = (float) Stored / StorageCapacity;
            var totalBarels = barrels.Length;
            var visibleBarels = (int) (totalBarels * available);
            if (visibleBarels == 0 && Stored > 0)
            {
                visibleBarels = 1;
            }

            for (int i = 0; i < totalBarels; i++)
            {
                barrels[i].SetActive(i < visibleBarels);
            }
        }
        
    }
}
