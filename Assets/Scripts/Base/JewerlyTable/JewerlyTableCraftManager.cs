using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ScriptableObjectFinder<T> where T : ScriptableObject
{
    public static List<T> FindAllInstances()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        List<T> assets = new List<T>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                assets.Add(asset);
        }

        return assets;
    }
}
#endif

public class JewerlyTableCraftManager : MonoBehaviour
{
    public static JewerlyTableCraftManager Instance = null;
    [SerializeField] List<JewerlyTableCraft> crafts = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public JewerlyTableCraft GetCraftByName(string name)
    {
        return crafts.Find((x) => x.craftName == name) ?? null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(JewerlyTableCraftManager))]
    public class JewerlyTableCraftManagerEditor : Editor
    {
        private SerializedProperty crafts;

        private void OnEnable()
        {
            crafts = serializedObject.FindProperty("crafts");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            GUILayout.Space(10);

            if (GUILayout.Button("Найти все крафты", GUILayout.Height(30)))
            {
                List<JewerlyTableCraft> allAssets = ScriptableObjectFinder<JewerlyTableCraft>.FindAllInstances();

                // Очистка и заполнение serializedProperty crafts найденными объектами
                crafts.ClearArray();
                for (int i = 0; i < allAssets.Count; i++)
                {
                    crafts.InsertArrayElementAtIndex(i);
                    crafts.GetArrayElementAtIndex(i).objectReferenceValue = allAssets[i];
                }

                serializedObject.ApplyModifiedProperties();

                // Отметить измененный объект для сохранения
                EditorUtility.SetDirty(target);

                EditorUtility.DisplayDialog("Поиск завершен",
                    $"Найдено {allAssets.Count} крафтов",
                    "OK");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
