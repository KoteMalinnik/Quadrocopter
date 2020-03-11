using UnityEngine;

public class PID: MonoBehaviour
{
	double P { get; set; }
	double I { get; set; }
	double D { get; set; }

	double previousError;
	double summaryError;

	public PID(double P, double I, double D)
	{
		this.P = P;
		this.I = I;
		this.D = D;
	}

	public double Calculate(double currentValue, double targetValue)
	{
		double dt = Time.fixedDeltaTime;
		double currentError = targetValue - currentValue;

		summaryError += currentError;
		double calculatedValue = P * currentError + I * summaryError * dt + D * (currentError - previousError)/ dt;
		previousError = currentError;

		return calculatedValue;
	}
};
