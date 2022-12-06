using UnityEngine;

public class BackgroundEffect : MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("The background material")]                                   private Material background;
    [SerializeField][Min(0f)][Tooltip("The background material main X offset multiplier")] private float backgroundXOffsetMultiplier = 0.015f;
    [SerializeField][Min(0f)][Tooltip("The background material main Y offset multiplier")] private float backgroundspawnYOffsetMultiplier = 0.015f;
    [SerializeField][Tooltip("The background material main X offset")]                     private float backgroundXOffset = -0.14f;
    [SerializeField][Tooltip("The background material main Y offset")]                     private float backgroundspawnYOffset = -0.22f;
    //~ private
    private GameManager gameManager;
    //~ unity methods (private)
    private void Start(){
        //~ initiate variables
        this.gameManager = Object.FindObjectOfType<GameManager>();
    }
    private void LateUpdate(){
        //~ set background circle to look at player
        Vector3 ppos = this.gameManager.PlayerPos;
        background.mainTextureOffset = new Vector2(
            (ppos.x * this.backgroundXOffsetMultiplier) + this.backgroundXOffset,
            (ppos.y * this.backgroundspawnYOffsetMultiplier) + this.backgroundspawnYOffset
        );
    }
}
