using Unity.Cinemachine;
using UnityEngine;

public class CustomCamera : Singleton<CustomCamera>
{
    public CinemachineTargetGroup targetGroup;
    public void Start()
    {
        CreateSingleton(this, SetDontDestroy);
    }

    public void AddToTargetGroup(Transform transform, float weigth = 1)
    {
        if (targetGroup == null || targetGroup.FindMember(transform) != -1)
        {
            return;
        }
        targetGroup.AddMember(transform, weigth, .5f);
    }

    public void RemoveFromTargetGroup(Transform transform)
    {
        if (targetGroup == null || targetGroup.FindMember(transform) != -1)
        {
            return;
        }
        targetGroup.RemoveMember(transform);
    }
}
