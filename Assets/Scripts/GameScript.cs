using UnityEngine;

public class GameScript:MonoBehaviour{
    [SerializeField][Tooltip("The Player Game Object")]private GameObject Player;
    [SerializeField][Tooltip("The Background Material")]private Material Background;

    private InputProvider input;

    void Start(){this.input=this.GetComponent<InputProvider>();}

    void LateUpdate(){
        // TODO
        // Player.transform.position
        // Background.mainTextureOffset=new Vector2(0f,0f);
    }

    public void OnPause(){
        Time.timeScale=0f;
        // TODO show pause menu
    }
    public void OnResume(){
        // TODO hide pause menu
        Time.timeScale=1f;
    }
}
