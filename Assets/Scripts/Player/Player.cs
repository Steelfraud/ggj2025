using System.Collections;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerData data;

        [HideInInspector, SerializeField] private Rigidbody playerRigidbody;

        private Vector3 lastMoveDirection;
        private Coroutine moveRoutine;

        void OnValidate()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        void OnDisable()
        {
            moveRoutine = null;
        }

        private void OnDestroy()
        {
            CustomCamera.Instance.RemoveFromTargetGroup(transform);
        }

        public void Move(Vector3 moveDirection)
        {
            if (data == null)
                return;

            lastMoveDirection = moveDirection;

            if (moveRoutine == null)
            {
                moveRoutine = StartCoroutine(MoveRoutine());
            }
        }

        IEnumerator MoveRoutine()
        {
            WaitForFixedUpdate fixedUpdateWait = new WaitForFixedUpdate();

            while (true)
            {
                if (data.MovementType == MovementType.Torque)
                {
                    playerRigidbody.AddTorque(Vector3.Cross(Vector3.up, lastMoveDirection) * data.MoveTorque);
                }
                else if (data.MovementType == MovementType.Force)
                {
                    playerRigidbody.AddForce(lastMoveDirection * data.MoveForce);
                }
                
                yield return fixedUpdateWait;
            }
        }
    }
}