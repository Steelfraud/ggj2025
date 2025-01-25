using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerController
{
    [RequireComponent(typeof(Player), typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
        [HideInInspector, SerializeField] private Player player;

        private Camera inputCamera;
        public Camera InputCamera 
        { 
            get 
            { 
                if (inputCamera != null)
                {
                    return inputCamera; 
                }
                else
                {
                    inputCamera = Camera.main;

                    if (inputCamera != null)
                    {
                        return inputCamera;
                    }
                    else
                    {
                        Debug.LogError("No main camera in scene. Player input requires one!");
                        return null;
                    }
                }
            } 
        }

        void OnValidate()
        {
            player = GetComponent<Player>();
        }

        void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            
            if (TryConvertInputToCameraSpace(inputVector, out Vector3 directionInWord))
            {
                player.SpawnedAvatar.Move(directionInWord);
            }
        }

        void OnDash(InputValue inputValue)
        {
            float inputPressed = inputValue.Get<float>();

            if (inputPressed > 0)
            {
                player.SpawnedAvatar.StartDash();
            }
            else
            {
                player.SpawnedAvatar.ReleaseDash();
            }
        }

        void OnRestart(InputValue inputValue)
        {
            if (!inputValue.isPressed)
                return;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        bool TryConvertInputToCameraSpace(Vector2 inputValue, out Vector3 directionInWorld)
        {
            if (InputCamera == null)
            {
                directionInWorld = Vector3.zero;
                return false;
            }

            // Flatten camera direction horizontally
            Vector3 cameraDirection = InputCamera.transform.forward;
            cameraDirection.y = 0;
            cameraDirection = cameraDirection.normalized;

            Vector3 convertedInput = new Vector3(inputValue.x, 0f, inputValue.y);
            directionInWorld = Quaternion.LookRotation(cameraDirection) * convertedInput;

            return true;
        }
    }
}