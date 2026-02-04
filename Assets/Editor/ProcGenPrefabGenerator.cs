using UnityEngine;
using UnityEditor;
using System.IO;

public class ProcGenPrefabGenerator : EditorWindow
{
    public static GameObject DestructibleObject;

    private string[] sizeOptions = { "1x1", "1x2", "1x3", "2x1", "3x1" };
    private int selectedIndex = 0;

    [MenuItem("Tools/Create Procedural Generation Prefabs")]
    public static void ShowWindow() => GetWindow<ProcGenPrefabGenerator>("Prefab Tool");

    private void OnGUI()
    {
        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);
        selectedIndex = EditorGUILayout.Popup("Select Size:", selectedIndex, sizeOptions);

        if (GUILayout.Button("Create Prefabs from Selection"))
        {
            CreatePrefab(sizeOptions[selectedIndex]);
        }
    }
    public static void CreatePrefab(string size)
    {
        Object[] selectedSprites = Selection.objects;
        string genprefabPath = "Assets/Prefabs/ProcGenTestPrefabs";
        string destructfabPath = "Assets/Prefabs/Effects Prefabs/Destructables/DestructiblePrefab.prefab";

        foreach (Object obj in selectedSprites)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(obj));
            if (sprite == null) continue;

            GameObject parentGO = new GameObject(sprite.name +"_"+ size);
            parentGO.transform.position = Vector3.zero;

            GameObject destructAsset = AssetDatabase.LoadAssetAtPath<GameObject>(destructfabPath);
            GameObject destructGO = (GameObject)PrefabUtility.InstantiatePrefab(destructAsset);
            destructGO.transform.SetParent(parentGO.transform);
            destructGO.transform.localPosition = Vector3.zero;

            GameObject childGO = new GameObject("SpriteCollider");
            childGO.transform.SetParent(parentGO.transform);
            childGO.transform.localPosition = Vector3.zero;

            childGO.transform.position = Vector3.zero;
            SpriteRenderer sr = childGO.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            childGO.AddComponent<PolygonCollider2D>();
            childGO.AddComponent<IsoSpriteSorting>();

            string path = genprefabPath + "/" + parentGO.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(parentGO, path);//save as prefab

            Debug.Log("Prefab from Sprite created: " + parentGO.name + ". Saved at " + path);
            DestroyImmediate(parentGO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
