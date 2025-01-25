using Unity.Cinemachine;
using UnityEngine;

public class CustomCamera : Singleton<CustomCamera>
{
    public CinemachineTargetGroup targetGroup;
    private GameObject worldCenterObject;
    public void Start()
    {
        CreateSingleton(this, SetDontDestroy);
        worldCenterObject = new GameObject();
        worldCenterObject.transform.parent = transform;
        AddToTargetGroup(worldCenterObject.transform, .75f);
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
