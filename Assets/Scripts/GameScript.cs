using UnityEngine;
using TMPro;

public class GameScript:MonoBehaviour{
    [SerializeField][Tooltip("The player game object")]private GameObject Player;
    [SerializeField][Tooltip("The points display game object")]private GameObject PointsDisplay;
    [SerializeField][Tooltip("The time display game object")]private GameObject TimeDisplay;
    [SerializeField][Tooltip("The background material")]private Material Background;
    [SerializeField][Tooltip("The background material main X offset multiplier")]private float BackgroundXOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main Y offset multiplier")]private float BackgroundYOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main X offset")]private float BackgroundXOffset=-0.14f;
    [SerializeField][Tooltip("The background material main Y offset")]private float BackgroundYOffset=-0.22f;
    [SerializeField][Tooltip("The starting time for the countdown in seconds")]private float time=60f;

    private TextMeshProUGUI pointsTMPro;
    private TextMeshProUGUI timeTMPro;
    private InputProvider input;
    private int points=0;

    void Start(){
        this.input=this.GetComponent<InputProvider>();
        this.pointsTMPro=this.PointsDisplay.GetComponent<TextMeshProUGUI>();
        this.timeTMPro=this.TimeDisplay.GetComponent<TextMeshProUGUI>();
        this.AddPoints(0);
    }

    public void AddPoints(int morePoints){this.pointsTMPro.text=$"Punkte: "+(this.points+=morePoints);}

    void Update(){
        this.time-=Time.deltaTime;
        this.timeTMPro.text="Zeit:      "+((int)this.time);
        // TODO call just once
        if((int)this.time<=0)this.OnGameOver();
    }
    void LateUpdate(){
        Vector3 ppos=Player.transform.position;
        Background.mainTextureOffset=new Vector2(
            (ppos.x*this.BackgroundXOffsetMultiplier)+this.BackgroundXOffset,
            (ppos.y*this.BackgroundYOffsetMultiplier)+this.BackgroundYOffset
        );
    }

    public void OnGameOver(){
        Time.timeScale=0f;
        // TODO show game over menu
        Debug.Log("show game over menu");
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
}
