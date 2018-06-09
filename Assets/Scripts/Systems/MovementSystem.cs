using System.ComponentModel;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : JobComponentSystem
{
    [ComputeJobOptimization]
    struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeed>
    {
        public float topBound;
        public float buttonBound;
        public float deltaTime;


        public void Execute(ref Position position, 
            [ReadOnly(true)] ref Rotation rotation, 
            [ReadOnly(true)] ref MoveSpeed speed)
        {
            float3 pos = position.Value;
            pos += deltaTime * speed.speed * math.forward(rotation.Value);


            if (pos.z < buttonBound)
                pos.z = topBound;

            position.Value = pos;
        }
    }



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gm = GameManager_ECS.Instance;
        var moveJob = new MovementJob
        {
            topBound = gm.topBound,
            buttonBound = gm.bottomBound,
            deltaTime = Time.deltaTime,
        };

        JobHandle moveHandle = moveJob.Schedule(this, 64, inputDeps);
        return moveHandle;
    }
}
