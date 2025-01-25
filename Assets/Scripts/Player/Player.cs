using System.Collections;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerData data;
        [SerializeField] private Collider playerCollider;

        [HideInInspector, SerializeField] private Rigidbody playerRigidbody;

        private Vector3 lastMoveDirection;
        private Vector3 lastNonZeroMoveDirection = new Vector3(0f, 0f, 1f);
        private float dashForce;
        private Coroutine moveRoutine;
        private Coroutine startDashRoutine;
        private Coroutine releaseDashRoutine;

        private float defaultStaticFriction;
        private float defaultDynamicFriction;
        private float canDashAtTime;

        [HideInInspector]
        public float UltimateForce;
        [HideInInspector]
        public bool ultimateFormEnabled;


        void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        void Awake()
        {
            defaultStaticFriction = data.PlayerPhysicsMaterial.staticFriction;
            defaultDynamicFriction = data.PlayerPhysicsMaterial.dynamicFriction;
            playerRigidbody.maxAngularVelocity = data.MoveMaxAngularVelocity;
        }

        void OnDisable()
        {
            moveRoutine = null;
        }

        private void OnDestroy()
        {
            //CustomCamera.Instance.RemoveFromTargetGroup(transform);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (ultimateFormEnabled && collision.gameObject.GetComponent<Rigidbody>())
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(UltimateForce * transform.forward, ForceMode.VelocityChange);
            }
        }

        public void Move(Vector3 moveDirection)
        {
            if (data == null)
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

        public IEnumerator UltimateForm(float durationSeconds, float force)
        {
            UltimateForce = force;
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
                //UpdateLimits();

                velocityToInputDot = Vector3.Dot(playerRigidbody.linearVelocity.normalized, lastMoveDirection);
                turnTorque = data.TurnTorqueAtTurnDot.Evaluate(velocityToInputDot);

                if (data.MovementType == MovementType.Torque || data.MovementType == MovementType.Both)
                {
                    playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, lastMoveDirection) * data.MoveTorque);
                }
                if (data.MovementType == MovementType.Force || data.MovementType == MovementType.Both)
                {
                    playerRigidbody.AddForce(lastMoveDirection * data.MoveForce);
                }

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

            while (true)
            {
                //playerRigidbody.AddTorque(-playerRigidbody.angularVelocity * data.DashBrakingAtChargeTime.Evaluate(timer));
                
                dashForce = data.DashForceAtChargeTime.Evaluate(timer);
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
            
            //Vector3 dashDirection = lastNonZeroMoveDirection.normalized;

            while (timer < data.DashDuration)
            {
                //playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, dashDirection) * dashForce, ForceMode.VelocityChange);

                timer += Time.fixedDeltaTime;
                playerRigidbody.maxAngularVelocity = Mathf.Lerp(data.DashMaxAngularVelocity, data.MoveMaxAngularVelocity, timer / data.DashDuration);
                yield return fixedUpdateWait;
            }
            
            releaseDashRoutine = null;
        }
    }
}