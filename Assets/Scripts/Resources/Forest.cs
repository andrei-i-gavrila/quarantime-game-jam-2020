using UnityEngine;

namespace Resources
{
    [ExecuteInEditMode]
    public class Forest : MonoBehaviour, IResource
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

        public TreeWithStump[] trees;

        void Start()
        {
            for (var i = 0; i < trees.Length; i++) {
                var rnd = Random.Range(0, trees.Length);
                var temp = trees[rnd];
                trees[rnd] = trees[i];
                trees[i] = temp;

                trees[i].transform.RotateAround(trees[i].transform.position, trees[i].transform.up, Random.Range(0, 360));
            }
        }

        void Update()
        {
            SetActive();
        }

        private void SetActive()
        {
            var lastActive = (int) (trees.Length * (1 - Depletion));
            if (lastActive == 0 && Amount > 0)
            {
                lastActive = 1;
            }

            for (var i = 0; i < trees.Length; i++)
            {
                trees[i].SetGrown(i < lastActive);
            }
        }
        
    }
}