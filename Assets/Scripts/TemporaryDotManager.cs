using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;
public class TemporaryDotManager : MonoBehaviour
{
    public static TemporaryDotManager instance;
    [SerializeField] Mesh dotMesh;
    [SerializeField] float dotScale;
    int dotLayer;

    Dictionary<Vector3, Vector3> dotsGrid;
    [SerializeField] Material indirectInstancedDotMaterial;
    private ComputeBuffer allDotInstancesBuffer;
    private ComputeBuffer argsBuffer;
    [SerializeField, Range(1, 4000000)] int computeBufferSize = 1000000;

    // for now, let's just track the last known memory index
    int computeBufferIterationIndex = 0;
    List<float4> dataToAddThisFrame;
    List<float4> replacementDataThisFrame;
    bool buffersInitialized;


    uint[] args;

    [SerializeField] float temporaryDotLifetime = 5;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        AllocateDotInstancesBuffer();
        dataToAddThisFrame = new List<float4>();
        replacementDataThisFrame = new List<float4>();
        
        dotLayer = LayerMask.NameToLayer("Scan");
    }
    private void OnDestroy()
    {
        allDotInstancesBuffer?.Release();
        argsBuffer?.Release();
    }
    public void SpawnDot(Vector3 dotPosition, float t)
    {
        ExperimentalDotSpawnFunction(dotPosition, t);
    }

    private void ExperimentalDotSpawnFunction(Vector3 dotPosition, float time)
    {
        dataToAddThisFrame.Add(new float4(dotPosition, time));
    }

    private void LateUpdate()
    {
        if (buffersInitialized)
        {
            if (dataToAddThisFrame.Count > 0)
            {
                allDotInstancesBuffer.SetData(dataToAddThisFrame, 0, computeBufferIterationIndex, dataToAddThisFrame.Count);
                computeBufferIterationIndex += dataToAddThisFrame.Count;
                if(computeBufferIterationIndex > computeBufferSize - 10000)
                {
                    computeBufferIterationIndex = 0;
                }
                dataToAddThisFrame.Clear();

                if(args[1] < computeBufferIterationIndex) args[1] = (uint)computeBufferIterationIndex;
                argsBuffer.SetData(args);
            }
            Bounds renderBound = new Bounds();
            renderBound.SetMinMax(new Vector3(-10000, -10000, -10000), new Vector3(10000, 10000, 10000));

            Graphics.DrawMeshInstancedIndirect(dotMesh, 0, indirectInstancedDotMaterial, renderBound, argsBuffer, layer: LayerMask.NameToLayer("Scan"));
        }

    }

    private void AllocateDotInstancesBuffer()
    {
        if (allDotInstancesBuffer != null)
        {
            allDotInstancesBuffer.Release();
        }
        float4[] emptyData = new float4[computeBufferSize];
        allDotInstancesBuffer = new ComputeBuffer(computeBufferSize, 4 * sizeof(float));
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
}
