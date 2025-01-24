using UnityEngine;

namespace PlayerController
{
    public enum MovementType
    {
        Force,
        Torque
    }

    [CreateAssetMenu(fileName = "New Player Data", menuName = "Player Controller/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public MovementType MovementType = MovementType.Torque;
        public float MoveForce = 10;
        public float MoveTorque = 10;
    }
}