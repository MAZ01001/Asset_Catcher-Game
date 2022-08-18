using UnityEngine;
using TMPro;

public class GameScript:MonoBehaviour{
    [Header("Player")]
    [SerializeField][Tooltip("The player game object")]private GameObject Player;
    [Header("Display")]
    [SerializeField][Tooltip("The points display game object")]private GameObject PointsDisplay;
    [SerializeField][Tooltip("The time display game object")]private GameObject TimeDisplay;
    [SerializeField][Tooltip("The starting time for the countdown in seconds")]private float countdown=60f;
    [Header("Background effect")]
    [SerializeField][Tooltip("The background material")]private Material Background;
    [SerializeField][Tooltip("The background material main X offset multiplier")]private float BackgroundXOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main Y offset multiplier")]private float BackgroundspawnYOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main X offset")]private float BackgroundXOffset=-0.14f;
    [SerializeField][Tooltip("The background material main Y offset")]private float BackgroundspawnYOffset=-0.22f;
    [Header("Spawner")]
    [SerializeField][Tooltip("The common primitive to spawn")]private GameObject commonPrimitive;
    [SerializeField][Tooltip("The rare primitive to spawn")]private GameObject rarePrimitive;
    [SerializeField][Tooltip("The parent game object for the spawning objects")]private GameObject spawnParent;
    [SerializeField][Tooltip("The persentage for how common the `rarePrimitive` is realtive to the `commonPrimitive`")]private float spawnRareRate=0.1f;
    [SerializeField][Tooltip("The spawn rate per second")]private float spawnRate=0.1f;
    [SerializeField][Tooltip("A random offset for the `spawnRate`")]private float spawnRateOffset=0f;
    [SerializeField][Tooltip("The max x offset for random spawning")]private float spawnMaxXOffset;
    [SerializeField][Tooltip("The y offset for random spawning")]private float spawnYOffset;

    private TextMeshProUGUI pointsTMPro;
    private TextMeshProUGUI timeTMPro;
    private InputProvider input;
    private int points=0;
    private bool gameOver=false;

    void Start(){
        this.input=this.GetComponent<InputProvider>();
        this.pointsTMPro=this.PointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro=this.TimeDisplay.GetComponent<TextMeshProUGUI>();
        this.AddPoints(0);
        this.countdown+=2f;
        InvokeRepeating("SpawnPrimitive",2f,this.spawnRate*Random.Range(1f,1f+this.spawnRateOffset));
    }

    public void AddPoints(int morePoints){this.pointsTMPro.text=$"Punkte: "+(this.points+=morePoints);}

    void Update(){
        if(!this.gameOver){
            this.countdown-=Time.deltaTime;
            this.timeTMPro.text="Zeit:      "+((int)this.countdown);
            if(this.countdown<=0f){
                this.gameOver=true;
                this.OnGameOver();
            }
        }
    }
    void LateUpdate(){
        Vector3 ppos=Player.transform.position;
        Background.mainTextureOffset=new Vector2(
            (ppos.x*this.BackgroundXOffsetMultiplier)+this.BackgroundXOffset,
            (ppos.y*this.BackgroundspawnYOffsetMultiplier)+this.BackgroundspawnYOffset
        );
    }

    public void SpawnPrimitive(){
        Instantiate(
            (Random.Range(0f,1f)<=this.spawnRareRate?this.rarePrimitive:this.commonPrimitive),
            new Vector3(
                Random.Range(-this.spawnMaxXOffset,this.spawnMaxXOffset),
                this.spawnYOffset,
                0f
            ),
            Random.rotationUniform,
            this.spawnParent.transform
        );
    }
    public void OnPause(){
        Time.timeScale=0f;
        // TODO show pause menu
        Debug.Log("show pause menu");
    }
    public void OnResume(){
        // TODO hide pause menu
        Debug.Log("hide pause menu");
        Time.timeScale=1f;
    }
    public void OnGameOver(){
        // TODO frenzy mode
        CancelInvoke("SpawnPrimitive");
        Time.timeScale=0f;
        // TODO show game over menu
        Debug.Log("show game over menu");
    }
}
