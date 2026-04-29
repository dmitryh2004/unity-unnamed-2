using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Article))]
public class ArticleEditor : Editor
{
    private SerializedProperty articlePartsProperty, headerProperty;

    private void OnEnable()
    {
        articlePartsProperty = serializedObject.FindProperty("articleParts");
        headerProperty = serializedObject.FindProperty("header");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(headerProperty);
        EditorGUILayout.PropertyField(articlePartsProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Добавить элементы:", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Текст", GUILayout.Height(25)))
            {
                AddArticlePart(typeof(ArticlePartText));
            }
            if (GUILayout.Button("Изображение", GUILayout.Height(25)))
            {
                AddArticlePart(typeof(ArticlePartImage));
            }
            if (GUILayout.Button("Ссылка", GUILayout.Height(25)))
            {
                AddArticlePart(typeof(ArticlePartLink));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddArticlePart(System.Type partType)
    {
        articlePartsProperty.arraySize++;
        int index = articlePartsProperty.arraySize - 1;

        SerializedProperty element = articlePartsProperty.GetArrayElementAtIndex(index);
        element.managedReferenceValue = (ArticlePart)System.Activator.CreateInstance(partType);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
