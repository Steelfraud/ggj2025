using Unity.Cinemachine;
using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;

    public void AddToTargetGroup(Transform transform)
    {
        if (targetGroup == null || targetGroup.FindMember(transform) != -1)
        {
            return;
        }
        targetGroup.AddMember(transform, 1, .5f);
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
