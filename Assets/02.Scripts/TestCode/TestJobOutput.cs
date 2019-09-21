using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using InTheHand.Devices.Bluetooth;
using InTheHand.Devices.Bluetooth.Rfcomm;
using InTheHand.Devices.Enumeration;

public class TestJobOutput : MonoBehaviour
{
    private void Start()
    {
        NativeArray<byte> result = new NativeArray<byte>(1, Allocator.TempJob);

        SimpleJob simpleJob = new SimpleJob
        {
            a = 1,
            b = 2,
            result = result
        };
        JobHandle jobHandle = simpleJob.Schedule();

        jobHandle.Complete();

        Debug.Log(simpleJob.result[0]);

        result.Dispose();
    }
}

public struct SimpleJob : IJob
{
    public int a;
    public int b;
    public NativeArray<byte> result;

    public void Execute()
    {
        result[0] = 3;
    }
}