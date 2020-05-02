using System;
using Resources;
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
        public int resourceTileSize = 1;
        public float resourceRarity = 0.05f;
        public GameObject[] resources;
    }
}