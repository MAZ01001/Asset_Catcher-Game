using UnityEngine;

public class BackgroundEffect:MonoBehaviour{
    [SerializeField][Tooltip("The background material")]private Material background;
    [SerializeField][Tooltip("The game script object")]private GameObject gameScriptObject;
    [SerializeField][Tooltip("The background material main X offset multiplier")]private float backgroundXOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main Y offset multiplier")]private float backgroundspawnYOffsetMultiplier=0.015f;
    [SerializeField][Tooltip("The background material main X offset")]private float backgroundXOffset=-0.14f;
    [SerializeField][Tooltip("The background material main Y offset")]private float backgroundspawnYOffset=-0.22f;

    private GameScript gameScript;

    private void Start(){this.gameScript=this.gameScriptObject.GetComponent<GameScript>();}
    private void LateUpdate(){
        Vector3 ppos=this.gameScript.PlayerPos;
        background.mainTextureOffset=new Vector2(
            (ppos.x*this.backgroundXOffsetMultiplier)+this.backgroundXOffset,
            (ppos.y*this.backgroundspawnYOffsetMultiplier)+this.backgroundspawnYOffset
        );
    }
}
