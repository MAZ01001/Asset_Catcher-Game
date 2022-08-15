using UnityEngine;

public class GameScript:MonoBehaviour{
    [SerializeField][Tooltip("The Player Game Object")]private GameObject Player;

    private InputProvider input;

    void Start(){this.input=this.GetComponent<InputProvider>();}

    public void OnPause(){
        Time.timeScale=0f;
        // TODO show pause menu
    }
    public void OnResume(){
        // TODO hide pause menu
        Time.timeScale=1f;
    }
}
