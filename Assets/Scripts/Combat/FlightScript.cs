using UnityEngine;
using System.Collections;

public class FlightScript : MonoBehaviour {
	
	public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 100.0f;

    public float acceleration;

    private bool increasingSpeed;
    private bool decreasingSpeed;

    private float maxAcceleration = 10.0f;
    private float minAcceleration = 1.0f;
    private float baseAcceleration = 5.0f;

	// Use this for initialization
	void Start () 
	{
        bool increasingSpeed = false;
        bool decreasingSpeed = false;
        acceleration = baseAcceleration;
	}
	
	// Update is called once per frame
	void Update () 
	{
       if (Input.GetKeyDown("space")) {
           increasingSpeed = true;
       } else if (Input.GetKeyDown("c")) {
           decreasingSpeed = true;
       } 

       if (Input.GetKeyUp("space")) {
           increasingSpeed = false;
       } else if (Input.GetKeyUp("c")) {
           decreasingSpeed = false;
       }
	}
	
	void FixedUpdate()
	{
		UpdateFunction();
	}
	
	void UpdateFunction()
    {

        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
        roll = Input.GetAxis("Roll") * (Time.fixedDeltaTime * RotationSpeed);
        pitch = Input.GetAxis("Pitch") * (Time.fixedDeltaTime * RotationSpeed);
        yaw = Input.GetAxis("Yaw") * (Time.fixedDeltaTime * RotationSpeed);
        AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        GetComponent<Rigidbody>().rotation *= AddRot;
        Vector3 AddPos = Vector3.forward;
        AddPos = GetComponent<Rigidbody>().rotation * AddPos;

        if (increasingSpeed && acceleration <= maxAcceleration) {
            acceleration += .1f;
        } else if (decreasingSpeed && acceleration >= minAcceleration) {
            acceleration -= .1f;
        } else if (acceleration < baseAcceleration) {
            acceleration += .1f;
        } else if (acceleration > baseAcceleration) {
            acceleration -= .1f;
        }
        GetComponent<Rigidbody>().velocity = AddPos * (Time.fixedDeltaTime * AmbientSpeed * acceleration);

    }
}
