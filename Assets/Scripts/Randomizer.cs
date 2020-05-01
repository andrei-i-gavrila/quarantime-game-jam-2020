using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class Randomizer : MonoBehaviour
    {
        public float RandomTranslateAmount = 1f;
        private void Start()
        {
            float rx = Random.Range(-RandomTranslateAmount, RandomTranslateAmount);
            float rz = Random.Range(-RandomTranslateAmount, RandomTranslateAmount);
            Vector3 vector = new Vector3(rx, 0, rz);
            gameObject.transform.Translate(vector);
            gameObject.transform.RotateAround(
                gameObject.transform.localPosition,
                gameObject.transform.up,
                Random.Range(0, 360)
            );
        }
    }
}