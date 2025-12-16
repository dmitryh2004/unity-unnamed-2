using UnityEngine;
using UnityEditor;
using System.Linq;

public class BatchLODAdder : EditorWindow
{
    [MenuItem("Tools/Add LOD to LootableItem Prefabs")]
    static void AddLODToLootables()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int processed = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null || !PrefabUtility.IsPartOfPrefabAsset(prefab)) continue;

            LootableItem lootable = prefab.GetComponent<LootableItem>();
            if (lootable == null) continue;

            // Добавляем LODGroup на корень
            LODGroup lodGroup = prefab.GetComponent<LODGroup>();
            if (lodGroup == null) lodGroup = prefab.AddComponent<LODGroup>();

            // Собираем все MeshRenderer в иерархии
            MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>(true).Where(r => r != null).ToArray();
            if (renderers.Length == 0) continue;

            // Настраиваем LOD уровни: LOD0 (полный), LOD1 (culling <10% экрана)
            LOD[] lods = new LOD[2];
            lods[0] = new LOD(0.9f, renderers);  // LOD0 до 90%
            lods[1] = new LOD(0.0f, renderers);  // LOD1 cull ниже 10%

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();

            // Сохраняем префаб
            EditorUtility.SetDirty(prefab);
            PrefabUtility.SavePrefabAsset(prefab);

            processed++;
            Debug.Log($"LOD добавлен к {path}", prefab);
        }

        Debug.Log($"Обработано префабов с LootableItem: {processed}");
        AssetDatabase.SaveAssets();  // Финальный флуш
        AssetDatabase.Refresh();
    }
}
