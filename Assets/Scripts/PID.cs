using UnityEngine;

public class PID: MonoBehaviour
{
	double currentError;
	double previousError;
	double summaryError;

	public double Calculate(double P, double I, double D, double currentValue, double targetValue, bool stabilizationON)
	{
		double dt = Time.fixedDeltaTime;

		currentError = targetValue - currentValue;
		summaryError += currentError;

		double calculatedValue = P * currentError + I * summaryError * dt + D * (currentError - previousError) / dt;
		previousError = currentError;

		return calculatedValue;
	}
};
