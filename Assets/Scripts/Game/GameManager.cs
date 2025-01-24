using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public GameUI UI;

    [Header("Game Settings")]
    public bool StartGameOnStart = true;

    public bool GameGoing => gameOngoing;
    public float RoundTimer => gameTimer;

    private float gameTimer = 0;
    private bool gameOngoing = false;

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

        if (gameOngoing)
        {
            gameTimer += Time.deltaTime;
        }
    }

    private void Initialize()
    {
        if (StartGameOnStart) 
        { 
            GameStart();
        }
    }

    private void GameStart()
    {
        gameOngoing = true;
        gameTimer = 0;
        UI.ToggleGameEndPanel(false);
    }

    private void GameEnd()
    {
        gameOngoing = false;
        UI.ToggleGameEndPanel(true);
    }

}