public enum TestResult
{
	NoStartOrEndFound = 0,
	PathIsBlocked = 1,
	CannotReachKey = 2,
	CannotReachTrigger = 3,
	CannotReachKeyOrTrigger = 4,
	KeyToEndBlocked = 5,
	TriggerToEndBlocked = 6,
	ExceptionDuringGeneration = 7
}
