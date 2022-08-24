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
    [SerializeField][Tooltip("The common primitive to spawn")]private GameObject commonPrimitive;
    [SerializeField][Tooltip("The uncommon primitive to spawn")]private GameObject uncommonPrimitive;
    [SerializeField][Tooltip("The rare primitive to spawn")]private GameObject rarePrimitive;
    [SerializeField][Tooltip("The parent game object for the spawning objects this is also used for despawning the primitives outside its collider trigger")]private GameObject spawnParent;
    [SerializeField][Tooltip("The persentage for how common the `uncommonPrimitive` is realtive to the `commonPrimitive`")]private float spawnUncommonRate=0.1f;
    [SerializeField][Tooltip("The persentage for how common the `rarePrimitive` is realtive to the `commonPrimitive`")]private float spawnRareRate=0.02f;
    [SerializeField][Tooltip("The spawn rate per second")]private float spawnRate=0.4f;
    [SerializeField][Tooltip("The countdown time to activate frenzy mode")]private int frenzyTime=10;
    [SerializeField][Tooltip("The spawn rate per second for frenzy mode")]private float spawnRateFrenzy=0.2f;
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
    //~ get components and start timer/spawner
    private void Awake(){Time.timeScale=1f;}
    private void Start(){
        DespawnDetection tmpDD=this.commonPrimitive.GetComponent<DespawnDetection>();
        tmpDD.despawnColliderObject=this.spawnParent;
        tmpDD=this.uncommonPrimitive.GetComponent<DespawnDetection>();
        tmpDD.despawnColliderObject=this.spawnParent;
        tmpDD=this.rarePrimitive.GetComponent<DespawnDetection>();
        tmpDD.despawnColliderObject=this.spawnParent;

        this.input=this.GetComponent<InputProvider>();
        this.pointsTMPro=this.pointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro=this.timeDisplay.GetComponent<TextMeshProUGUI>();
        this.highscoreTMPro=this.highscore.GetComponent<TextMeshProUGUI>();
        this.AddPoints(0);
        this.countdown+=1;
        InvokeRepeating("SpawnPrimitive",1f,this.spawnRate);
        InvokeRepeating("TimerEvents",0f,1f);
    }
    //~ time, display, frenzy mode, and game over
    public void AddPoints(int morePoints){this.pointsTMPro.text=$"Punkte: "+(this.points+=morePoints);}
    public void TimerEvents(){
        this.countdown-=1;
        if(this.countdown<=0)this.OnGameOver();
        else if(this.countdown<=this.frenzyTime){
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
        float common=Random.Range(0f,1f);
        Instantiate<GameObject>(
            (
                common<=this.spawnRareRate?this.rarePrimitive
                :common<=this.spawnUncommonRate?this.uncommonPrimitive
                :this.commonPrimitive
            ),
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
        if(primitive.name==$"{commonPrimitive.name}(Clone)"){
            this.AddPoints(1);
            Destroy(primitive);
        }
        else if(primitive.name==$"{uncommonPrimitive.name}(Clone)"){
            this.AddPoints(4);
            Destroy(primitive);
        }
        else if(primitive.name==$"{rarePrimitive.name}(Clone)"){
            this.AddPoints(8);
            Destroy(primitive);
        }
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
