using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain
{
    [ExecuteInEditMode]
    public class TerrainGenerator : MonoBehaviour
    {
        const string meshHolderName = "Terrain Mesh";

        public bool autoUpdate = true;

        public bool centralize = true;
        public int worldSize = 20;

        public NoiseSettings terrainNoise;
        public Material mat;

        public Biome[] biomes;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        Mesh mesh;

        bool needsUpdate;

        void Update()
        {
            if (needsUpdate && autoUpdate)
            {
                needsUpdate = false;
                Generate();
            }
        }

        public TerrainData Generate()
        {
            CreateMeshComponents();

            var numTilesPerLine = Mathf.CeilToInt(worldSize);
            var min = centralize ? -numTilesPerLine / 2f : 0;
            var map = HeightmapGenerator.GenerateHeightmap(terrainNoise, numTilesPerLine);

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();

            // Some convenience stuff:
            var upVectors = new[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
            var cardinalsDx = new[] {1, 0, -1, 0};
            var cardinalsDy = new[] {0, 1, 0, -1};
            var sideVertexIndexByDir = new[] {new[] {0, 1}, new[] {3, 2}, new[] {2, 0}, new[] {1, 3}};
            var sideNormalsByDir = new[]
            {
                new[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward}, 
                new[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back},
                new[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left}, 
                new[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right}
            };

            // Terrain data:
            var terrainData = new TerrainData(numTilesPerLine);


            var colors = new List<Color>();

            for (var y = 0; y < numTilesPerLine; y++)
            {
                for (var x = 0; x < numTilesPerLine; x++)
                {
                    var biomeAndStep = GetBiomeInfo(map[x, y]);
                    var biomeIndex = (int) biomeAndStep.x;
                    terrainData.biomeIndices[x, y] = biomeIndex;
                    terrainData.biomesStep[x, y] = biomeAndStep.y;
                }
            }

            // Bridge gaps between water and land tiles, and also fill in sides of map
            for (var y = 0; y < numTilesPerLine; y++)
            {
                for (var x = 0; x < numTilesPerLine; x++)
                {
                    var biome = biomes[terrainData.biomeIndices[x, y]];
                    var step = terrainData.biomesStep[x, y];
                    var color = Color.Lerp(biome.startColor, biome.endColor, step);

                    var verticesColor = new[] {color, color, color, color};
                    colors.AddRange(verticesColor);

                    // Vertices
                    var vertexIndex = vertices.Count;
                    var height = biome.depth;
                    var northWest = new Vector3(min + x, height, min + y + 1);
                    var northEast = northWest + Vector3.right;
                    var southWest = northWest - Vector3.forward;
                    var southEast = southWest + Vector3.right;
                    var cornerVertices = new[] {northWest, northEast, southWest, southEast};
                    vertices.AddRange(cornerVertices);
                    normals.AddRange(upVectors);
                    triangles.AddRange(new[] {vertexIndex, vertexIndex + 1, vertexIndex + 2});
                    triangles.AddRange(new[] {vertexIndex + 1, vertexIndex + 3, vertexIndex + 2});

                    if (terrainData.biomeIndices[x, y] == 0) continue;

                    for (var i = 0; i < 4; i++)
                    {
                        var neighbourX = x + cardinalsDx[i];
                        var neighbourY = y + cardinalsDy[i];

                        var neighbourIsOutOfBounds = neighbourX < 0 || neighbourX >= numTilesPerLine || neighbourY < 0 || neighbourY >= numTilesPerLine;

                        var depthOfNeighbour = neighbourIsOutOfBounds ? -1 : biomes[terrainData.biomeIndices[neighbourX, neighbourY]].depth - biome.depth;

                        if (depthOfNeighbour > 0) continue;

                        vertexIndex = vertices.Count;
                        var edgeVertexIndexA = sideVertexIndexByDir[i][0];
                        var edgeVertexIndexB = sideVertexIndexByDir[i][1];
                        vertices.Add(cornerVertices[edgeVertexIndexA]);
                        vertices.Add(cornerVertices[edgeVertexIndexA] + Vector3.up * depthOfNeighbour);
                        vertices.Add(cornerVertices[edgeVertexIndexB]);
                        vertices.Add(cornerVertices[edgeVertexIndexB] + Vector3.up * depthOfNeighbour);

                        colors.AddRange(colors);
                        triangles.AddRange(new[] {vertexIndex, vertexIndex + 1, vertexIndex + 2, vertexIndex + 1, vertexIndex + 3, vertexIndex + 2});
                        normals.AddRange(sideNormalsByDir[i]);
                    }

                    // Terrain data:
                }
            }

            // Update mesh:
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, true);
            mesh.SetColors(colors);
            mesh.SetNormals(normals);

            meshRenderer.sharedMaterial = mat;

            return terrainData;
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

        void CreateMeshComponents()
        {
            GameObject holder = null;

            if (meshFilter == null)
            {
                if (GameObject.Find(meshHolderName))
                {
                    holder = GameObject.Find(meshHolderName);
                }
                else
                {
                    holder = new GameObject(meshHolderName);
                    holder.AddComponent<MeshRenderer>();
                    holder.AddComponent<MeshFilter>();
                }

                meshFilter = holder.GetComponent<MeshFilter>();
                meshRenderer = holder.GetComponent<MeshRenderer>();
            }

            if (meshFilter.sharedMesh == null)
            {
                mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh = meshFilter.sharedMesh;
                mesh.Clear();
            }

            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        void OnValidate()
        {
            needsUpdate = true;
        }

        [Serializable]
        public class Biome
        {
            public float height;
            public float depth;
            public Color startColor;
            public Color endColor;
            public int numSteps;
        }

        public class TerrainData
        {
            public int size;
            public Vector3[,] tileCentres;
            public int[,] biomeIndices;
            public float[,] biomesStep;

            public TerrainData(int size)
            {
                this.size = size;
                tileCentres = new Vector3[size, size];
                biomeIndices = new int[size, size];
                biomesStep = new float[size, size];
            }
        }
    }
}