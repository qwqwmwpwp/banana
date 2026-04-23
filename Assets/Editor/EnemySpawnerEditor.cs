using UnityEditor;
using UnityEngine;
using EnemyCommon;

[CustomEditor(typeof(EnemySpawner_Old))]
public class EnemySpawnerEditor : Editor
{
    // 刷新基礎設定
    SerializedProperty spawnModeProp;
    SerializedProperty spawnPointsProp;
    SerializedProperty patrolPointsProp;

    // 波次刷新設定
    SerializedProperty wavesProp;
    SerializedProperty delayBetweenWavesProp;
    SerializedProperty maxAliveEnemiesProp;

    // 循環刷新設定
    SerializedProperty loopEnemyProp;
    SerializedProperty loopSpawnCountProp;
    SerializedProperty loopSpawnIntervalProp;

    // 主從循環刷新設定
    SerializedProperty esmProp;

    private void OnEnable()
    {
        // 快取所有屬性
        spawnModeProp = serializedObject.FindProperty("spawnMode");
        spawnPointsProp = serializedObject.FindProperty("spawnPoints");
        patrolPointsProp = serializedObject.FindProperty("patrolPoints");
        wavesProp = serializedObject.FindProperty("waves");
        delayBetweenWavesProp = serializedObject.FindProperty("delayBetweenWaves");
        maxAliveEnemiesProp = serializedObject.FindProperty("maxAliveEnemies");
        loopEnemyProp = serializedObject.FindProperty("loopEnemyId");
        loopSpawnCountProp = serializedObject.FindProperty("loopSpawnCount");
        loopSpawnIntervalProp = serializedObject.FindProperty("loopSpawnInterval");
        esmProp = serializedObject.FindProperty("esm");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(spawnModeProp, new GUIContent("刷新模式"));

        SpawnModeOld selectedMode = (SpawnModeOld)spawnModeProp.enumValueIndex;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("刷新設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spawnPointsProp, new GUIContent("刷新點"));
        EditorGUILayout.PropertyField(patrolPointsProp, new GUIContent("巡邏點"));
        EditorGUILayout.PropertyField(maxAliveEnemiesProp, new GUIContent("敵人生成數量上限"));

        EditorGUILayout.Space(5);

        switch (selectedMode)
        {
            case SpawnModeOld.Wave:
                EditorGUILayout.LabelField("波次設定", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(wavesProp, new GUIContent("波次設定"));
                EditorGUILayout.PropertyField(delayBetweenWavesProp, new GUIContent("波次間隔"));
                
                break;

            case SpawnModeOld.Loop:
                EditorGUILayout.LabelField("循環刷新設定", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(loopEnemyProp, new GUIContent("刷新目標"));
                EditorGUILayout.PropertyField(loopSpawnCountProp, new GUIContent("單次刷新數量"));
                EditorGUILayout.PropertyField(loopSpawnIntervalProp, new GUIContent("刷新間隔"));
                break;
            
            case SpawnModeOld.SlaveLoop:
                EditorGUILayout.LabelField("主從循環刷新設定", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(esmProp, new GUIContent("刷新控制器"));
                EditorGUILayout.PropertyField(loopEnemyProp, new GUIContent("刷新目標"));
                EditorGUILayout.PropertyField(loopSpawnIntervalProp, new GUIContent("刷新間隔"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
