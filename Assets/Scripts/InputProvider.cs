using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour{
    //~ public
    public float move{get; private set;}
    public bool jumpPressed{get; private set;}
    //~ private
    private GameManager gameManager;
    //~ unity methods (private)
    private void Start(){
        //~ initiate variables
        this.gameManager = Object.FindObjectOfType<GameManager>();
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
        if(this.gameManager.gameOver) return;
        if(value.isPressed){
            if(Time.timeScale > 0.001f) this.gameManager.OnPause();
            else this.gameManager.OnResume();
        }
    }
}
