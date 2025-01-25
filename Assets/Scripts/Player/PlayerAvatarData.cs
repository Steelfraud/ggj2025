using UnityEngine;

namespace PlayerController
{
    /*public enum MovementType
    {
        Force,
        Torque,
        Both
    }*/

    [CreateAssetMenu(fileName = "New Player Avatar Data", menuName = "Player Controller/Player Avatar Data")]
    public class PlayerAvatarData : ScriptableObject
    {
        //[Header("References")]
        //public PhysicsMaterial PlayerPhysicsMaterial;
        [Header("Pushing")]
        public AnimationCurve AddedPushMultiplierOnMove;
        public AnimationCurve AddedPushMultiplierOnDash;
        [Min(0f)] public float PushedCooldown = 0.2f;

        [Header("Limits")]
        [Min(0f)] public float MoveMaxAngularVelocity = 20f;
        [Min(0f)] public float DashMaxAngularVelocity = 100f;

        [Header("Moving")]
        //public MovementType MovementType = MovementType.Torque;
        //[Min(0f)] public float MoveForce = 10;
        [Min(0f)] public float MoveTorque = 10;
        [Range(0f, 10f)] public float MovePushForceMultiplier = 1;

        [Space]
        public AnimationCurve TurnTorqueAtTurnDot;

        [Header("Dashing")]
        public AnimationCurve DashForceAtChargeTime;
        public AnimationCurve DashBrakingAtChargeTime;
        [Min(0f)] public float DashDuration = 0.5f;
        [Min(0f)] public float DashCooldown = 2;
        [Range(0f, 10f)] public float DashPushForceMultiplier = 1;

        public float GetMaxTurnTorque()
        {
            if (TurnTorqueAtTurnDot == null || TurnTorqueAtTurnDot.keys.Length == 0)
                return 0;


            float maxTurnTorque = float.MinValue;

            for (int i = 0; i < TurnTorqueAtTurnDot.keys.Length; i++)
            {
                if (TurnTorqueAtTurnDot.keys[i].value > maxTurnTorque)
                {
                    maxTurnTorque = TurnTorqueAtTurnDot.keys[i].value;
                }
            }

            return maxTurnTorque;
        }
    }
}