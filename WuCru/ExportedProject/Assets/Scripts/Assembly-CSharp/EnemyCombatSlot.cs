public class EnemyCombatSlot
{
	public LevelNode levelNode;

	public Character occupiedBy;

	public bool IsOccupied
	{
		get
		{
			return occupiedBy != null;
		}
	}

	public EnemyCombatSlot(LevelNode levelNode)
	{
		this.levelNode = levelNode;
	}
}
