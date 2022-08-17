using UnityEngine;

public class PlayerMovements:MonoBehaviour{
    [SerializeField][Tooltip("The Ground Collision Layer")]private LayerMask groundLayer;
    [SerializeField][Tooltip("The Jump Height")]private float jumpHeight=6f;
    [SerializeField][Tooltip("The Movement Speed")]private float moveSpeed=10f;
    [SerializeField][Tooltip("The Movement Smoothing Value")]private float moveSmooth=0.1f;
    [SerializeField][Tooltip("The Movement Multiplier for in-air movement")]private float airMoveMultiplier=0.7f;

    private InputProvider input;
    private SphereCollider sc;
    private Rigidbody rb;
    private Vector3 moveDir=Vector3.zero;
    private Vector3 smoothVelocity=Vector3.zero;
    private bool onGround=false;
    private bool jumpToggle=false;

    void Start(){
        this.input=this.GetComponent<InputProvider>();
        this.sc=this.GetComponent<SphereCollider>();
        this.rb=this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate(){
        //~ move
        if(
            this.input.move>0.05f
            ||this.input.move<-0.05f
        ){
            this.moveDir=(Vector3.right*this.input.move).normalized*this.moveSpeed;
            if(!this.onGround)this.moveDir*=this.airMoveMultiplier;//~ slower movement in air
            this.rb.velocity=Vector3.SmoothDamp(
                this.rb.velocity,
                new Vector3(
                    (this.rb.velocity.x+this.moveDir.x)*0.5f,
                    this.rb.velocity.y,//~ preserve jump and gravity
                    0f
                ),
                ref this.smoothVelocity,
                this.moveSmooth
            );
        }

        //~ jump
        if(this.onGround){
            if(
                this.input.jumpPressed
                &&!this.jumpToggle
            ){
                this.rb.AddForce(Physics.gravity*(this.jumpHeight*-10f),ForceMode.Force);
                this.jumpToggle=true;
            }else if(
                !this.input.jumpPressed
                &&this.jumpToggle
            )this.jumpToggle=false;
        }
    }

    //~ ground check
    private bool CheckIsLayerInMask(LayerMask mask,int layer){return mask==(mask|(1<<layer));}
    private void OnCollisionEnter(Collision collision){
        if(
            !this.onGround
            &&this.CheckIsLayerInMask(this.groundLayer,collision.gameObject.layer)
        )this.onGround=true;
    }
    private void OnCollisionExit(Collision collision){
        if(
            this.onGround
            &&this.CheckIsLayerInMask(this.groundLayer,collision.gameObject.layer)
        )this.onGround=false;
    }

}
