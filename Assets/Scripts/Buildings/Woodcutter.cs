using System;
using Resources;
using UnityEngine;

namespace Buildings
{
    [ExecuteInEditMode]
    public class Woodcutter : MonoBehaviour, IProducent
    {
        private Forest _forest;
        private Inventory _inventory;
        
        public int ProductionRate { get => 20; }

        private void Start()
        {
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _inventory.RegisterProducent(1, this);
        }

        public int Produce(float rate)
        {
            if (_forest == null)
            {
                _forest = FindForest();
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
            // TODO: If no forest available
            return GameObject.Find("Forest").GetComponent<Forest>();
        }
    }
}