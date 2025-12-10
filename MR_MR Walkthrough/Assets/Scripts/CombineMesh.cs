using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
{
    void Start()
    {
        Optimize();
    }
    public void Optimize()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].transform == transform)
            {
                i++;
                continue; // Skip the parent mesh filter
            }

            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            // Disable original
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        MeshFilter mf = transform.GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        // Enable the final combined mesh
        transform.gameObject.SetActive(true);
    }
}
