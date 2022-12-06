using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour{
    //~ inspector (private)
    [Header("Player")]
    [SerializeField][Tooltip("The player game object")]                                 private GameObject player;
    [Header("Display")]
    [SerializeField][Tooltip("The points display game object")]                         private GameObject pointsDisplay;
    [SerializeField][Tooltip("The time display game object")]                           private GameObject timeDisplay;
    [SerializeField][Min(0)][Tooltip("The starting time for the countdown in seconds")] private int countdown = 60;
    [SerializeField][Tooltip("The menu background ui element")]                         private GameObject menuBackground;
    [SerializeField][Tooltip("The pause menu ui element")]                              private GameObject pauseMenu;
    [SerializeField][Tooltip("The game over menu ui element")]                          private GameObject gameOverMenu;
    [SerializeField][Tooltip("The highscore ui element")]                               private GameObject highscore;
    [Header("Spawner")]
    [SerializeField][Tooltip("The spawner that also has the spawner script")]           private GameObject spawner;
    [SerializeField][Min(0f)][Tooltip("The spawn rate per second")]                     private float spawnRate = 0.4f;
    [SerializeField][Min(1f)][Tooltip("The countdown time to activate frenzy mode")]    private int frenzyTime = 10;
    [SerializeField][Min(0f)][Tooltip("The spawn rate per second for frenzy mode")]     private float spawnRateFrenzy = 0.2f;
    //~ public
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public Vector3 PlayerPos => this.player.transform.position;
    //~ private
    private PrimitiveSpawner primitiveSpawner;
    private TextMeshProUGUI pointsTMPro;
    private TextMeshProUGUI timeTMPro;
    private TextMeshProUGUI highscoreTMPro;
    private InputProvider input;
    private int points = 0;
    //~ unity methods (private)
    private void Awake(){
        //~ reset time scale on start (before everything else)
        Time.timeScale = 1f;
    }
    private void Start(){
        //~ get components
        this.primitiveSpawner = this.spawner.GetComponent<PrimitiveSpawner>();
        this.input = this.GetComponent<InputProvider>();
        this.pointsTMPro = this.pointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro = this.timeDisplay.GetComponent<TextMeshProUGUI>();
        this.highscoreTMPro = this.highscore.GetComponent<TextMeshProUGUI>();
        //~ reset display and timer
        this.AddPoints(0);
        this.countdown++;
        //~ start invoke loops
        this.primitiveSpawner.StartSpawner(this.spawnRate, 1f);
        InvokeRepeating("TimerEvents", 0f, 1f);
    }
    //~ public methods
    /// <summary> Add points when collecting a primitive </summary>
    public void PrimitiveCollision(GameObject primitive) => this.AddPoints(this.primitiveSpawner.CollectAndGetPoints(primitive));
    /// <summary> Increase <see cref="points"/> by <paramref name="morePoints"/> and update the points ui </summary>
    /// <param name="morePoints"> The number of points to add </param>
    public void AddPoints(int morePoints) => this.pointsTMPro.text = $"Points: {(this.points += morePoints)}";
    /// <summary>
    ///     Handles the timer events like
    ///     <br/>countdown and update timer ui,
    ///     <br/>activate frenzy mode,
    ///     <br/>and call <see cref="OnGameOver"> when timer reaches 0
    /// </summary>
    public void TimerEvents(){
        this.countdown--;
        if(this.countdown <= 0) this.OnGameOver();
        else if(this.countdown <= this.frenzyTime) this.primitiveSpawner.ChangeRate(this.spawnRateFrenzy);
        this.timeTMPro.text = $"Time:   {this.countdown}";
    }
    /// <summary> Pauses the game and activates the pause menu </summary>
    public void OnPause(){
        Time.timeScale = 0f;
        this.pauseMenu.SetActive(true);
        this.menuBackground.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    /// <summary> Resumes the game and closes the pause menu </summary>
    public void OnResume(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        this.pauseMenu.SetActive(false);
        this.menuBackground.SetActive(false);
        Time.timeScale = 1f;
    }
    /// <summary> Stops the game, disables timer events and spawner, update highscore ui, and activates the game over menu </summary>
    public void OnGameOver(){
        this.gameOver = true;
        Time.timeScale = 0f;
        CancelInvoke("TimerEvents");
        this.primitiveSpawner.StopSpawner();
        this.highscoreTMPro.text = $"Highscore\n{this.points}";
        this.gameOverMenu.SetActive(true);
        this.menuBackground.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
