using System;
using Resources;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Buildings
{

    [ExecuteInEditMode]
    public class FarmLand : MonoBehaviour, IResourceSite, IProducent
    {
        
        [SerializeField] private int _maxAmount;
        [SerializeField] private int _amount;

        public int MaxAmount
        {
            get => _maxAmount;
            set => _maxAmount = value;
        }

        public int Amount
        {
            get => _amount;
            set { _amount = value; SetActive();}
        }

        public float Depletion => 1 - ((float) Amount / MaxAmount);
        public string ResourceName { get => "Wheat"; }
        public int ResourceId { get => 2; }
        public int ProductionRate { get => 2; }

        public GameObject[] wheat;
        private Inventory _inventory;

        // Start is called before the first frame update
        void Start()
        {
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _inventory.RegisterProducent(ResourceId, this);
            for (var i = 0; i < wheat.Length; i++) {
                var rnd = Random.Range(0, wheat.Length);
                var temp = wheat[rnd];
                wheat[rnd] = wheat[i];
                wheat[i] = temp;

                wheat[i].transform.RotateAround(wheat[i].transform.position, wheat[i].transform.up, Random.Range(0, 360));
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetActive(); // Reactivity in Editor
        }

        private void OnDestroy()
        {
            _inventory.UnregisterProducent(ResourceId, this);
        }

        private void SetActive()
        {
            var lastActive = (int) (wheat.Length * (1 - Depletion));
            if (lastActive == 0 && Amount > 0)
            {
                lastActive = 1;
            }

            for (var i = 0; i < wheat.Length; i++)
            {
                wheat[i].gameObject.SetActive(i < lastActive);
            }
        }

        public int Produce(float rate)
        {
            int produced = Math.Min((int) (rate * ProductionRate), Amount);
            Amount = _amount - produced;
            return produced;
        }
    }
}
