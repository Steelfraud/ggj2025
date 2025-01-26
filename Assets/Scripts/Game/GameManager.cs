
using PlayerController;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    
    [Header("References")]
    public GameUI UI;
    public BubbleSpawner Spawner;
    public Transform PlayerSpawnPos;
    public PlayerInputManager PlayerInputManager;
    public List<Transform> PlayerSpawnPositions;

    [Header("Game Settings")]
    public bool StartGameOnStart = true;
    public bool AutomaticRestart = true;
    public AnimationCurve PickUpTimerCurve;
    public float TimeBeforeFirstPickUp = 0f;
    public float PickupMinimumTime = 1f;
    public float PickupMaximumTime = 10f;
    public int MaximumPickUps = 0;

    public bool GameGoing => gameOngoing;
    public float RoundTimer => gameTimer;

    private float gameTimer = 0;
    private float timeTillNextPickup = 0f;
    private bool gameOngoing = false;
    private int highestPlayerCount = 0;
    private List<PickUpSpawnPosition> pickUpSpawns = new List<PickUpSpawnPosition>(); 
    private List<PlayerPickUpObjectBase> activePickUps = new List<PlayerPickUpObjectBase>();
    public List<PlayerAvatar> activePlayers = new List<PlayerAvatar>();
    public List<Player> joinedPlayers = new List<Player>();
    private List<Transform> usedSpawnPositions = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CreateSingleton(this, SetDontDestroy))
        {
            Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            SceneManager.LoadScene(0);
        }

        if (gameOngoing && this.activePlayers.Count > 0)
        {
            gameTimer += Time.deltaTime;
            timeTillNextPickup -= Time.deltaTime;

            if (timeTillNextPickup < 0f)
            {
                CreatePickUp();
            }
        }
    }

    private void Initialize()
    {
        pickUpSpawns = FindObjectsByType<PickUpSpawnPosition>(FindObjectsSortMode.None).ToList();

        if (StartGameOnStart) 
        { 
            GameStart();
        }
    }

    public void RemovePickUp(PlayerPickUpObjectBase pickUp)
    {
        activePickUps.Remove(pickUp);

        PickUpSpawnPosition spawn = pickUpSpawns.Find(x => x.ActivePickUp == pickUp);

        if (spawn != null)
        {
            spawn.ActivePickUp = null;
        }
    }

    public void KillPlayer(PlayerAvatar player)
    {
        player.PlayerSFXHandler.PlaySoundEffect("Death");
        player.Kill();
        activePlayers.Remove(player);
        player.gameObject.SetActive(false);
        CustomCamera.Instance.RemoveFromTargetGroup(player.transform);

        if ((highestPlayerCount > 1 && activePlayers.Count == 1) || activePlayers.Count == 0)
        {
            if (gameOngoing)
                GameEnd();
        }
    }

    private void GameStart()
    {
        gameOngoing = true;
        gameTimer = 0;
        UI.ToggleGameEndPanel(false);

        MusicPlaylistManager.Instance.ChangePlaylist("Game");
        SoundEffectManager.instance.PlaySoundEffect("Announcer_BobbleOn");

        if (TimeBeforeFirstPickUp > 0)
        {
            timeTillNextPickup = TimeBeforeFirstPickUp;
        }
        else
        {
            timeTillNextPickup = Random.Range(PickupMinimumTime, PickupMaximumTime);
        }        

        foreach (Player player in joinedPlayers) 
        {
            Destroy(player.gameObject);
        }

        usedSpawnPositions.Clear();

        highestPlayerCount = 0;
        //int playerIndex = 0;

        foreach (PlayerData data in DataManager.Instance.activePlayers)
        {            
            PlayerInputManager.JoinPlayer(data.PlayerIndex, pairWithDevice: InputSystem.devices.First(x => x.deviceId == data.DeviceID));
        }

        //foreach (InputDevice device in InputSystem.devices) 
        //{
        //    if (device.name.Contains("Mouse")) // this is called a lazy hack
        //    {
        //        continue;
        //    }

        //    PlayerInputManager.JoinPlayer(playerIndex++, pairWithDevice: device);
        //    Debug.Log("device name: " + device.description);
        //}

        PlayerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;        
    }

    private void GameEnd()
    {
        gameOngoing = false;

        string winnerText = "";

        if (activePlayers.Count == 1)
        {
            PlayerData data = DataManager.Instance.GetPlayer(activePlayers[0].MyColor);
            winnerText = "Player " + data.PlayerIndex;
            data.PlayerWins++;
            UI.ShowGameEnd(winnerText, activePlayers[0].MyColor.PlayerPortrait, activePlayers[0].MyColor.PlayerColor);
        }
        else
        {
            winnerText = "Survived: " + Mathf.RoundToInt(gameTimer) + "s";
            UI.ShowGameEnd(winnerText, null, Color.black);
        }

        
        PlayerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

        if (Spawner != null)
        {
            Spawner.enabled = false;
        }

        if (AutomaticRestart)
            Invoke(nameof(RestartLevel), 4);
    }

    private void CreatePickUp()
    {
        timeTillNextPickup = Random.Range(PickupMinimumTime, PickupMaximumTime) * PickUpTimerCurve.Evaluate(gameTimer);

        if (MaximumPickUps > 0 && activePickUps.Count + 1 >= MaximumPickUps)
        {
            PlayerPickUpObjectBase pickUpToClear = activePickUps[0];
            pickUpToClear.DestroyPickup();
        }

        List<PickUpSpawnPosition> possibleSpawns = new List<PickUpSpawnPosition>(pickUpSpawns);
        possibleSpawns.RemoveAll(x => x.ActivePickUp != null);

        if (possibleSpawns.Count == 0)
        {
            return;
        }
        
        PickUpSpawnPosition randomSpawnPos = possibleSpawns.GetRandomElementFromList();

        List<PickUpDataObject> pickUpDataObjects = GameData.GetAll<PickUpDataObject>();
        PickUpDataObject randomPickUp = LogicUtils.GetWeighedRandom(pickUpDataObjects, (x) => x.PickupWeight);

        GameObject newPickUp = randomPickUp.CreatePickUp();
        PlayerPickUpObjectBase pickUpScript = newPickUp.GetComponent<PlayerPickUpObjectBase>();
        activePickUps.Add(pickUpScript);
        randomSpawnPos.ActivePickUp = pickUpScript;
        newPickUp.transform.position = randomSpawnPos.transform.position;

        pickUpScript.SetupPickup(randomPickUp);

        CustomCamera.Instance.AddPickupToTargetGroup(newPickUp.transform);
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        Player newPlayer = input.gameObject.GetComponent<Player>();
        
        joinedPlayers.Add(newPlayer);
        highestPlayerCount++;        

        List<Transform> possibleSpawns = new List<Transform>(PlayerSpawnPositions);
        possibleSpawns.RemoveAll(x => usedSpawnPositions.Contains(x));

        if (possibleSpawns.Count == 0)
        {
            usedSpawnPositions.Clear();
            possibleSpawns = new List<Transform>(PlayerSpawnPositions);
        }

        Transform spawnPos = possibleSpawns.GetRandomElementFromList();
        usedSpawnPositions.Add(spawnPos);

        PlayerVisualInfo colorToSet = DataManager.Instance.GetPlayerColor(input.devices[0].deviceId);
        PlayerData player = DataManager.Instance.GetPlayer(colorToSet);

        activePlayers.Add(newPlayer.SpawnPlayerAvatar(spawnPos.transform.position, colorToSet));
        CustomCamera.Instance.AddToTargetGroup(newPlayer.SpawnedAvatar.transform);

        UI.AddNewPlayerUI("Player " + player.PlayerIndex, newPlayer.SpawnedAvatar, colorToSet.PlayerPortrait);
        newPlayer.SpawnedAvatar.PlayerSFXHandler.PlaySoundEffect("Select");
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

}