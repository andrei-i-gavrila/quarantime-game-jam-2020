using UnityEngine;

namespace Resources
{
    public class TreeWithStump : MonoBehaviour
    {
        public GameObject tree;
        public GameObject stump;
        private bool _grown;

        public void SetGrown(bool grown)
        {
            if (grown == _grown) return;
            _grown = grown;
            tree.gameObject.SetActive(grown);
            stump.gameObject.SetActive(!grown);
        }
    }
}