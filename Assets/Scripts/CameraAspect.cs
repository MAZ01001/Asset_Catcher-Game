using UnityEngine;

public class CameraAspect:MonoBehaviour{
    /// <summary> the horizontal width relative to the vertical height </summary>
    [SerializeField][Tooltip("the horizontal width relative to the vertical height")] private float aspectHorizontal;
    /// <summary> the horizontal width relative to the vertical height </summary>
    [SerializeField][Tooltip("the horizontal width relative to the vertical height")] private float aspectVertical;

    /// <summary> screen width in px from last update </summary>
    private int lastScreenWidth=0;
    /// <summary> screen height in px from last update </summary>
    private int lastScreenHeight=0;

    /// <summary>
    ///     ( called every frame ( if Behaviour is enabled ) after all Update functions have been called )
    ///     <br/> for rendering black bars if camera aspect is nt supported
    /// </summary>
    private void LateUpdate(){
        if(!(this.lastScreenWidth==Screen.width&&this.lastScreenHeight==Screen.height)){//~ only if screen size is different from last update
            this.lastScreenWidth=Screen.width;
            this.lastScreenHeight=Screen.height;
            // TODO division to Start()
            //~ black bars if camera aspect is not supported
            float targetaspect=this.aspectHorizontal/this.aspectVertical;
            float windowaspect=(float)this.lastScreenWidth/(float)this.lastScreenHeight;
            float scaleheight=windowaspect/targetaspect;
            Camera camera=this.GetComponent<Camera>();
            if(scaleheight<1f){//~ if scaled height is less than current height, add letterbox
                Rect rect=camera.rect;
                rect.width=1f;
                rect.height=scaleheight;
                rect.x=0;
                rect.y=(1f-scaleheight)/2f;
                camera.rect=rect;
            }else{//~ else add pillarbox
                float scalewidth=1f/scaleheight;
                Rect rect=camera.rect;
                rect.width=scalewidth;
                rect.height=1f;
                rect.x=(1f-scalewidth)/2f;
                rect.y=0;
                camera.rect=rect;
            }
        }
    }
}