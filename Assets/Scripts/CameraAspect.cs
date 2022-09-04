using UnityEngine;

public class CameraAspect:MonoBehaviour{
    [SerializeField][Tooltip("the horizontal width relative to the vertical height")] private float aspectHorizontal=16f;
    [SerializeField][Tooltip("the vertical height relative to the horizontal width")] private float aspectVertical=9f;

    /// <summary> screen width in px from last update </summary>
    private int lastScreenWidth=0;
    /// <summary> screen height in px from last update </summary>
    private int lastScreenHeight=0;
    /// <summary> the main camera (on [this] game object) </summary>
    private Camera cam;

    /// <summary> get components </summary>
    void Start(){this.cam=this.GetComponent<Camera>();}

    /// <summary>
    ///     ( called every frame ( if Behaviour is enabled ) after all Update functions have been called )
    ///     <br/> show black bars when outside set aspect ratio
    /// </summary>
    private void LateUpdate(){
        if(!(this.lastScreenWidth==Screen.width&&this.lastScreenHeight==Screen.height)){//~ only calculate if screen size is different from last update
            this.lastScreenWidth=Screen.width;
            this.lastScreenHeight=Screen.height;
            float aspectWidthToHeight=(//~ calculate the relation of width and height of the given aspect ratio and the screen size
                (this.aspectVertical*(float)this.lastScreenWidth)/
                (this.aspectHorizontal*(float)this.lastScreenHeight)
            );//~ (ideally 1 | <1 higher than wide | >1 wider than height)
            Rect rect=cam.rect;//~ get render view
            rect.size=Vector2.one;//~ initial size 100% of screen size
            rect.position=Vector2.zero;//~ initial anchor position top left [0,0]
            if(aspectWidthToHeight<1f){//~ if higher than wide
                rect.height=aspectWidthToHeight;//~ set height to % of full screen height
                rect.y=(1f-aspectWidthToHeight)*.5f;//~ center vertically
            }else if(aspectWidthToHeight>1f){//~ if wider than height
                float aspectHeightToWidth=1f/aspectWidthToHeight;//~ invert aspect → (ideally 1 | <1 wider than height | >1 higher than wide)
                rect.width=aspectHeightToWidth;//~ set width to % of full screen width
                rect.x=(1f-aspectHeightToWidth)*.5f;//~ center horizontally
            }
            cam.rect=rect;//~ set render view
        }
    }
}