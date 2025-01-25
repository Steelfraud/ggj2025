using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CustomCamera : Singleton<CustomCamera>
{
    public class PickupTarget
    {
        public Transform transform;
        public float timer;

        public PickupTarget(Transform t)
        {
            transform = t;
            timer = 0;
        }
    }

    public CinemachineTargetGroup targetGroup;
    [SerializeField]
    private CinemachineTargetGroup hitTargetGroup;
    private CinemachineGroupFraming groupFraming;
    private CinemachineCamera cinemachineCamera;
    private GameObject worldCenterObject;
    public List<PickupTarget> pickUpList = new List<PickupTarget>();

    [SerializeField]
    private AnimationCurve pickupWeightCurve;

    [SerializeField]
    private float worldCenterWeight = .5f;

    [SerializeField]
    private float pickupWeightReductionSpeed = .1f;


    public void Start()
    {
        CreateSingleton(this, SetDontDestroy);
        groupFraming = GetComponentInChildren<CinemachineGroupFraming>();
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        worldCenterObject = new GameObject();
        worldCenterObject.transform.parent = transform;
        AddToTargetGroup(worldCenterObject.transform, worldCenterWeight);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) 
        {
            StartCoroutine(FocusOnHit());
        }

        ReducePickupWeights();
    }

    private IEnumerator FocusOnHit()
    {
        // Stoppaa peli ?
        var oldDamping = groupFraming.Damping;
        groupFraming.Damping = 0f;
        cinemachineCamera.Target.TrackingTarget = hitTargetGroup.Transform;
        // Focus camera on hit - ignore other targets
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(1);
        groupFraming.Damping = oldDamping;
        cinemachineCamera.Target.TrackingTarget = targetGroup.Transform;
        // Continue game
        Time.timeScale = 1f;
        // Get back to main stuff
    }

    public void AddToTargetGroup(Transform t, float weigth = 1)
    {
        if (targetGroup == null || targetGroup.FindMember(t) != -1)
        {
            return;
        }
        targetGroup.AddMember(t, weigth, .5f);
    }

    public void AddPickupToTargetGroup(Transform t)
    {
        if (pickUpList.Any(x => x.transform == t))
        {
            return;
        }

        pickUpList.Add(new PickupTarget(t));
        AddToTargetGroup(t, pickupWeightCurve.Evaluate(0));
    }

    public void RemoveFromTargetGroup(Transform t)
    {
        if (targetGroup == null || targetGroup.FindMember(t) == -1)
        {
            return;
        }
        var current = pickUpList.FirstOrDefault(x => x.transform == t); 
        if (current is not null)
        {
            pickUpList.Remove(current);
            targetGroup.RemoveMember(current.transform);
            return;
        }
        targetGroup.RemoveMember(t);
    }
    private void ReducePickupWeights()
    {
        var dTime = Time.deltaTime;
        for (int i = pickUpList.Count - 1;  i > -1; i--) 
        { 
            var pickup = pickUpList[i];
            var targetId = targetGroup.FindMember(pickup.transform);
            if (targetId == -1)
            {
                pickUpList.Remove(pickup); // Issue?
                continue;
            }

            pickup.timer += dTime *= pickupWeightReductionSpeed;
            targetGroup.Targets[targetId].Weight = pickupWeightCurve.Evaluate(pickup.timer);
            if (pickup.timer > 1)
            {
                RemoveFromTargetGroup(pickup.transform);
            }
        }
    }
}
