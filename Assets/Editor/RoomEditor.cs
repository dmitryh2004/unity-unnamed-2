using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomScriptable))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Получаем ссылку на объект
        RoomScriptable room = (RoomScriptable)target;

        // Рисуем стандартные поля
        room.id = EditorGUILayout.IntField("Id", room.id);
        room.length = EditorGUILayout.IntField("Length", room.length);
        room.width = EditorGUILayout.IntField("Width", room.width);
        room.height = EditorGUILayout.IntField("Height", room.height);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn directions", EditorStyles.boldLabel);

        // North
        room.north = EditorGUILayout.Toggle("North", room.north);
        if (room.north)
            room.northHeightOffset = EditorGUILayout.IntField("North Height Offset", room.northHeightOffset);
        EditorGUILayout.Space(10);

        // West
        room.west = EditorGUILayout.Toggle("West", room.west);
        if (room.west)
            room.westHeightOffset = EditorGUILayout.IntField("West Height Offset", room.westHeightOffset);
        EditorGUILayout.Space(10);

        // South
        room.south = EditorGUILayout.Toggle("South", room.south);
        if (room.south)
            room.southHeightOffset = EditorGUILayout.IntField("South Height Offset", room.southHeightOffset);
        EditorGUILayout.Space(10);

        // East
        room.east = EditorGUILayout.Toggle("East", room.east);
        if (room.east)
            room.eastHeightOffset = EditorGUILayout.IntField("East Height Offset", room.eastHeightOffset);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn settings", EditorStyles.boldLabel);
        room.minNeighbours = EditorGUILayout.IntSlider("Min Neighbours", room.minNeighbours, 1, 4);
        room.maxNeighbours = EditorGUILayout.IntSlider("Max Neighbours", room.maxNeighbours, 1, 4);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Doors settings", EditorStyles.boldLabel);
        room.hasDoors = EditorGUILayout.Toggle("Has doors", room.hasDoors);
        room.canLockDoors = EditorGUILayout.Toggle("Can lock the doors", room.canLockDoors);

        // Сохраняем изменения
        if (GUI.changed)
        {
            EditorUtility.SetDirty(room);
        }
    }
}

