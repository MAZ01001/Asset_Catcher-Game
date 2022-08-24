using UnityEngine;

public class PlayerMovements:MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("The ground collision layer")]private LayerMask groundLayer;
    [SerializeField][Tooltip("The primitive trigger layer")]private LayerMask primitiveTriggerLayer;
    [SerializeField][Tooltip("The jump height")]private float jumpHeight=6f;
    [SerializeField][Tooltip("The movement speed")]private float moveSpeed=10f;
    [SerializeField][Tooltip("The movement smoothing value")]private float moveSmooth=0.1f;
    [SerializeField][Tooltip("The movement multiplier for in-air movement")]private float airMoveMultiplier=0.7f;
    //~ private
    private InputProvider input;
    private SphereCollider sc;
    private Rigidbody rb;
    private Vector3 moveDir=Vector3.zero;
    private Vector3 smoothVelocity=Vector3.zero;
    private bool onGround=false;
    private bool jumpToggle=false;
    private GameScript gameScript;
    //~ get components
    void Start(){
        this.input=this.GetComponent<InputProvider>();
        this.sc=this.GetComponent<SphereCollider>();
        this.rb=this.GetComponent<Rigidbody>();
        this.gameScript=Object.FindObjectOfType<GameScript>();
    }
    //~ movement
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
    //~ collision checks
    private bool CheckIsLayerInMask(LayerMask mask,int layer){return(mask&(1<<layer))!=0;}
    //~ collect primitive
    private void OnTriggerEnter(Collider collider){
        if(this.CheckIsLayerInMask(this.primitiveTriggerLayer,collider.gameObject.layer))this.gameScript.ExplodePrimitive(collider.transform.parent.gameObject);
    }
    //~ ground check
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
