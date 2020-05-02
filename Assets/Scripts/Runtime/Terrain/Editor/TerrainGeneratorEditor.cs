using Runtime.Map;
using UnityEditor;
using UnityEngine;

namespace Runtime.Terrain.Editor {
    [CustomEditor (typeof (TerrainGenerator))]
    public class TerrainGeneratorEditor : UnityEditor.Editor {
        TerrainGenerator terrainGen;

        public override void OnInspectorGUI () {
            // DrawDefaultInspector ();
            //
            // if (GUILayout.Button ("Refresh")) {
            //     terrainGen.Generate ();
            // }
        }

        void OnEnable () {
            // terrainGen = (TerrainGenerator) target;
        }
    }
}