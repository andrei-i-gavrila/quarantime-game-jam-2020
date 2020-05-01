using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Buildings
{

    [ExecuteInEditMode]
    public class StorageUnit : MonoBehaviour
    {

        public int StorageCapacity;

        [SerializeField] private int _stored;
        public int Stored
        {
            get => _stored;
            set {
                _stored = value; SetState();
            }
        }
        
        public GameObject[] barrels;
        
        // Start is called before the first frame update
        void Start()
        {
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
