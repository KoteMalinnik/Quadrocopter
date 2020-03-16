using UnityEngine;

public class PID: MonoBehaviour
{
	float currentError;
	float previousError;
	float summaryError;

	public float Calculate(float P, float I, float D, float currentValue, double targetValue, bool stabilizationON = true)
	{
		float dt = Time.fixedDeltaTime;

		currentError = (float)targetValue - currentValue;
		summaryError += currentError;

		float calculatedValue = P * currentError + I * summaryError * dt + D * (currentError - previousError) / dt;
		previousError = currentError;

		return calculatedValue;
	}
};
