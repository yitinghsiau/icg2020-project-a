using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script of player's car
public class CarEntity : MonoBehaviour
{
    //Assign 4 wheel objects
    public GameObject wheelFrontRight;
    public GameObject wheelFrontLeft;
    public GameObject wheelBackRight;
    public GameObject wheelBackLeft;


    //Assign reverse radar objects
    public GameObject carRadarLeft;
    public GameObject carRadarRight;

    //Assign 4 outer wall objects
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallTop;
    public GameObject wallBottom;
    


    //car steering parameters
    float m_FrontWheelAngle = 0;              //initiate wheels' angular position
    const float WHEEL_Angle_LIMIT = 40f;      //Set wheels' angular position limit
    public float turnAngularVelocity = 20f;   //Set the angular velocity of wheel turning


    //accelerate and decellerate
    public float m_Velocity;

    public float acceleration = 3f;
    public float deceleration = 10f;
    public float maxVelocity = 20f;
    public float minVelocity = -15f;

    float m_DeltaMovement;

    //Assign car length according to game object (for the calculation of turning radius)
    float carLength = 1.02f;

    //Build the renderer for car and radar
    [SerializeField] SpriteRenderer[] m_Renderers = new SpriteRenderer[5];
    [SerializeField] SpriteRenderer[] m_radarRenderers = new SpriteRenderer[2];

    

    //Assign ID number for each parallel parking space
    int parallel_parking_space;

    //Assign bool parameters for status check
    bool entranceCheck;                  //"True" when the car fully enters the parking space (leaving the "outer sensor")
    bool stayCheck;                      //"Ture" when the car lies on the "inner seneor" inside the parkng space
    bool collisionCheck;                 //"True" when the collision initiates (in order to turn off the reverse radar when collision)
    bool stopCheck = false;              //"Ture" when the car collide and "stay" on objects, and reset to "False" when collision ends
    bool forwardstopCheck = false;       //"True" when front side of the car collides with objects, and reset to "False" when collision ends
    bool backwardstopCheck = false;      //"True" when rear side of the car collides with objects, and reset to "False" when collision ends


    void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        //acceleration
        if (Input.GetKey(KeyCode.UpArrow) && m_Velocity >= -0.01)
        {
            //Locks key input when front side of the car collides with objects
            if(forwardstopCheck == false)
            {
                m_Velocity = Mathf.Min(maxVelocity, m_Velocity + Time.fixedDeltaTime * acceleration);
            }          
                        
        }

        //deceleration
        if (Input.GetKey(KeyCode.DownArrow) && m_Velocity <= 0.01)
        {
            //Locks key input when rear side of the car collides with objects
            if (backwardstopCheck == false)
            {
                m_Velocity = Mathf.Max(-maxVelocity, m_Velocity - Time.fixedDeltaTime * acceleration);
            }                      
            
        }

        //brake
        if (Input.GetKey(KeyCode.Space))
        {
            if (m_Velocity > 0)
            {
                m_Velocity = Mathf.Max(0, m_Velocity - Time.fixedDeltaTime * deceleration);
            }
            else if (m_Velocity < 0)
            {
                m_Velocity = Mathf.Min(-m_Velocity, m_Velocity + Time.fixedDeltaTime * deceleration);
            }

        }

