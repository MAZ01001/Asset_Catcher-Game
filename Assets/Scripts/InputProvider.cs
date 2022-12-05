using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("The game manager in scene")] private GameObject gameManager;
    //~ public
    public float move{get; private set;}
    public bool jumpPressed{get; private set;}
    //~ private
    private GameScript gameScript;
    //~ unity methods (private)
    private void Start(){
        //~ initiate variables
        this.gameScript = this.gameManager.GetComponent<GameScript>();
    }
    private void OnEnable(){
        //~ initial cursor lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDisable(){
        //~ final cursor unlock
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    //~ input system callbacks (private)
    private void OnMove(InputValue value){
        Vector2 movement = value.Get<Vector2>();
        this.move = movement.x;
        if(movement.y > 0f) this.jumpPressed = true;
        else if(movement.y == 0f) this.jumpPressed = false;
    }
    private void OnJump(InputValue value) => this.jumpPressed = value.isPressed;
    private void OnPause(InputValue value){
        if(this.gameScript.gameOver) return;
        if(value.isPressed){
            if(Time.timeScale > 0.001f) this.gameScript.OnPause();
            else this.gameScript.OnResume();
        }
    }
}
