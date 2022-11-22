using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class NewDestinationSystem : SystemBase
{
    private RandomSystem randomSystem;

    protected override void OnCreate()
    {
        randomSystem = World.GetExistingSystem<RandomSystem>();
    }

    protected override void OnUpdate()
    {
        var randomArray = randomSystem.RandomArray;

        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) =>
            {

                float distance = math.abs(math.length(destination.value - translation.Value));

                if (distance < 0.1f)
                {
                    var random = randomArray[nativeThreadIndex];

                    destination.value.x = random.NextFloat(0, 500);
                    destination.value.z = random.NextFloat(0, 500);

                    randomArray[nativeThreadIndex] = random;
                }

            }).ScheduleParallel();
    }
}
