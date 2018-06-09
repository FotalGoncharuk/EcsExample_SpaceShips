using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[ComputeJobOptimization]
public struct MovementJob : IJobParallelForTransform {

    // ===== Data =====
    public float moveSpeed;
    public float topBound;
    public float buttonBound;
    public float deltaTime;


    public void Execute(int index, TransformAccess transform)
    {
        var pos = transform.position;
        pos += moveSpeed * deltaTime * (transform.rotation * Vector3.forward);

        if (pos.y < buttonBound)
            pos.y = topBound;

        transform.position = pos;
    }
}
