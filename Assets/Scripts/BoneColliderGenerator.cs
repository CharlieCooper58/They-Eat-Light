using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoneColliderGenerator : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField, Range(0f, 1f)] float boneWeightThreshold = 0.5f;
    Transform[] bones;
    MeshCollider[] boneColliders;
    Dictionary<int, List<int>> boneTriangleMappings;

    Mesh sharedMesh;

    int discardedTriangles;

    [SerializeField] int minTrianglesToDefineMesh = 2;

    [SerializeField] bool isCreature;

    private void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        bones = skinnedMeshRenderer.bones;
        StartCoroutine(WaitAndBake());

    }
    IEnumerator WaitAndBake()
    {
        yield return null;
        GenerateBoneColliders();
    }
    void SetUpTriangleMappings()
    {
        boneTriangleMappings = new();
        for (int i = 0; i < bones.Length; i++)
        {
            boneTriangleMappings.Add(i, new List<int>());
        }
    }
    MeshCollider AddMeshColliderToBone(Transform bone)
    {
        GameObject newCollider = new GameObject("BoneCollider_" + gameObject.name);
        newCollider.layer = isCreature?GameManager.creatureScansLayer:GameManager.scanColliderLayer;
        newCollider.transform.parent = bone;
        newCollider.transform.localPosition = Vector3.zero;
        newCollider.transform.localScale = Vector3.one;
        newCollider.transform.localRotation = Quaternion.identity;
        return newCollider.AddComponent<MeshCollider>();
    }
    private void GenerateBoneColliders()
    {
        // Add mesh colliders to all the bones
        SetUpTriangleMappings();
        sharedMesh = new();
        skinnedMeshRenderer.BakeMesh(sharedMesh, true);
        //sharedMesh = skinnedMeshRenderer.sharedMesh;
        BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
        int[] triangles = sharedMesh.triangles;
        Vector3[] vertices = sharedMesh.vertices;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            AssignTriangleToBones(new int[3] { triangles[i], triangles[i + 1], triangles[i + 2] }, boneWeights, boneTriangleMappings);
        }

        foreach( var kvp in boneTriangleMappings)
        {
            var refTriangles = kvp.Value;
            if(refTriangles.Count < 1)
            {
                continue;
            }
            int[] boneTriangles = new int[refTriangles.Count];
            List<Vector3> boneVertices = new List<Vector3>();
            HashSet<int> allVerticesUsedForBone = new HashSet<int>(refTriangles);
            Dictionary<int, int> vertexMappings = new Dictionary<int, int>();

            

            int counter = 0;
            foreach (int oldIndex in allVerticesUsedForBone)
            {
                vertexMappings[oldIndex] = counter++;
                //Matrix4x4 bindpose = skinnedMeshRenderer.sharedMesh.bindposes[kvp.Key];
                Vector3 v = skinnedMeshRenderer.transform.TransformPoint(vertices[oldIndex]);
                Vector3 localPos = bones[kvp.Key].worldToLocalMatrix.MultiplyPoint3x4(v);
                boneVertices.Add(localPos);

                //boneVertices.Add(bones[kvp.Key].InverseTransformPoint(skinnedMeshRenderer.transform.TransformPoint(vertices[oldIndex])));
            }

            for(int i = 0; i < refTriangles.Count; i++)
            {
                boneTriangles[i] = vertexMappings[refTriangles[i]];
            }


            Mesh boneMesh = new();
            boneMesh.SetVertices(boneVertices);
            boneMesh.SetTriangles(boneTriangles, 0);
            boneMesh.RecalculateBounds();

            AddMeshColliderToBone(bones[kvp.Key]).sharedMesh = boneMesh;
        }
    }

    private void AssignTriangleToBones(int[] triangle, BoneWeight[] boneWeights, Dictionary<int, List<int>> boneTriangleMappings)
    {
        Dictionary<int, int> candidateBones = new();
        for(int i = 0; i < 3; i++)
        {
            float[] weights = new float[4]{ boneWeights[triangle[i]].weight0, boneWeights[triangle[i]].weight1, boneWeights[triangle[i]].weight2, boneWeights[triangle[i]].weight3 };
            int[] boneIndices = new int[4] { boneWeights[triangle[i]].boneIndex0, boneWeights[triangle[i]].boneIndex1, boneWeights[triangle[i]].boneIndex2, boneWeights[triangle[i]].boneIndex3 };
            for(int j = 0; j < 4; j++)
            {
                if (weights[j] > boneWeightThreshold)
                {
                    if (candidateBones.ContainsKey(boneIndices[j]))
                    {
                        candidateBones[boneIndices[j]]++;
                    }
                    else
                    {
                        candidateBones.Add(boneIndices[j], 1);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        bool orphanTriangle = true;
        foreach(int key in candidateBones.Keys)
        {
            if (candidateBones[key] > 1)
            {
                orphanTriangle = false;
                for(int j = 0; j<3; j++)
                {
                    boneTriangleMappings[key].Add(triangle[j]);
                }
            }
        }
        if (orphanTriangle)
        {
            discardedTriangles++;
        }
    }
}
