using System.Collections.Generic;
using Resources;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Buildings
{

    public class StorageUnit : MonoBehaviour, IResourceStorage
    {
        private Inventory _inventory;

        private int _storageCapacity = 500;
        private int _stored = 0;
        public int StorageCapacity
        {
            get => _storageCapacity;
        }

        public int Stored
        {
            get => _stored;
        }

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
            int space = _storageCapacity - Stored;
            if (amount > space) return false;
            amounts[resource] += amount;
            _stored += amount;
            return true;
        }

        public bool TakeResource(int resource, int amount)
        {
            int available = amounts[resource];
            if (available < amount) return false;
            amounts[resource] -= amount;
            _stored -= amount;
            return true;
        }

        private void SetState()
        {
            var available = (float) _stored / _storageCapacity;
            var totalBarels = barrels.Length;
            var visibleBarels = (int) (totalBarels * available);
            if (visibleBarels == 0 && _stored > 0)
            {
                visibleBarels = 1;
            }

            for (int i = 0; i < totalBarels; i++)
            {
                barrels[i].SetActive(i < visibleBarels);
            }
        }

        public int StoredResource(int resource)
        {
            return amounts[resource];
        }
    }
}
