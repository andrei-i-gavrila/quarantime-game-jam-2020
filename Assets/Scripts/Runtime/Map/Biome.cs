using System;
using UnityEngine;

namespace Runtime.Map
{
    [Serializable]
    public class Biome
    {
        public float height;
        public float startDepth;
        public float endDepth;
        public Color startColor;
        public Color endColor;
        public int numSteps;
        public Material material;
    }
}