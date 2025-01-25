using System;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(PlayerAvatar))]
    public class PlayerAvatarFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dashChargeParticles;
        [SerializeField] private ParticleSystem dashShockwaveParticles;
        [SerializeField] private ParticleSystem dashingParticles;

        [HideInInspector, SerializeField] private PlayerAvatar playerAvatar;

        private Transform fxBase;

        void OnValidate()
        {
            playerAvatar = GetComponent<PlayerAvatar>();
        }

        void OnEnable()
        {
            InitializeFX();

            playerAvatar.OnDashStart += OnDashStart;
            playerAvatar.OnDashRelease += OnDashRelease;
            playerAvatar.OnDashEnd += OnDashEnd;
        }

        

        void OnDisable()
        {
            Destroy(fxBase.gameObject);

            playerAvatar.OnDashStart -= OnDashStart;
            playerAvatar.OnDashRelease -= OnDashRelease;
            playerAvatar.OnDashEnd -= OnDashEnd;
        }

        void InitializeFX()
        {
            fxBase = new GameObject("FX Base").transform;
            dashChargeParticles.transform.SetParent(fxBase.transform);
            dashShockwaveParticles.transform.SetParent(fxBase.transform);
            dashChargeParticles.transform.localPosition = Vector3.down * 0.5f;
            dashShockwaveParticles.transform.localPosition = Vector3.zero;
        }

        void Update()
        {
            fxBase.position = playerAvatar.transform.position;
            dashChargeParticles.transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, -playerAvatar.PlayerRigidbody.angularVelocity));
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
    }
}