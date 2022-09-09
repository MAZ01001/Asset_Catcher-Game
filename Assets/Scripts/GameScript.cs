using UnityEngine;
using TMPro;

public class GameScript:MonoBehaviour{
    //~ inspector (private)
    [Header("Player")]
    [SerializeField][Tooltip("The player game object")]private GameObject player;
    [Header("Display")]
    [SerializeField][Tooltip("The points display game object")]private GameObject pointsDisplay;
    [SerializeField][Tooltip("The time display game object")]private GameObject timeDisplay;
    [SerializeField][Tooltip("The starting time for the countdown in seconds")]private int countdown=60;
    [SerializeField][Tooltip("The menu background ui element")]private GameObject menuBackground;
    [SerializeField][Tooltip("The pause menu ui element")]private GameObject pauseMenu;
    [SerializeField][Tooltip("The game over menu ui element")]private GameObject gameOverMenu;
    [SerializeField][Tooltip("The highscore ui element")]private GameObject highscore;
    [Header("Spawner")]
    [SerializeField][Tooltip("The spawner that also has the spawner script")]private GameObject spawner;
    [SerializeField][Tooltip("The spawn rate per second")]private float spawnRate=0.4f;
    [SerializeField][Tooltip("The countdown time to activate frenzy mode")]private int frenzyTime=10;
    [SerializeField][Tooltip("The spawn rate per second for frenzy mode")]private float spawnRateFrenzy=0.2f;
    //~ public
    [HideInInspector]public bool gameOver=false;
    [HideInInspector]public Vector3 PlayerPos{get=>this.player.transform.position;}
    //~ private
    private PrimitiveSpawner primitiveSpawner;
    private TextMeshProUGUI pointsTMPro;
    private TextMeshProUGUI timeTMPro;
    private TextMeshProUGUI highscoreTMPro;
    private InputProvider input;
    private int points=0;
    //~ reset time scale on start (before everything else)
    private void Awake(){Time.timeScale=1f;}
    private void Start(){
        //~ get components
        this.primitiveSpawner=this.spawner.GetComponent<PrimitiveSpawner>();
        this.input=this.GetComponent<InputProvider>();
        this.pointsTMPro=this.pointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro=this.timeDisplay.GetComponent<TextMeshProUGUI>();
        this.highscoreTMPro=this.highscore.GetComponent<TextMeshProUGUI>();
        //~ reset display and timer
        this.AddPoints(0);
        this.countdown+=1;
        //~ start invoke loops
        this.primitiveSpawner.StartSpawner(this.spawnRate,1f);
        InvokeRepeating("TimerEvents",0f,1f);
    }
    //~ time, display, frenzy mode, and game over
    public void AddPoints(int morePoints){this.pointsTMPro.text=$"Punkte: "+(this.points+=morePoints);}
    public void TimerEvents(){
        this.countdown-=1;
        if(this.countdown<=0)this.OnGameOver();
        else if(this.countdown<=this.frenzyTime)this.primitiveSpawner.ChangeRate(this.spawnRateFrenzy);
        this.timeTMPro.text=$"Zeit:      {this.countdown}";
    }
    //~ primitive explode effect
    public void ExplodePrimitive(GameObject primitive){
        // TODO explosions - in PrimitiveSpawner ?
        // FIXME no index → -1
        this.AddPoints((this.primitiveSpawner.GetIndexOf(primitive)*2)+1);
        Destroy(primitive);
    }
    //~ menus
    public void OnPause(){
        Time.timeScale=0f;
        this.pauseMenu.SetActive(true);
        this.menuBackground.SetActive(true);
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.None;
    }
    public void OnResume(){
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
        this.pauseMenu.SetActive(false);
        this.menuBackground.SetActive(false);
        Time.timeScale=1f;
    }
    public void OnGameOver(){
        this.gameOver=true;
        Time.timeScale=0f;
        CancelInvoke("TimerEvents");
        this.primitiveSpawner.StopSpawner();
        this.highscoreTMPro.text=$"Highscore\n{this.points}";
        this.gameOverMenu.SetActive(true);
        this.menuBackground.SetActive(true);
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.None;
    }
}
