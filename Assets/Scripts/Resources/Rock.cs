using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources
{
    [ExecuteInEditMode]
    public class Rock : MonoBehaviour, IResourceSite
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
            set { _amount = value; SetDepletedState();}
        }

        private void SetDepletedState()
        {
            var scale = 1 - Depletion;
            gameObject.SetActive(scale > 0.0);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public float Depletion => 1 - ((float) Amount / MaxAmount);
        public string ResourceName { get => "Stone"; }
        public int ResourceId { get => 1; }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
           SetDepletedState(); // Dev Editor
        }
    }
    
}
