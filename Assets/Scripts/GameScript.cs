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
    [Header("Background effect")]
    [SerializeField][Tooltip("The background material")]private Material background;
    [SerializeField][Tooltip("The background material main X offset multiplier")]private float backgroundXOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main Y offset multiplier")]private float backgroundspawnYOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main X offset")]private float backgroundXOffset=-0.14f;
    [SerializeField][Tooltip("The background material main Y offset")]private float backgroundspawnYOffset=-0.22f;
    [Header("Spawner")]
    [SerializeField][Tooltip("The parent game object for the spawning objects this is also used for despawning the primitives outside its collider trigger")]private GameObject spawnParent;
    [SerializeField][Tooltip("The primitives to spawn (in order of rarity)")]private GameObject[] primitives;
    [SerializeField][Tooltip("The rarities (0.00 to 1.00 (%) in descanding order) for each element (except the first) in the `primitives` array")]private float[] spawnRateRarities;
    [SerializeField][Tooltip("The spawn rate per second")]private float spawnRate=0.4f;
    [SerializeField][Tooltip("The countdown time to activate frenzy mode")]private int frenzyTime=10;
    [SerializeField][Tooltip("The spawn rate per second for frenzy mode")]private float spawnRateFrenzy=0.2f;
    // TODO
    [SerializeField][Tooltip("The max x offset for random spawning")]private float spawnMaxXOffset=7f;
    [SerializeField][Tooltip("The y offset for random spawning")]private float spawnYOffset=13f;
    //~ public
    [HideInInspector]public bool gameOver=false;
    //~ private
    private TextMeshProUGUI pointsTMPro;
    private TextMeshProUGUI timeTMPro;
    private TextMeshProUGUI highscoreTMPro;
    private InputProvider input;
    private int points=0;
    //~ reset time scale on start (before everything else)
    private void Awake(){Time.timeScale=1f;}
    private void Start(){
        //~ sort array to escanding order and resize to primitives array length -1
        System.Array.Sort<float>(this.spawnRateRarities,new System.Comparison<float>((f1,f2)=>f2.CompareTo(f1)));
        System.Array.Resize<float>(ref this.spawnRateRarities,this.primitives.Length-1);
        //~ set the despawn collider for each primitive
        foreach(GameObject primitive in this.primitives)primitive.GetComponent<DespawnDetection>().despawnColliderObject=this.spawnParent;
        //~ get components
        this.input=this.GetComponent<InputProvider>();
        this.pointsTMPro=this.pointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro=this.timeDisplay.GetComponent<TextMeshProUGUI>();
        this.highscoreTMPro=this.highscore.GetComponent<TextMeshProUGUI>();
        //~ reset display and timer
        this.AddPoints(0);
        this.countdown+=1;
        //~ start invoke loops
        InvokeRepeating("SpawnPrimitive",1f,this.spawnRate);
        InvokeRepeating("TimerEvents",0f,1f);
    }
    //~ time, display, frenzy mode, and game over
    public void AddPoints(int morePoints){this.pointsTMPro.text=$"Punkte: "+(this.points+=morePoints);}
    public void TimerEvents(){
        this.countdown-=1;
        if(this.countdown<=0)this.OnGameOver();
        else if(this.countdown<=this.frenzyTime){
            // TODO reference to method if in other script
            CancelInvoke("SpawnPrimitive");
            InvokeRepeating("SpawnPrimitive",0f,this.spawnRateFrenzy);
        }
        this.timeTMPro.text=$"Zeit:      {this.countdown}";
    }
    //~ background effect
    private void LateUpdate(){
        Vector3 ppos=player.transform.position;
        background.mainTextureOffset=new Vector2(
            (ppos.x*this.backgroundXOffsetMultiplier)+this.backgroundXOffset,
            (ppos.y*this.backgroundspawnYOffsetMultiplier)+this.backgroundspawnYOffset
        );
    }
    //~ primitive spawn/explode
    public void SpawnPrimitive(){
        // TODO externealise to another script
        float rarity=Random.Range(0f,1f);
        GameObject prim=null;
        for(int i=this.spawnRateRarities.Length-1;i>=0;i--){
            if(rarity<=this.spawnRateRarities[i]){
                prim=this.primitives[i+1];
                break;
            }
        }
        if(prim==null)prim=this.primitives[0];
        Instantiate<GameObject>(
            prim,
            new Vector3(
                Random.Range(-this.spawnMaxXOffset,this.spawnMaxXOffset),
                this.spawnYOffset,
                0f
            ),
            Random.rotationUniform,
            this.spawnParent.transform
        );
    }
    public void ExplodePrimitive(GameObject primitive){
        // TODO explosions
        this.AddPoints((System.Array.IndexOf<GameObject>(this.primitives,primitive)+1)*4);
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
        CancelInvoke("SpawnPrimitive");
        this.highscoreTMPro.text=$"Highscore\n{this.points}";
        this.gameOverMenu.SetActive(true);
        this.menuBackground.SetActive(true);
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.None;
    }
}
