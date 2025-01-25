using PlayerController;
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

    [SerializeField]
    private float pushCameraEffectLimit = 30f;

    [SerializeField]
    private float pushCameraEffectCooldown = 1f;
    private bool canPushCameraEffect = true;

    [SerializeField]
    private float freezeDuration = 1f;
    [SerializeField]
    private float freezeTimeScale = 0.2f;

    [SerializeField]
    private float targetRadius = 5f;
    public void Start()
    {
        CreateSingleton(this, SetDontDestroy);
        groupFraming = GetComponentInChildren<CinemachineGroupFraming>();
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        worldCenterObject = new GameObject();
        worldCenterObject.transform.parent = transform;
        AddToTargetGroup(worldCenterObject.transform, worldCenterWeight);
        PlayerAvatar.OnAnyPlayerPushed += OnPlayerPush; 
    }

    public void OnDestroy()
    {
        PlayerAvatar.OnAnyPlayerPushed -= OnPlayerPush; 
    }

    private void OnPlayerPush(Transform pushed, Transform pusher, Vector3 pushForce)
    {
        if (pushForce.magnitude < pushCameraEffectLimit || !canPushCameraEffect)
        {
            return;
        }
        canPushCameraEffect = false;
        hitTargetGroup.AddMember(pushed, 1, 1);

        if (pusher.GetComponentInChildren<PlayerAvatar>())
        {
            hitTargetGroup.AddMember(pusher, 1, 1);
        }
        StartCoroutine(FocusOnHit());
    }

    public void Update()
    {
        ReducePickupWeights();
    }

    private IEnumerator FocusOnHit()
    {
        // Stoppaa peli ?
        var oldDamping = groupFraming.Damping;
        groupFraming.Damping = 0f;
        cinemachineCamera.Target.TrackingTarget = hitTargetGroup.Transform;
        // Focus camera on hit - ignore other targets
        Time.timeScale = freezeTimeScale;
        yield return new WaitForSecondsRealtime(freezeDuration);
        groupFraming.Damping = oldDamping;
        cinemachineCamera.Target.TrackingTarget = targetGroup.Transform;
        hitTargetGroup.Targets.Clear();
        // Continue game
        Time.timeScale = 1f;
        // Get back to main stuff
        yield return new WaitForSecondsRealtime(pushCameraEffectCooldown);
        canPushCameraEffect = true;
    }

    public void AddToTargetGroup(Transform t, float weigth = 1)
    {
        if (targetGroup == null || targetGroup.FindMember(t) != -1)
        {
            return;
        }
        targetGroup.AddMember(t, weigth, targetRadius);
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