        //Steering Control
        //turn left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_FrontWheelAngle = Mathf.Clamp(m_FrontWheelAngle + Time.fixedDeltaTime * turnAngularVelocity, -WHEEL_Angle_LIMIT, WHEEL_Angle_LIMIT);
            UpdateWheels();
        }
        //turn right
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_FrontWheelAngle = Mathf.Clamp(m_FrontWheelAngle - Time.fixedDeltaTime * turnAngularVelocity, -WHEEL_Angle_LIMIT, WHEEL_Angle_LIMIT);
            UpdateWheels();
        }
        //reset the wheel angle when speed > 0.5f
        else
        {
            if (m_FrontWheelAngle > 0 && m_Velocity > 0.5f)
            {
                m_FrontWheelAngle = Mathf.Clamp(m_FrontWheelAngle - Time.fixedDeltaTime * 1f * turnAngularVelocity, 0, WHEEL_Angle_LIMIT);
                UpdateWheels();
            }
            else if (m_FrontWheelAngle < 0 && m_Velocity > 0.5f)
            {
                m_FrontWheelAngle = Mathf.Clamp(m_FrontWheelAngle + Time.fixedDeltaTime * 1f * turnAngularVelocity, -WHEEL_Angle_LIMIT, 0);
                UpdateWheels();
            }
        }

        //Force to reset the wheel angle
        if (Input.GetKey(KeyCode.S))
        {
            m_FrontWheelAngle = 0;
            UpdateWheels();
        }


        //Emergency Stop
        if (Input.GetKey(KeyCode.Z))
        {
            Stop();
        }


        //Collision Sensor

        //left reverse radar
        //detect the position relative to the walls
        float radarLeft_residualLeft = (this.carRadarLeft.transform.position.x) - (wallLeft.transform.position.x);
        float radarLeft_residualRight = (wallRight.transform.position.x) - (this.carRadarLeft.transform.position.x);
        float radarLeft_residualTop = (wallTop.transform.position.y) - (this.carRadarLeft.transform.position.y);
        float radarLeft_residualBottom = (this.carRadarLeft.transform.position.y) - (wallBottom.transform.position.y);

        //return value of the min. distance
        float radarLeft_distance = Mathf.Min(radarLeft_residualLeft, radarLeft_residualRight, radarLeft_residualTop, radarLeft_residualBottom);

        //change the color of radar base on the returned value (activates only when the car is reversing)
        if (stayCheck == false && collisionCheck == false && m_Velocity < -0.01f)
        {
            if (radarLeft_distance <= 1 && radarLeft_distance > 0.5)
            {
                //change color to green
                Color alarmColor = new Color(0.37f, 0.75f, 0.25f, 1f);
                m_radarRenderers[0].color = alarmColor;
            }
            else if (radarLeft_distance <= 0.5 && radarLeft_distance > 0.2)
            {
                //change color to orange
                Color alarmColor = new Color(0.75f, 0.50f, 0.25f, 1f);
                m_radarRenderers[0].color = alarmColor;
            }
            else if (radarLeft_distance <= 0.2)
            {
                //change color to red
                Color alarmColor = new Color(0.85f, 0.25f, 0.25f, 1f);
                m_radarRenderers[0].color = alarmColor;
            }
            else if (radarLeft_distance > 1)
            {
                //turn off the radar when distance > 1 (color = transparent)
                Color alarmColor = new Color(1f, 1f, 1f, 0f);
                m_radarRenderers[0].color = alarmColor;
            }
        }
        else
        {
            //turn off the radar when collision happened or parking completed (color = transparent)
            Color alarmColor = new Color(1f, 1f, 1f, 0f);
            m_radarRenderers[0].color = alarmColor;
        }

        //right reverse radar
        //detect the position relative to the walls
        float radarRight_residualLeft = (this.carRadarRight.transform.position.x) - (wallLeft.transform.position.x);
        float radarRight_residualRight = (wallRight.transform.position.x) - (this.carRadarRight.transform.position.x);
        float radarRight_residualTop = (wallTop.transform.position.y) - (this.carRadarRight.transform.position.y);
        float radarRight_residualBottom = (this.carRadarRight.transform.position.y) - (wallBottom.transform.position.y);
        //return the min. distance
        float radarRight_distance = Mathf.Min(radarRight_residualLeft, radarRight_residualRight, radarRight_residualTop, radarRight_residualBottom);

        //change the color of radar base on the returned value (activates only when the car is reversing)
        if (stayCheck == false && collisionCheck == false && m_Velocity < -0.01f)
        {
            if (radarRight_distance <= 1 && radarRight_distance > 0.5)
            {
                //change color to green
                Color alarmColor = new Color(0.37f, 0.75f, 0.25f, 1f);
                m_radarRenderers[1].color = alarmColor;
            }
            else if (radarRight_distance <= 0.5 && radarRight_distance > 0.2)
            {
                //change color to orange
                Color alarmColor = new Color(0.75f, 0.50f, 0.25f, 1f);
                m_radarRenderers[1].color = alarmColor;
            }
            else if (radarRight_distance <= 0.2)
            {
                //change color to red
                Color alarmColor = new Color(0.85f, 0.25f, 0.25f, 1f);
                m_radarRenderers[1].color = alarmColor;
            }
            else if (radarRight_distance > 1)
            {
                //turn off the radar when distance > 1 (color = transparent)
                Color alarmColor = new Color(1f, 1f, 1f, 0f);
                m_radarRenderers[1].color = alarmColor;
            }
        }
        else
        {
            //turn off the radar when collision happened or parking completed (color = transparent)
            Color alarmColor = new Color(1f, 1f, 1f, 0f);
            m_radarRenderers[1].color = alarmColor;
        }


    }


    void UpdateWheels()
    {
        //update wheel by m_FrontWheelAngle
        Vector3 localEulerAngles = new Vector3(0f, 0f, m_FrontWheelAngle);
        wheelFrontLeft.transform.localEulerAngles = localEulerAngles;
        wheelFrontRight.transform.localEulerAngles = localEulerAngles;

    }

    void Update()
    {
        m_DeltaMovement = m_Velocity * Time.fixedDeltaTime;

        //update car transform
        this.transform.Rotate(0f, 0f, (1 / carLength) * Mathf.Tan(Mathf.Deg2Rad * m_FrontWheelAngle) * m_DeltaMovement * Mathf.Rad2Deg);
        this.transform.Translate(Vector3.right * m_DeltaMovement);
    }

    


    void ResetColor()
    {
        ChangeColor(Color.white);
    }

    void ChangeColor(Color color)
    {
        foreach(SpriteRenderer r in m_Renderers)
        {
            r.color = color;
        }
    }


    void ResetRadarColor()
    {
        Color alarmColor = new Color(1f, 1f, 1f, 0f);
        foreach (SpriteRenderer r in m_radarRenderers)
        {
            r.color = alarmColor;
        }
    }

    void Stop()
    {
        m_Velocity = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //change color of the car to red and stop when collision initiated
        Stop();
        Debug.Log("OnCollisionEnter2D");
        ChangeColor(Color.red);
        collisionCheck = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(stopCheck == false)
        {
            //check which side of the car is collided with objects
            if (forwardstopCheck == false && m_Velocity >= 0.01f)
            {
                Stop();
                forwardstopCheck = true;
                Debug.Log("OnCollisionStay2D");
            }
            else if (backwardstopCheck == false && m_Velocity <= -0.01f)
            {
                Stop();
                backwardstopCheck = true;
                Debug.Log("OnCollisionStay2D");
            }

            stopCheck = true;
        }  
    }



    void OnCollisionExit2D(Collision2D collision)
    {
        //Reset parameters when collision ends
        ResetColor();
        Debug.Log("OnCollisionExit2D");
        collisionCheck = false;
        stopCheck = false;
        forwardstopCheck = false;
        backwardstopCheck = false;
    }




    void OnTriggerExit2D(Collider2D other)
    {
        Sensor sensor = other.gameObject.GetComponent<Sensor>();        
        string checked_identifier = sensor.identifier_outer;

        //set entranceCheck = true when the car fully passes the "outer sensor"
        if (entranceCheck == false)
        {
            switch (checked_identifier)
            {
                case "reverse_easy":
                    entranceCheck = true;
                    break;
                case "reverse_normal":
                    entranceCheck = true;
                    break;
                case "reverse_hard":
                    entranceCheck = true;
                    break;
                case "parallel_easy":
                    entranceCheck = true;
                    parallel_parking_space = 1;
                    break;
                case "parallel_normal":
                    entranceCheck = true;
                    parallel_parking_space = 2;
                    break;
                case "parallel_hard":
                    entranceCheck = true;
                    parallel_parking_space = 3;
                    break;
            }
        }

        stayCheck = false;
    }




    void OnTriggerStay2D(Collider2D other)
    {
        Sensor sensor = other.gameObject.GetComponent<Sensor>();

        //check if the car has passed the "outer sensor" (fully enter the parking space)
        if (entranceCheck == true)
        {
            string checked_identifier = sensor.identifier_inner;

            //Detect which parking space the car enters and change the color to green
            switch (checked_identifier)
            {
                case "reverse_easy":
                    ChangeColor(Color.green);
                    this.Invoke("ResetColor", 2f);
                    ResetRadarColor();
                    stayCheck = true;
                    Debug.Log("Reverse Parking Easy is DONE!");
                    break;
                case "reverse_normal":
                    ChangeColor(Color.green);
                    this.Invoke("ResetColor", 2f);
                    ResetRadarColor();
                    stayCheck = true;
                    Debug.Log("Reverse Parking Normal is DONE!");
                    break;
                case "reverse_hard":
                    ChangeColor(Color.green);
                    this.Invoke("ResetColor", 2f);
                    ResetRadarColor();
                    stayCheck = true;
                    Debug.Log("Reverse Parking Hard is DONE!");
                    break;
                case "parallel_easy":
                    if(parallel_parking_space == 1)
                    {
                        ChangeColor(Color.green);
                        this.Invoke("ResetColor", 2f);
                        ResetRadarColor();
                        stayCheck = true;
                        Debug.Log("Parallel Parking Easy is DONE!");
                    }                    
                    break;
                case "parallel_normal":
                    if(parallel_parking_space == 2)
                    {
                        ChangeColor(Color.green);
                        this.Invoke("ResetColor", 2f);
                        ResetRadarColor();
                        stayCheck = true;
                        Debug.Log("Parallel Parking Normal is DONE!");
                    }
                    break;
                case "parallel_hard":
                    if (parallel_parking_space == 3)
                    {
                        ChangeColor(Color.green);
                        this.Invoke("ResetColor", 2f);
                        ResetRadarColor();
                        stayCheck = true;
                        Debug.Log("Parallel Parking Hard is DONE!");
                    }
                    break;
            }

            entranceCheck = false;

            
        }
    }
}
