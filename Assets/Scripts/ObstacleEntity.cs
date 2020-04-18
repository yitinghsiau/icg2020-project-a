using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//script of obstacles (roadblocks)
public class ObstacleEntity : MonoBehaviour
{

    //Build the color renderer
    SpriteRenderer m_TargetRenderer;
    void Start()
    {
        m_TargetRenderer = this.GetComponent<SpriteRenderer>();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //change the color of obstacle to  red when collision initiated
        m_TargetRenderer.color = Color.red;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //reset the color of obstacle to original color when collision ended
        m_TargetRenderer.color = Color.white;
    }


}
