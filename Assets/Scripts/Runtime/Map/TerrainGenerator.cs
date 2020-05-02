using System.Collections.Generic;
using System.Linq;
using Runtime.Terrain;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Map
{
    [ExecuteInEditMode]
    public class TerrainGenerator : MonoBehaviour
    {
        private const string meshHolderName = "Terrain Mesh";

        public bool autoUpdate = true;

        public bool centralize = true;
        public int worldSize = 20;
        public float tileSize = 10f;

        public NoiseSettings terrainNoise;

        public Biome[] biomes;
        public List<GameObject> generatedResources;

        private MeshFilter[] _meshFilters;
        private MeshRenderer[] _meshRenderers;
        private MeshCollider[] _meshColliders;
        private Mesh[] _meshes;

        private bool _needsUpdate;

        private void Start()
        {
            Generate();
        }

        private void Update()
        {
            if (!_needsUpdate || !autoUpdate) return;
            _needsUpdate = false;
            Generate();
        }

        public TerrainData Generate()
        {
            float halfTile = (float)(tileSize / 2.0);
            CreateMeshComponents();

            var numTilesPerLine = Mathf.CeilToInt(worldSize);
            var min = centralize ? -numTilesPerLine / 2f : 0;
            var map = HeightmapGenerator.GenerateHeightmap(terrainNoise, numTilesPerLine);
            var prng = new System.Random (terrainNoise.seed);

            var vertices = biomes.Select(_ => new List<Vector3>()).ToArray();
            var triangles = biomes.Select(_ => new List<int>()).ToArray();

            var terrainData = new TerrainData(numTilesPerLine);
            var colors = biomes.Select(_ => new List<Color>()).ToArray();

            for (var y = 0; y < numTilesPerLine; y++)
            {
                for (var x = 0; x < numTilesPerLine; x++)
                {
                    var biomeAndStep = GetBiomeInfo(map[x, y]);
                    var biomeIndex = (int) biomeAndStep.x;
                    var biome = biomes[biomeIndex];
                    terrainData.BiomeIndices[x, y] = biomeIndex;
                    terrainData.BiomesStep[x, y] = biomeAndStep.y;
                    terrainData.Depths[x, y] = biomeIndex > 0 ? Mathf.Lerp(biomes[biomeIndex - 1].maxTerrainHeight, biome.maxTerrainHeight, biomeAndStep.y) : 0f;
                }
            }

            foreach (GameObject generatedResource in generatedResources)
            {
                DestroyImmediate(generatedResource);
            }
            generatedResources = new List<GameObject>();
            
            for (var y = 0; y < numTilesPerLine; y++)
            {
                for (var x = 0; x < numTilesPerLine; x++)
                {
                    // break;
                    var biomeAndStep = GetBiomeInfo(map[x, y]);
                    var biomeIndex = (int) biomeAndStep.x;
                    var biome = biomes[biomeIndex];
                    var height = terrainData.Depths[x, y] * tileSize;
                    if (biome.resources.Length == 0) continue;
                    
                    if (x % biome.resourceTileSize != 0 || y % biome.resourceTileSize != 0)
                    {
                        continue;
                    }

                    
                    var r = prng.Next(biome.resources.Length);
                    var shouldGenerate = prng.NextDouble() > (1 - biome.resourceRarity);
                    if (shouldGenerate)
                    {
                        GameObject gm = Instantiate<GameObject>(
                            biome.resources[r], 
                            new Vector3(min + x*tileSize, height, min + y*tileSize),
                            Quaternion.identity
                            );
                        gm.transform.RotateAround(gm.transform.position, gm.transform.up, Random.Range(0, 360));
                        generatedResources.Add(gm);
                    }


                }
            }

            // Bridge gaps between water and land tiles, and also fill in sides of map
            for (var y = 0; y < numTilesPerLine; y++)
            {
                for (var x = 0; x < numTilesPerLine; x++)
                {
                    var biomeIndex = terrainData.BiomeIndices[x, y];
                    var biome = biomes[biomeIndex];
                    var step = terrainData.BiomesStep[x, y];
                    var color = Color.Lerp(biome.startColor, biome.endColor, step);
                    var height = terrainData.Depths[x, y];

                    var vertexCount = vertices[biomeIndex].Count;

                    var topRight = new Vector3(min + x*tileSize - halfTile, GetCornerHeight(terrainData, x, y, -1, -1), min + y*tileSize - halfTile);
                    var topLeft = new Vector3(min + x*tileSize + halfTile, GetCornerHeight(terrainData, x, y, 1, -1), min + y*tileSize - halfTile);
                    var bottomRight = new Vector3(min + x*tileSize - halfTile, GetCornerHeight(terrainData, x, y, -1, 1), min + y*tileSize + halfTile);
                    var bottomLeft = new Vector3(min + x*tileSize + halfTile, GetCornerHeight(terrainData, x, y, 1, 1), min + y*tileSize + halfTile);
                    
                    vertices[biomeIndex].AddRange(new[]
                    {
                        topLeft,
                        topRight,
                        bottomRight,
                        bottomLeft
                    });
                    colors[biomeIndex].AddRange(new[]
                    {
                        color,
                        color,
                        color,
                        color
                    });
                    triangles[biomeIndex].AddRange(new[]
                    {
                        vertexCount + 0, vertexCount + 1, vertexCount + 2,
                        vertexCount + 0, vertexCount + 2, vertexCount + 3,
                    });
                }
            }


            for (var biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
            {
                var mesh = _meshes[biomeIndex];
                mesh.SetVertices(vertices[biomeIndex]);
                mesh.SetTriangles(triangles[biomeIndex], 0, true);
                mesh.SetColors(colors[biomeIndex]);
                mesh.Optimize();
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
                mesh.RecalculateBounds();

                _meshRenderers[biomeIndex].sharedMaterial = biomes[biomeIndex].material;
            }

            return terrainData;
        }

        private float GetCornerHeight(TerrainData terrainData, int x, int y, int dx, int dy)
        {
            var sumHeight = terrainData.Depths[x, y];
            var countHeights = 1f;

            var dxExists = x + dx >= 0 && x + dx < terrainData.Size;
            var dyExists = y + dy >= 0 && y + dy < terrainData.Size;
            if (dxExists)
            {
                sumHeight += terrainData.Depths[x + dx, y];
                countHeights++;
                if (dyExists)
                {
                    sumHeight += terrainData.Depths[x + dx, y + dy];
                    countHeights++;
                }
            }

            if (dyExists)
            {
                sumHeight += terrainData.Depths[x, y + dy];
                countHeights++;
            }

            sumHeight *= tileSize;

            return sumHeight / countHeights;
        }

        private Vector2 GetBiomeInfo(float height)
        {
            // Find current biome
            var biomeIndex = 0;
            float biomeStartHeight = 0;
            for (var i = 0; i < biomes.Length; i++)
            {
                if (height <= biomes[i].height)
                {
                    biomeIndex = i;
                    break;
                }

                biomeStartHeight = biomes[i].height;
            }

            var biome = biomes[biomeIndex];
            var sampleT = Mathf.InverseLerp(biomeStartHeight, biome.height, height);
            sampleT = (int) (sampleT * biome.numSteps) / (float) Mathf.Max(biome.numSteps, 1);

            // UV stores x: biomeIndex and y: val between 0 and 1 for how close to prev/next biome
            return new Vector2(biomeIndex, sampleT);
        }

        private void CreateMeshComponents()
        {
            _meshFilters = new MeshFilter[biomes.Length];
            _meshRenderers = new MeshRenderer[biomes.Length];
            _meshColliders = new MeshCollider[biomes.Length];
            _meshes = new Mesh[biomes.Length];

            for (var biomeIndex = 0; biomeIndex < biomes.Length; biomeIndex++)
            {
                if (_meshFilters[biomeIndex] == null)
                {
                    var terrainMeshName = meshHolderName + "_" + biomeIndex;
                    var holder = GameObject.Find(terrainMeshName);
                    if (holder)
                    {
                        _meshFilters[biomeIndex] = holder.GetComponent<MeshFilter>();
                        _meshRenderers[biomeIndex] = holder.GetComponent<MeshRenderer>();
                        _meshColliders[biomeIndex] = holder.GetComponent<MeshCollider>();
                    }
                    else
                    {
                        holder = new GameObject(terrainMeshName);
                        _meshRenderers[biomeIndex] = holder.AddComponent<MeshRenderer>();
                        _meshFilters[biomeIndex] = holder.AddComponent<MeshFilter>();
                        _meshColliders[biomeIndex] = holder.AddComponent<MeshCollider>();
                        holder.layer = 9;
                    }
                }

                if (_meshFilters[biomeIndex].sharedMesh == null || _meshColliders[biomeIndex].sharedMesh == null)
                {
                    _meshes[biomeIndex] = new Mesh {indexFormat = IndexFormat.UInt32};
                    _meshFilters[biomeIndex].sharedMesh = _meshes[biomeIndex];
                    _meshColliders[biomeIndex].sharedMesh = _meshes[biomeIndex];
                }
                else
                {
                    _meshes[biomeIndex] = _meshFilters[biomeIndex].sharedMesh;
                    _meshes[biomeIndex].Clear();
                }

                _meshRenderers[biomeIndex].shadowCastingMode = ShadowCastingMode.Off;
            }
        }

        private void OnValidate()
        {
            _needsUpdate = true;
        }


        public class TerrainData
        {
            public int Size;
            public Vector3[,] TileCentres;
            public int[,] BiomeIndices;
            public float[,] BiomesStep;
            public float[,] Depths;

            public TerrainData(int size)
            {
                this.Size = size;
                TileCentres = new Vector3[size, size];
                BiomeIndices = new int[size, size];
                BiomesStep = new float[size, size];
                Depths = new float[size, size];
            }
        }
    }
}