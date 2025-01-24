
using PlayerController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public GameUI UI;
    public Transform PlayerSpawnPos;
    public GameObject PlayerPrefab;

    [Header("Game Settings")]
    public bool StartGameOnStart = true;
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
    private List<PickUpSpawnPosition> pickUpSpawns = new List<PickUpSpawnPosition>(); 
    private List<PlayerPickUpObjectBase> activePickUps = new List<PlayerPickUpObjectBase>();
    private Player activePlayer;

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
        if (Input.GetKeyDown(KeyCode.F8)) //debug keybinds
        {
            GameEnd();
        }

        //if (false) //check for player count
        //{
        //    GameEnd();
        //}

        if (gameOngoing)
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

    public void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        GameEnd();
    }

    private void GameStart()
    {
        gameOngoing = true;
        gameTimer = 0;
        UI.ToggleGameEndPanel(false);

        if (TimeBeforeFirstPickUp > 0)
        {
            timeTillNextPickup = TimeBeforeFirstPickUp;
        }
        else
        {
            timeTillNextPickup = Random.Range(PickupMinimumTime, PickupMaximumTime);
        }        

        activePlayer = Instantiate(PlayerPrefab).GetComponent<Player>();
    }

    private void GameEnd()
    {
        gameOngoing = false;
        UI.ToggleGameEndPanel(true);
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
    }

}