using UnityEngine;

public class PID: MonoBehaviour
{
	static double previousError;
	static double summaryError;

	public static double Calculate(double P, double I, double D, double currentValue, double targetValue)
	{
		double dt = Time.fixedDeltaTime;
		double currentError = targetValue - currentValue;

		summaryError += currentError;
		double calculatedValue = P * currentError + I * summaryError * dt + D * (currentError - previousError)/ dt;
		previousError = currentError;

		return calculatedValue;
	}
};
