using System.Collections;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAvatar : MonoBehaviour
    {
        public GameManager.PlayerColors MyColor;

        public PlayerModifierHandler PlayerModifierHandler => modifierHandler;

        [HideInInspector]
        public float UltimateForce;
        [HideInInspector]
        public bool ultimateFormEnabled;

        [SerializeField] private PlayerModifierHandler modifierHandler;
        [SerializeField] private PlayerAvatarData data;
        [SerializeField] private Collider playerCollider;
        [SerializeField] private MeshRenderer playerRenderer;

        [HideInInspector, SerializeField] private Rigidbody playerRigidbody; public Rigidbody PlayerRigidbody { get { return playerRigidbody; } }

        public delegate void PlayerPushedAction(Transform pushed, Transform pusher, Vector3 pushForce);
        public static event PlayerPushedAction OnAnyPlayerPushed; 

        private bool isDashing; public bool IsDashing { get { return isDashing; } }
        private float pushMultiplier = 1f; 

        /// <summary>
        /// This value gets increased after getting pushed
        /// </summary>
        public float PushMultiplier { get { return pushMultiplier; } }

        private Vector3 currentVelocity;
        private Vector3 lastMoveDirection;
        private Vector3 lastNonZeroMoveDirection = new Vector3(0f, 0f, 1f);
        private float dashForce;
        private Coroutine moveRoutine;
        private Coroutine startDashRoutine;
        private Coroutine releaseDashRoutine;

        private float defaultStaticFriction;
        private float defaultDynamicFriction;
        private float canDashAtTime;
        private float canPushAtTime;

        void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        void Awake()
        {
            defaultStaticFriction = playerCollider.material.staticFriction;
            defaultDynamicFriction = playerCollider.material.dynamicFriction;
            playerRigidbody.maxAngularVelocity = data.MoveMaxAngularVelocity;
        }

        void OnDisable()
        {
            pushMultiplier = 1f;
            canDashAtTime = 0f;
            canPushAtTime = 0f;
            isDashing = false;
            moveRoutine = null;
        }

        private void OnDestroy()
        {
            if (CustomCamera.Instance != null)
                CustomCamera.Instance.RemoveFromTargetGroup(transform);
        }

        void FixedUpdate()
        {
            currentVelocity = playerRigidbody.linearVelocity;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.tag == "Player" && collision.collider.TryGetComponent(out PlayerAvatar playerAvatar))
            {
                (PlayerAvatar collisionLoser, PlayerAvatar collisionWinner, Vector3 pushForce) = ResolvePlayerCollision(this, playerAvatar);

                // If collision loser is null, it didn't have a dashing player
                if (collisionLoser != null)
                {
                    collisionLoser.Push(collisionWinner.transform, pushForce, collisionWinner.isDashing ? data.AddedPushMultiplierOnDash.Evaluate(pushForce.magnitude) : data.AddedPushMultiplierOnMove.Evaluate(pushForce.magnitude));
                }
            }
            if (ultimateFormEnabled && collision.collider.TryGetComponent(out Rigidbody hitBody))
            {
                hitBody.AddForce(UltimateForce * (collision.transform.position - transform.position), ForceMode.VelocityChange);
            }
        }

        (PlayerAvatar collisionLoser, PlayerAvatar collisionWinner, Vector3 pushForce) ResolvePlayerCollision(PlayerAvatar avatarA, PlayerAvatar avatarB)
        {
            if (avatarA.currentVelocity.magnitude > avatarB.currentVelocity.magnitude)
            {
                if (avatarA.IsDashing)
                {
                    return (avatarB, avatarA, (avatarB.transform.position - avatarA.transform.position).normalized * avatarA.currentVelocity.magnitude * avatarA.data.DashPushForceMultiplier);
                }
                else
                {
                    return (avatarB, avatarA, (avatarB.transform.position - avatarA.transform.position).normalized * avatarA.currentVelocity.magnitude * avatarA.data.MovePushForceMultiplier);
                }
            }
            else
            {
                if (avatarB.IsDashing)
                {
                    return (avatarA, avatarB, (avatarA.transform.position - avatarB.transform.position).normalized * avatarB.currentVelocity.magnitude * avatarA.data.DashPushForceMultiplier);
                }
                else
                {
                    return (avatarA, avatarB, (avatarA.transform.position - avatarB.transform.position).normalized * avatarB.currentVelocity.magnitude * avatarA.data.MovePushForceMultiplier);
                }
            }
        }

        public void Freeze()
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        public void Push(Transform pusher, Vector3 pushForce, float addedPushMultiplier)
        {
            if (Time.time < canPushAtTime)
                return;

            Debug.Log("Pushed: " + gameObject.GetInstanceID() + " | Final Force: " + (pushForce.magnitude * pushMultiplier).ToString("F2") + " | Just Multiplier: " + pushMultiplier.ToString("F2") + " | Frame: " + Time.frameCount);
            canPushAtTime = Time.time + data.PushedCooldown;
            playerRigidbody.AddForce(pushForce * pushMultiplier, ForceMode.VelocityChange);
            pushMultiplier += addedPushMultiplier;

            OnAnyPlayerPushed?.Invoke(this.transform, pusher, pushForce);
        }

        public void SetPlayerColor(GameManager.PlayerColors color)
        {
            MyColor = color;
            playerRenderer.material = color.PlayerMaterial;
        }

        public void Move(Vector3 moveDirection)
        {
            if (data == null )
                return;

            lastMoveDirection = moveDirection;

            if (lastMoveDirection != Vector3.zero)
            {
                lastNonZeroMoveDirection = lastMoveDirection;
            }

            if (moveRoutine == null)
            {
                moveRoutine = StartCoroutine(MoveRoutine());
            }
        }

        public void StartDash()
        {
            if (data == null || Time.time < canDashAtTime)
                return;

            if (startDashRoutine != null)
            {
                StopCoroutine(startDashRoutine);
            }
            if (releaseDashRoutine != null)
            {
                StopCoroutine(releaseDashRoutine);
                releaseDashRoutine = null;
            }

            startDashRoutine = StartCoroutine(StartDashRoutine());
        }

        public void ReleaseDash()
        {
            // Don't allow release if dash hasn't been charged
            if (data == null || startDashRoutine == null)
                return;

            StopCoroutine(startDashRoutine);
            startDashRoutine = null;
            releaseDashRoutine = StartCoroutine(ReleaseDashRoutine());
        }

        public void TriggerUltimateForm(float durationSeconds, float force)
        {
            UltimateForce = force;
            StartCoroutine(UltimateForm(durationSeconds));
        }

        IEnumerator UltimateForm(float durationSeconds)
        {
            ultimateFormEnabled = true;
            yield return new WaitForSeconds(durationSeconds);
            ultimateFormEnabled = false;
        }

        IEnumerator MoveRoutine()
        {
            WaitForFixedUpdate fixedUpdateWait = new WaitForFixedUpdate();
            float velocityToInputDot;
            float turnTorque;
            float maxTurnTorque = data.GetMaxTurnTorque();

            while (true)
            {
                velocityToInputDot = Vector3.Dot(playerRigidbody.linearVelocity.normalized, lastMoveDirection);
                turnTorque = data.TurnTorqueAtTurnDot.Evaluate(velocityToInputDot);
                playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, lastMoveDirection) * (data.MoveTorque + modifierHandler.GetValueModifier(ModifiedValueNumber.MoveTorque)));

                Debug.DrawRay(playerRigidbody.transform.position + Vector3.up * 0.5f, lastMoveDirection * turnTorque / maxTurnTorque, Color.green, Time.fixedDeltaTime);

                playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, lastMoveDirection) * turnTorque);

                yield return fixedUpdateWait;
            }
        }

        IEnumerator StartDashRoutine()
        {
            WaitForFixedUpdate fixedUpdateWait = new WaitForFixedUpdate();

            float timer = 0f;
            playerRigidbody.maxAngularVelocity = data.DashMaxAngularVelocity;
            playerCollider.material.staticFriction = 0f;
            playerCollider.material.dynamicFriction = 0f;
            isDashing = false;

            while (true)
            {
                dashForce = data.DashForceAtChargeTime.Evaluate(timer) + modifierHandler.GetValueModifier(ModifiedValueNumber.DashForce);
                playerRigidbody.AddForce(-playerRigidbody.linearVelocity * data.DashBrakingAtChargeTime.Evaluate(timer));
                playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, lastNonZeroMoveDirection.normalized) * dashForce, ForceMode.VelocityChange);

                timer += Time.fixedDeltaTime;
                yield return fixedUpdateWait;
            }
        }

        IEnumerator ReleaseDashRoutine()
        {
            WaitForFixedUpdate fixedUpdateWait = new WaitForFixedUpdate();
            yield return fixedUpdateWait;

            Debug.DrawRay(transform.position, lastNonZeroMoveDirection * 2f, Color.magenta, data.DashDuration);
            float timer = 0f;

            canDashAtTime = Time.time + data.DashCooldown;
            playerCollider.material.staticFriction = defaultStaticFriction;
            playerCollider.material.dynamicFriction = defaultDynamicFriction;
            isDashing = true;

            while (timer < data.DashDuration)
            {
                timer += Time.fixedDeltaTime;
                playerRigidbody.maxAngularVelocity = Mathf.Lerp(data.DashMaxAngularVelocity, data.MoveMaxAngularVelocity, timer / data.DashDuration);
                yield return fixedUpdateWait;
            }

            isDashing = false;
            releaseDashRoutine = null;
        }
    }
}