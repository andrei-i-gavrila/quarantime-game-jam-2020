using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Map
{
    [Serializable]
    public class Biome
    {
        public float height;
        [FormerlySerializedAs("endDepth")] public float maxTerrainHeight;
        public Color startColor;
        public Color endColor;
        public int numSteps;
        public Material material;
    }
}