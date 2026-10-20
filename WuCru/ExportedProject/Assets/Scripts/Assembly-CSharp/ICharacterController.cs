public interface ICharacterController
{
	Character CurrentEnemy { get; }

	void ToggleHide(bool hidden);

	void ToggleTrapped(bool trapped);

	void WalkTo(LevelNode target, bool force = false);

	bool CanSeePlayer();
}
