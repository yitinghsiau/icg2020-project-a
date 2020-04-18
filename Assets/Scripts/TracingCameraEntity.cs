using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//script of tracing camera
public class TracingCameraEntity : MonoBehaviour
{
    //assign target object for tracing camera
    public CarEntity targetObject;

    //assign the value for tracing delay
    float MOVING_THRESHOLD = 3f;

    Camera m_Camera;
    float m_OrthographicSize;


    void Start()
    {
        //zoom to fit the velocity of the target object when initialized
        m_Camera = this.GetComponent<Camera>();
        m_OrthographicSize = m_Camera.orthographicSize;
    }


    void LateUpdate()
    {
        //zoom to fit according to the velocity of the target object 
        Vector2 deltaPos = this.transform.position - targetObject.transform.position;
        m_Camera.orthographicSize = m_OrthographicSize + targetObject.m_Velocity * 0.2f;



        //target object tracing
        if (deltaPos.magnitude > MOVING_THRESHOLD)
        {
            deltaPos.Normalize();

            Vector2 newPosition = new Vector2(targetObject.transform.position.x, targetObject.transform.position.y)
                + deltaPos * MOVING_THRESHOLD;
            this.transform.position = new Vector3(newPosition.x, newPosition.y, this.transform.position.z);
        }
    }
}
