using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PlayerController
{
    [RequireComponent(typeof(PlayerAvatar))]
    public class PlayerAvatarFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dashChargeParticles;
        [SerializeField] private ParticleSystem dashShockwaveParticles;
        [SerializeField] private ParticleSystem dashingParticles;
        [SerializeField] private ParticleSystem pushedParticles;
        [SerializeField] private DecalProjector heightShadowDecal;

        [HideInInspector, SerializeField] private PlayerAvatar playerAvatar;

        private Transform fxBase;

        void OnValidate()
        {
            playerAvatar = GetComponent<PlayerAvatar>();
        }

        void OnEnable()
        {
            InitializeFX();

            playerAvatar.OnDashStart.AddListener(OnDashStart);
            playerAvatar.OnDashRelease.AddListener(OnDashRelease);
            playerAvatar.OnDashEnd.AddListener(OnDashEnd);
            playerAvatar.OnPushed += OnPushed;
        }

        void OnDisable()
        {
            if (fxBase != null)
            {
                Destroy(fxBase.gameObject);
            }

            playerAvatar.OnDashStart.RemoveListener(OnDashStart);
            playerAvatar.OnDashRelease.RemoveListener(OnDashRelease);
            playerAvatar.OnDashEnd.RemoveListener(OnDashEnd);
            playerAvatar.OnPushed -= OnPushed;
        }

        void InitializeFX()
        {
            fxBase = new GameObject("FX Base").transform;

            dashChargeParticles.transform.SetParent(fxBase.transform);
            dashShockwaveParticles.transform.SetParent(fxBase.transform);
            heightShadowDecal.transform.SetParent(fxBase.transform);

            dashChargeParticles.transform.localPosition = Vector3.down * 0.5f;
            dashShockwaveParticles.transform.localPosition = Vector3.zero;
            heightShadowDecal.transform.localPosition = Vector3.zero;
        }

        void Update()
        {
            fxBase.position = playerAvatar.transform.position;

            Vector3 newDirection = Vector3.Cross(Vector3.up, -playerAvatar.PlayerRigidbody.angularVelocity);

            if (newDirection != Vector3.zero)
            {
                dashChargeParticles.transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }

        void OnDashStart()
        {
            dashingParticles.Play();
            dashChargeParticles.Play();
        }

        void OnDashRelease()
        {
            dashChargeParticles.Stop();
            dashingParticles.Play();
            dashShockwaveParticles.transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, -playerAvatar.PlayerRigidbody.angularVelocity));
            dashShockwaveParticles.Play();
        }

        void OnDashEnd()
        {
            dashingParticles.Stop();
        }

        void OnPushed(Transform pushed, Transform pusher, Vector3 pushForce)
        {
            dashChargeParticles.Stop();
            dashingParticles.Stop();
            pushedParticles.Play();
        }
    }
}