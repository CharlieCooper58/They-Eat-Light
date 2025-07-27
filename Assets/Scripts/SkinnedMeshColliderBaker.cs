using System.Collections;
using UnityEngine;

public class SkinnedMeshColliderBaker : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;
    [SerializeField] bool bakeAtStart = true;


    float nextBakeTime = 0;
    void Start()
    {
        // Get references to the SkinnedMeshRenderer and MeshCollider components
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        // Initialize the baked mesh
        bakedMesh = new Mesh();

        // Optionally, bake immediately at the start
        StartCoroutine("BakeAfterDelay");
    }

    IEnumerator BakeAfterDelay()
    {
        yield return new WaitForSeconds(2);
        BakeMesh();
    }
    private void Update()
    {
        TryBakeMesh();
    }

    // Call this method to bake the skinned mesh and update the mesh collider
    public void BakeMesh()
    {
        if (skinnedMeshRenderer == null || meshCollider == null)
        {
            Debug.LogError("Missing SkinnedMeshRenderer or MeshCollider components.");
            return;
        }

        // Bake the skinned mesh into a static mesh
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Assign the baked mesh to the mesh collider
        meshCollider.sharedMesh = bakedMesh;
    }
    public void TryBakeMesh()
    {
        if(Time.time > nextBakeTime)
        {
            BakeMesh();
            nextBakeTime = Time.time + .25f;
        }
    }
}
