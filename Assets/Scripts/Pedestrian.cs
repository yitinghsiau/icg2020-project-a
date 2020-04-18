using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//script of walking pedestrian
public class Pedestrian : MonoBehaviour
{
    //give the pedestrian an initial speed
    float m_ManVelocity = 1.5f;
    float m_ManMovement;

    //build the color renderer
    SpriteRenderer m_ManRenderer;
    void Start()
    {
        m_ManRenderer = this.GetComponent<SpriteRenderer>();
    }


    //---------pedestrian's movement----------//

    void FixedUpdate()
    {        
        m_ManMovement = m_ManVelocity * Time.fixedDeltaTime;
        

        if (this.transform.position.y >= 10)
        {
            //turn the pedestrian 180 degree and walk back when he walks to the upper limit (Y = 10)
            Vector3 manAngle = new Vector3(0f, 0f, 90f);
            this.transform.localEulerAngles = manAngle;
        }
        else if (this.transform.position.y <= -10)
        {
            //turn the pedestrian 180 degree and walk back when he walks to the lower limit (Y = -10)
            Vector3 manAngle = new Vector3(0f, 0f, -90f);
            this.transform.localEulerAngles = manAngle;
        }

        this.transform.Translate(Vector3.left * m_ManMovement);
    }


    //-----------collision detection--------------//

    void OnCollisionEnter2D(Collision2D collision)
    {
        //change the pedestrian's color to red and speed up 1.5 times when collision initiated
        m_ManRenderer.color = Color.red;
        m_ManVelocity = m_ManVelocity * 1.5f;
        Debug.Log("Are you San Bou??");
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //reset the pedestrian's color to original when collision ended
        m_ManRenderer.color = Color.white;
    }
}

