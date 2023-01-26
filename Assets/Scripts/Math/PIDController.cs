
using UnityEngine;

public class PIDController
{
	//Our PID coefficients for tuning the controller
	private static float pCoeff = .8f;
    private static float iCoeff = .0002f;
    private static float dCoeff = .2f;
    private static float minimum = -1;
    private static float maximum = 1;

    //Variables to store values between calculations
    private float integral;
    private float lastProportional;

	//We pass in the value we want and the value we currently have, the code
	//returns a number that moves us towards our goal
	public float Seek(float seekValue, float currentValue)
	{
		float deltaTime = Time.fixedDeltaTime;
		float proportional = seekValue - currentValue;

		float derivative = (proportional - lastProportional) / deltaTime;
		integral += proportional * deltaTime;
		lastProportional = proportional;

		//This is the actual PID formula. This gives us the value that is returned
		float value = pCoeff * proportional + iCoeff * integral + dCoeff * derivative;
		value = Mathf.Clamp(value, minimum, maximum);

		return value;
	}
}