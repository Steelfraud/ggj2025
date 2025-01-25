using System;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(PlayerAvatar))]
    public class PlayerAvatarFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dashChargeParticles;

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
        }

        void OnDisable()
        {
            Destroy(fxBase.gameObject);

            playerAvatar.OnDashStart -= OnDashStart;
            playerAvatar.OnDashRelease -= OnDashRelease;
        }

        void InitializeFX()
        {
            fxBase = new GameObject("FX Base").transform;
            dashChargeParticles.transform.SetParent(fxBase.transform);
            dashChargeParticles.transform.localPosition = Vector3.down * 0.5f;
        }

        void Update()
        {
            fxBase.position = playerAvatar.transform.position;
            dashChargeParticles.transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, -playerAvatar.PlayerRigidbody.angularVelocity));
        }

        void OnDashStart()
        {
            dashChargeParticles.Play();
        }

        void OnDashRelease()
        {
            dashChargeParticles.Stop();
        }
    }
}