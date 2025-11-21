using UnityEngine;
using UnityEditor;

public class PrintModelBounds
{
    [MenuItem("Tools/Print Selected Model Bounds")]
    static void PrintSelectedModelBounds()
    {
        GameObject go = Selection.activeGameObject;

        if (go == null)
        {
            Debug.LogError("❌ No object selected! Please select your model root.");
            return;
        }

        // Combine bounds of all renderers
        var renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogError("❌ No Mesh Renderers found in selected object!");
            return;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        Vector3 size = bounds.size;

        Debug.Log($"✅ Total Bounds Size for {go.name}:  X={size.x:F3}  Y={size.y:F3}  Z={size.z:F3}");
    }
}
