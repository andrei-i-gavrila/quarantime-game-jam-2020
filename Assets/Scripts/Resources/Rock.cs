using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources
{
    [ExecuteInEditMode]
    public class Rock : MonoBehaviour, IResource
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
