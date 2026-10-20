using System;

public static class UnitConversion
{
	private const float kMetre = 1f;

	private const float kKilometre = 1000f;

	private const float kMile = 1609.344f;

	private const float kSecond = 1f;

	private const float kMinute = 60f;

	private const float kHour = 3600f;

	private const float kKilogram = 1f;

	private const float kPi = (float)Math.PI;

	private const float kRadian = 1f;

	private const float kDegree = (float)Math.PI / 180f;

	private const float kRevolution = (float)Math.PI * 2f;

	public const float MilesToMetres = 1609.344f;

	public const float MetresToMiles = 0.0006213712f;

	public const float RadToDeg = 57.29578f;

	public const float DegToRad = (float)Math.PI / 180f;

	public const float MphToMps = 0.44704f;

	public const float MpsToMph = 2.2369363f;

	public const float RpmToRps = (float)Math.PI / 30f;

	public const float RpsToRpm = 30f / (float)Math.PI;
}
