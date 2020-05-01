using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Buildings
{

    [ExecuteInEditMode]
    public class FarmLand : MonoBehaviour, IResource
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

        public GameObject[] wheat;

        // Start is called before the first frame update
        void Start()
        {
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

        private void SetActive()
        {
            var lastActive = (int) (wheat.Length * (1 - Depletion));
            if (lastActive == 0 && Amount != 0)
            {
                lastActive = 1;
            }

            for (var i = 0; i < wheat.Length; i++)
            {
                wheat[i].gameObject.SetActive(i < lastActive);
            }
        }
        
    }
    
}
