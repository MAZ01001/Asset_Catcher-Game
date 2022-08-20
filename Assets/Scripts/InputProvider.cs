using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider:MonoBehaviour{
    public float move;
    public bool jumpPressed;

    private GameScript gameScript;

    private void Start(){this.gameScript=FindObjectOfType<GameScript>();}

    private void OnEnable(){
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }
    private void OnDisable(){
        Cursor.lockState=CursorLockMode.None;
        Cursor.visible=true;
    }

    private void OnMove(InputValue value){this.move=value.Get<float>();}
    private void OnJump(InputValue value){this.jumpPressed=value.isPressed;}
    private void OnPause(InputValue value){
        if(this.gameScript.gameOver)return;
        if(value.isPressed){
            if(Time.timeScale>0.001f)this.gameScript.OnPause();
            else this.gameScript.OnResume();
        }
    }
}
