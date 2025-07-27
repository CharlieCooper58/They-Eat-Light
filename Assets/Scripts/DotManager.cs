using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using Unity.Mathematics;
using System;
public class DotManager : MonoBehaviour
{
    public static DotManager instance;
    [SerializeField] Mesh dotMesh;
    [SerializeField] Material dotMaterial;
    List<List<Matrix4x4>> matrixBatches;
    [SerializeField] float dotScale;
    int dotLayer;


    [Header("Experimental Indirect GPU Instancing")]
    [SerializeField] bool useExperimentalDotSpawning = false;

    Dictionary<Vector3,Vector3> dotsGrid;
    [SerializeField] Material indirectInstancedDotMaterial;
    private ComputeBuffer allDotInstancesBuffer;
    private ComputeBuffer argsBuffer;
    [Range(1, 4000000)] int computeBufferSize = 3000000;

    // for now, let's just track the last known memory index
    int computeBufferIterationIndex = 0;
    List<float3> dataToAddThisFrame;
    bool buffersInitialized;


    uint[] args;

    //temporary dots
    private ComputeBuffer temporaryDotInstancesBuffer;
    private ComputeBuffer temporaryDotArgsBuffer;
    private ComputeBuffer temporaryDotIndicesBuffer;
    [SerializeField] int temporaryDotBufferSize = 50000;
    uint[] temporaryBufferArgs;
    List<float3> temporaryDotsToAddThisFrame;
    int temporaryBufferIterationIndex = 0;
    [SerializeField] Material temporaryDotMaterial;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (useExperimentalDotSpawning)
        {
            AllocateDotInstancesBuffer();
            dataToAddThisFrame = new List<float3>();
        }
        else
        {
            matrixBatches = new List<List<Matrix4x4>>
            {
                new List<Matrix4x4>()
            };
        }

        dotLayer = LayerMask.NameToLayer("Scan");
    }
    private void OnDestroy()
    {
        allDotInstancesBuffer?.Release();
        argsBuffer?.Release();
    }
    public void SpawnDot(Vector3 dotPosition)
    {
        if (useExperimentalDotSpawning)
        {
            ExperimentalDotSpawnFunction(dotPosition);
            return;
        }
        Matrix4x4 newDotMatrix = Matrix4x4.TRS(dotPosition, Quaternion.identity, dotScale*Vector3.one);
        if (matrixBatches.Last().Count > 999)
        {
            matrixBatches.Add(new List<Matrix4x4>());
        }
        matrixBatches.Last().Add(newDotMatrix);

    }

    private void ExperimentalDotSpawnFunction(Vector3 dotPosition)
    {
        dataToAddThisFrame.Add(dotPosition);
    }

    private void Update()
    {
        if (useExperimentalDotSpawning)
        {
            return;
        }
        if(matrixBatches[0].Count == 0) { return; }
        foreach(List<Matrix4x4> matrixBatch in matrixBatches) 
        {
            Graphics.DrawMeshInstanced(dotMesh, 0, dotMaterial, matrixBatch, null, ShadowCastingMode.Off, false, layer:dotLayer);
        }
    }
    private void LateUpdate()
    {
        if (useExperimentalDotSpawning && buffersInitialized)
        {
            if (dataToAddThisFrame.Count > 0)
            {
                allDotInstancesBuffer.SetData(dataToAddThisFrame, 0, computeBufferIterationIndex, dataToAddThisFrame.Count);
                computeBufferIterationIndex += dataToAddThisFrame.Count;
                dataToAddThisFrame.Clear();

                args[1] = (uint)computeBufferIterationIndex;
                argsBuffer.SetData(args);
            }
            Bounds renderBound = new Bounds();
            renderBound.SetMinMax(new Vector3(-10000, -10000, -10000), new Vector3(10000, 10000, 10000));

            Graphics.DrawMeshInstancedIndirect(dotMesh, 0, indirectInstancedDotMaterial, renderBound, argsBuffer, layer: LayerMask.NameToLayer("Scan"));
        }

    }

    private void AllocateDotInstancesBuffer()
    {
        if(allDotInstancesBuffer != null)
        {
            allDotInstancesBuffer.Release();
        }
        float3[] emptyData = new float3[computeBufferSize];
        allDotInstancesBuffer = new ComputeBuffer(computeBufferSize, 3*sizeof(float));
        allDotInstancesBuffer.SetData(emptyData);
        computeBufferIterationIndex = 0;

        args = new uint[5] { 0, 0, 0, 0, 0 };
        args[0] = dotMesh.GetIndexCount(0);
        args[1] = 0;
        args[2] = dotMesh.GetIndexStart(0);
        args[3] = dotMesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        indirectInstancedDotMaterial.SetBuffer("_AllInstancesTransformBuffer", allDotInstancesBuffer);
        indirectInstancedDotMaterial.SetFloat("_DotScale", dotScale);

        buffersInitialized = true;
    }
    /*
    private void AllocateTemporaryDotsBuffer()
    {
        if(temporaryDotInstancesBuffer != null)
        {
            temporaryDotInstancesBuffer.Release();
        }
        float3[] emptyData = new float3[temporaryDotBufferSize];
        temporaryDotInstancesBuffer = new ComputeBuffer(temporaryDotBufferSize, 3*sizeof(float));
        temporaryDotInstancesBuffer.SetData(emptyData);
        temporaryBufferIterationIndex = 0;
        temporaryBufferArgs = new uint[5] { 0, 0, 0, 0, 0 };
        temporaryBufferArgs[0] = dotMesh.GetIndexCount(0);
        temporaryBufferArgs[1] = (uint)temporaryDotBufferSize;
        temporaryBufferArgs[2] = dotMesh.GetIndexStart(0);
        temporaryBufferArgs[3] = dotMesh.GetBaseVertex(0);
        temporaryBufferArgs[4] = 0;
        temporaryDotArgsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        temporaryDotArgsBuffer.SetData(temporaryBufferArgs);

        temporaryDotMaterial.SetBuffer
    } */
}
