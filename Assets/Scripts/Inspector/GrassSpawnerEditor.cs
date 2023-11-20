using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEditorInternal;

namespace Assets.Scripts.Inspector
{
    [CustomEditor(typeof(GrassSpawner))]
    public class GrassSpawnerEditor :Editor
    {

        public override void OnInspectorGUI()
        {

            GrassSpawner myTarget = (GrassSpawner)target;


            myTarget.xSize = EditorGUILayout.FloatField("x size", myTarget.xSize);
            myTarget.zSize = EditorGUILayout.FloatField("z size", myTarget.zSize);
            myTarget.interval = EditorGUILayout.FloatField("interval", myTarget.interval);
            myTarget.perlinIntensity = EditorGUILayout.FloatField("perlinIntensity", myTarget.perlinIntensity);
            myTarget.snowLine = EditorGUILayout.FloatField("snow line", myTarget.snowLine);

            myTarget.SpawnPrefab = (GameObject)EditorGUILayout.ObjectField("Spawn Prefab", myTarget.SpawnPrefab, typeof(GameObject), true);
            myTarget.SpawnParent = EditorGUILayout.ObjectField("Spawn Parent", myTarget.SpawnParent, typeof(Transform), true) as Transform;


            //InternalEditorUtility.LayerMaskToConcatenatedLayersMask(myTarget.spawnLayermask), InternalEditorUtility.layers);

            myTarget.spawnLayermask = LayerMask.GetMask("Terrain");
            EditorGUILayout.MaskField("SpawnLayer", myTarget.spawnLayermask, InternalEditorUtility.layers);

            if (GUILayout.Button("Generate Foliage on \"Terrain\" layer"))
            {
                myTarget.GridSpawn();   
            }
        }
    }
}
