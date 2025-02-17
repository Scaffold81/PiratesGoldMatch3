using Game.Enums;
using Game.Structures;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(Sublevel))]
[Serializable]
public class SublevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Sublevel script = (Sublevel)target; // Получаем ссылку на скрипт

        // Отображаем поля levelName, numberOfMoves и levelStartDialogue
        script.levelName = EditorGUILayout.TextField("Level Name", script.levelName);
        
       
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Стартовый диалог уровня или задание", EditorStyles.boldLabel);
        script.levelStartDialogue = EditorGUILayout.TextArea(script.levelStartDialogue, GUILayout.Height(200));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Количество доступных ходов", EditorStyles.boldLabel);
        script.numberOfMoves = EditorGUILayout.IntField("Number of Moves", script.numberOfMoves);
       
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Задачи уровня", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelTasks"), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Типы нод используемые в уровне", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nodeTypes"), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Типы нод исключения в уровне", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("excludedNodeTypes"), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Количество строк в поле", EditorStyles.boldLabel);
       
        script.rows = EditorGUILayout.IntField("Rows", script.rows);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Количество столбцов", EditorStyles.boldLabel);
        script.cols = EditorGUILayout.IntField("Columns", script.cols);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Поле нод. Здесь можно задать ноду по умолчанию или ноду препятсвие", EditorStyles.boldLabel);
        if (script.nodeField == null || script.nodeField.GetLength(0) != script.rows || script.nodeField.GetLength(1) != script.cols)
        {
            script.nodeField = new NodeType[script.rows, script.cols];
        }
       
        for (int i = 0; i < script.rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < script.cols; j++)
            {
                script.nodeField[i, j] = (NodeType)EditorGUILayout.EnumPopup(script.nodeField[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Поле целей. Здесь можно задать дополнительное свойство ноды(цель)", EditorStyles.boldLabel);
        if (script.targetField == null || script.targetField.GetLength(0) != script.rows || script.targetField.GetLength(1) != script.cols)
        {
            script.targetField = new NodeType[script.rows, script.cols];
        }

        // Отображаем значения массива nodeField с использованием EnumPopup
       
       
        for (int i = 0; i < script.rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < script.cols; j++)
            {
                if (!Application.isPlaying)
                    script.targetField[i, j] = (NodeType)EditorGUILayout.EnumPopup(script.targetField[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        if (GUI.changed) // Проверяем, были ли внесены изменения
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target); // Помечаем объект как измененный
        }
    }
}



