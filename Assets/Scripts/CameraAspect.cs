using UnityEngine;

public class CameraAspect : MonoBehaviour{
    //~ inspector (private)
    [SerializeField][Tooltip("the horizontal width relative to the vertical height")] private float aspectHorizontal = 16f;
    [SerializeField][Tooltip("the vertical height relative to the horizontal width")] private float aspectVertical = 9f;
    //~ private
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;
    //~ unity methods (private)
    private void LateUpdate(){
        //~ show black bars when outside set aspect ratio
        if(
            this.lastScreenWidth != Screen.width
            || this.lastScreenHeight != Screen.height
        ){
            //~ if screen size is different from last update
            this.lastScreenWidth = Screen.width;
            this.lastScreenHeight = Screen.height;
            //~ calculate the relation of width and height of the given aspect ratio and the screen size (ideally 1 | <1 higher than wide | >1 wider than height)
            float aspectWidthToHeight = (
                (this.aspectVertical * (float)this.lastScreenWidth)
                / (this.aspectHorizontal * (float)this.lastScreenHeight)
            );
            //~ get current render view
            Rect rect = Camera.main.rect;
            rect.size = Vector2.one;
            rect.position = Vector2.zero;
            //~ letterbox or pillarbox
            if(aspectWidthToHeight < 1f){
                rect.height = aspectWidthToHeight;
                rect.y = (1f - aspectWidthToHeight) * 0.5f;
            }else if(aspectWidthToHeight > 1f){
                float aspectHeightToWidth = 1f / aspectWidthToHeight;
                rect.width = aspectHeightToWidth;
                rect.x = (1f - aspectHeightToWidth) * 0.5f;
            }
            //~ override with modified render view
            Camera.main.rect = rect;
        }
    }
}
