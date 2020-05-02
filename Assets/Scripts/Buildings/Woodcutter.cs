using System;
using System.Linq;
using Resources;
using UnityEngine;

namespace Buildings
{
    [ExecuteInEditMode]
    public class Woodcutter : MonoBehaviour, IProducent, IBuilding
    {
        private Forest _forest;
        private Inventory _inventory;
        private bool _placed;

        public void Place()
        {
            _placed = true;
        }

        public int ProductionRate { get => 50; }

        private void Start()
        {
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _inventory.RegisterProducent(0, this);
        }

        public int Produce(float rate)
        {
            if (!_placed) return 0;
            if (_forest == null)
            {
                _forest = FindForest();
                if (_forest == null) return 0;
            }

            var produced = Math.Min(_forest.Amount, (int) (ProductionRate * rate));
            _forest.Amount -= produced;
            if (_forest.Amount <= 0)
            {
                _forest = null;
            }

            return produced;
        }

        private Forest FindForest()
        {
            // TODO: If no forest available - IDLE
            var forests = GameObject.FindGameObjectsWithTag("WoodResource")
                .Select(g => g.GetComponent<Forest>())
                .Where(g => g.Amount > 0);
            Forest closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (Forest forest in forests)
            {
                Vector3 diff = forest.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = forest;
                    distance = curDistance;
                }
            }
            return closest;
        }
    }
}