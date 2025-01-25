using UnityEngine;

namespace PlayerController
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerAvatar avatarPrefab;
        private PlayerAvatar spawnedAvatar; public PlayerAvatar SpawnedAvatar { get { return spawnedAvatar; } }

        public PlayerVisualInfo MyColor;

        public PlayerAvatar SpawnPlayerAvatar(Vector3 spawnPosition, PlayerVisualInfo playerColors)
        {
            spawnedAvatar = GameObject.Instantiate(avatarPrefab, spawnPosition, Quaternion.identity).GetComponent<PlayerAvatar>();
            spawnedAvatar.SetPlayerColor(playerColors);
            MyColor = playerColors;

            return spawnedAvatar;
        }
    }
}