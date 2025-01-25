using UnityEngine;

namespace PlayerController
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerAvatar avatarPrefab;
        private PlayerAvatar spawnedAvatar; public PlayerAvatar SpawnedAvatar { get { return spawnedAvatar; } }

        public PlayerAvatar SpawnPlayerAvatar(Vector3 spawnPosition, GameManager.PlayerColors playerColors)
        {
            spawnedAvatar = GameObject.Instantiate(avatarPrefab, spawnPosition, Quaternion.identity).GetComponent<PlayerAvatar>();
            spawnedAvatar.SetPlayerColor(playerColors);

            return spawnedAvatar;
        }
    }
}