using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//script of moving car
public class MovingCar : MonoBehaviour
{
    //give the moving car an initial speed
    float m_MovingCarVelocity = 5.0f;
    float m_MovingCarMovement;

    //build the color renderer
    [SerializeField] SpriteRenderer[] m_MovingCarRenderers = new SpriteRenderer[5];


    //---------moving car's movement----------//

    void FixedUpdate()
    {
        m_MovingCarMovement = m_MovingCarVelocity * Time.fixedDeltaTime;

        
        if (this.transform.position.y >= 10 && this.transform.position.x >= 3)
        {
            //turn the moving car 90 degree leftward when it moves to the upper right limit ([X,Y] = [3,10])
            Vector3 manAngle = new Vector3(0f, 0f, 180f);
            this.transform.localEulerAngles = manAngle;

        }
        else if (this.transform.position.y >= 10 && this.transform.position.x <= -15)
        {
            //turn the moving car 90 degree leftward when it moves to the upper left limit ([X,Y] = [-15,10])
            Vector3 manAngle = new Vector3(0f, 0f, -90f);
            this.transform.localEulerAngles = manAngle;

        }
        else if (this.transform.position.y <= -8 && this.transform.position.x <= -15)
        {
            //turn the moving car 90 degree leftward when it moves to the lower left limit ([X,Y] = [-15,-8])
            Vector3 manAngle = new Vector3(0f, 0f, 0f);
            this.transform.localEulerAngles = manAngle;

        }
        else if (this.transform.position.y <= -8 && this.transform.position.x >= 3)
        {
            //turn the moving car 90 degree leftward when it moves to the lower right limit ([X,Y] = [3,-8])
            Vector3 manAngle = new Vector3(0f, 0f, 90f);
            this.transform.localEulerAngles = manAngle;

        }
        
              
        this.transform.Translate(Vector3.right * m_MovingCarMovement);
    }



    //-----------collision detection--------------//

    void OnCollisionEnter2D(Collision2D collision)
    {
        //change the moving car's color to red and speed up 1.25 times when collision initiated
        ChangeColor(Color.red);
        m_MovingCarVelocity = m_MovingCarVelocity * 1.25f;
        Debug.Log("Are you San Bou??");
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //reset the moving car's color to original when collision ended
        ResetColor();
    }



    //-----reset color-----//

    void ResetColor()
    {
        ChangeColor(Color.green);
    }


    //-----change color-------//

    void ChangeColor(Color color)
    {
        foreach (SpriteRenderer r in m_MovingCarRenderers)
        {
            r.color = color;
        }
    }
}
