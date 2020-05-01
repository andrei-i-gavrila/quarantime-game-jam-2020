﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Terrain {
    [CustomEditor (typeof (TerrainGenerator))]
    public class TerrainGeneratorEditor : Editor {
        TerrainGenerator terrainGen;

        public override void OnInspectorGUI () {
            DrawDefaultInspector ();

            if (GUILayout.Button ("Refresh")) {
                terrainGen.Generate ();
            }
        }

        void OnEnable () {
            terrainGen = (TerrainGenerator) target;
        }
    }
}