using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
	private LevelNode playerCombatPos;

	private List<EnemyCombatSlot> enemyCombatSlots = new List<EnemyCombatSlot>();

	private List<EnemyCombatSlot> enemySpectatorSlots = new List<EnemyCombatSlot>();

	private AIAgent currentAttacker;

	private float duellingStatus = 0.5f;

	private CombatResult prevResult;

	private float damageMultiplier = 1f;

	public int consecutiveHits = 1;

	public int consecutiveEnemyAttackOptions;

	public float combatStartTime;

	public float combatTimeout = 5f;

	private CombatStage stage;

	private AIAgent secondaryAttacker;

	private float secondaryAttackTime;

	private float secondaryAttackGraceDuration = 1f;

	private Character playerCharacter;

	private AIAgent _knockDownAttacker;

	private float _knockdownTimer;

	public LevelNode PlayerCombatSlot
	{
		get
		{
			return playerCombatPos;
		}
	}

	public float DuellingStatus
	{
		get
		{
			return duellingStatus;
		}
		set
		{
			duellingStatus = Mathf.Clamp01(value);
		}
	}

	public float TimeSinceLastSecondaryAttack
	{
		get
		{
			return Time.time - secondaryAttackTime;
		}
	}

	public EnemyCombatSlot[] EnemyCombatSlots
	{
		get
		{
			return enemyCombatSlots.ToArray();
		}
	}

	public EnemyCombatSlot[] EnemySpectatorSlots
	{
		get
		{
			return enemySpectatorSlots.ToArray();
		}
	}

	public float SecondaryAttackGraceDuration
	{
		get
		{
			return secondaryAttackGraceDuration;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier;
		}
	}

	public int NumEnemies
	{
		get
		{
			int num = 0;
			foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
			{
				if (enemyCombatSlot.IsOccupied)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int NumSpectators
	{
		get
		{
			int num = 0;
			foreach (EnemyCombatSlot enemySpectatorSlot in enemySpectatorSlots)
			{
				if (enemySpectatorSlot.IsOccupied)
				{
					num++;
				}
			}
			return num;
		}
	}

	public AIAgent CurrentAttacker
	{
		get
		{
			return currentAttacker;
		}
		set
		{
			if (currentAttacker != null && value != null)
			{
				Debug.LogError("Did have CurrentAttacker before - Previous attacker in UNKNOWN STATE! FIX!");
			}
			if (value != null)
			{
				bool flag = false;
				foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
				{
					if (enemyCombatSlot.occupiedBy != null && enemyCombatSlot.occupiedBy == value.Character)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.LogError("Setting CurrentAttacker with enemy that HAS NO ENEMY-COMBAT-SLOT!!! Bailing out");
					return;
				}
			}
			currentAttacker = value;
			duellingStatus = 0.5f;
			if (currentAttacker != null)
			{
				StartCoroutine(InitiatePrimaryAttack());
			}
		}
	}

	public AIAgent SecondaryAttacker
	{
		get
		{
			return secondaryAttacker;
		}
		set
		{
			if (secondaryAttacker != null)
			{
				HUDItemTracker hUDItemTracker = ScreenHud._pInstance._enemyIconTracker;
				if (GameController.Instance.Seed >= 5000)
				{
					hUDItemTracker = ScreenHud._pInstance._enemyIconTrackerCity;
				}
				hUDItemTracker.DismissIcon(secondaryAttacker.gameObject.transform);
				ScreenHud._pInstance._enemyIconSeenTracker.DismissIcon(secondaryAttacker.Character.gameObject.transform);
				ScreenHud._pInstance._enemyIconSuspiciousTracker.DismissIcon(secondaryAttacker.Character.gameObject.transform);
			}
			secondaryAttacker = value;
			if (secondaryAttacker != null)
			{
				secondaryAttackTime = Time.time;
				StartCoroutine(InitiateSecondaryAttack());
			}
		}
	}

	public EnemyCombatSlot GetCombatSlotByAgent(AIAgent agent)
	{
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.occupiedBy == agent.Character)
			{
				return enemyCombatSlot;
			}
		}
		return null;
	}

	private void Update()
	{
		if (playerCharacter == null && GamePlayer.Instance.Player != null && GamePlayer.Instance.Player.Character != null)
		{
			playerCharacter = GamePlayer.Instance.Player.Character;
		}
		if (secondaryAttacker != null)
		{
			secondaryAttackTime = Time.time;
		}
	}

	public bool CanUseAsCombatOrSpectatorSlot(LevelNode node, LevelDirection dir)
	{
		if (node != null && node.Elevation == playerCombatPos.Elevation && node.GetNodeEdgeValue(LevelHelper.OppositeDirection(dir)) == LevelNodeEdgeValue.Empty && playerCombatPos.GetNodeEdgeValue(dir) == LevelNodeEdgeValue.Empty)
		{
			if (node.ObjectTypes.Count == 0)
			{
				return true;
			}
			if (node.ContainsObjectType(LevelNodeObjectType.Key) || node.ContainsObjectType(LevelNodeObjectType.Studs) || node.ContainsObjectType(LevelNodeObjectType.StartFlag) || node.ContainsObjectType(LevelNodeObjectType.ConstrictaiBossEnemy) || node.ContainsObjectType(LevelNodeObjectType.FangpyreBossEnemy) || node.ContainsObjectType(LevelNodeObjectType.GruntEnemy) || node.ContainsObjectType(LevelNodeObjectType.HypnobraiBossEnemy) || node.ContainsObjectType(LevelNodeObjectType.ScoutEnemy) || node.ContainsObjectType(LevelNodeObjectType.SnakeBossEnemy) || node.ContainsObjectType(LevelNodeObjectType.VenomariBossEnemy) || node.ContainsObjectType(LevelNodeObjectType.WarriorEnemy))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanGetCombatSlots()
	{
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			playerCombatPos = GetPlayerCombatSlot();
			if (playerCombatPos == null)
			{
				return false;
			}
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(playerCombatPos.Row, playerCombatPos.Col), levelDirection, GamePlayer.Instance.Level.Nodes);
			if (CanUseAsCombatOrSpectatorSlot(neighbourAt, levelDirection))
			{
				return true;
			}
		}
		return false;
	}

	private LevelNode GetPlayerCombatSlot()
	{
		if (LevelNodeBlockedForPlayerCombat(playerCharacter.CurrentPositionNode))
		{
			for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
			{
				LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(playerCharacter.CurrentPositionNode.Row, playerCharacter.CurrentPositionNode.Col), levelDirection, GamePlayer.Instance.Level.Nodes);
				if (!LevelNodeBlockedForPlayerCombat(neighbourAt) && playerCharacter.CurrentPositionNode.GetNodeEdgeValue(levelDirection) == LevelNodeEdgeValue.Empty)
				{
					return neighbourAt;
				}
			}
			return null;
		}
		return playerCharacter.CurrentPositionNode;
	}

	private bool LevelNodeBlockedForPlayerCombat(LevelNode node)
	{
		if (node.ContainsObjectType(LevelNodeObjectType.BellTower) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_earth) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_electricity) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_energy) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_fire) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_ice) || node.ContainsObjectType(LevelNodeObjectType.CrystalShard_water) || node.ContainsObjectType(LevelNodeObjectType.LampPost) || node.ContainsObjectType(LevelNodeObjectType.Tree))
		{
			return true;
		}
		return false;
	}

	public void InitializeCombat()
	{
		combatStartTime = Time.time;
		playerCombatPos = GetPlayerCombatSlot();
		damageMultiplier = 1f;
		secondaryAttackTime = Time.time;
		for (LevelDirection levelDirection = LevelDirection.North; levelDirection <= LevelDirection.West; levelDirection++)
		{
			LevelNode neighbourAt = LevelHelper.GetNeighbourAt(new LevelPoint(playerCombatPos.Row, playerCombatPos.Col), levelDirection, GamePlayer.Instance.Level.Nodes);
			if (CanUseAsCombatOrSpectatorSlot(neighbourAt, levelDirection))
			{
				enemyCombatSlots.Add(new EnemyCombatSlot(neighbourAt));
			}
		}
		for (int i = -1; i <= 1; i += 2)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				if (LevelHelper.PointInsideBounds(new LevelPoint(playerCombatPos.Row + i, playerCombatPos.Col + j), GamePlayer.Instance.Level.Nodes) && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + i, playerCombatPos.Col + j].Elevation == playerCombatPos.Elevation && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + i, playerCombatPos.Col + j].ObjectTypes.Count == 0)
				{
					enemySpectatorSlots.Add(new EnemyCombatSlot(GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + i, playerCombatPos.Col + j]));
				}
			}
		}
		for (int k = -2; k <= 2; k += 4)
		{
			if (LevelHelper.PointInsideBounds(new LevelPoint(playerCombatPos.Row + k, playerCombatPos.Col), GamePlayer.Instance.Level.Nodes) && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + k, playerCombatPos.Col].Elevation == playerCombatPos.Elevation && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + k, playerCombatPos.Col].ObjectTypes.Count == 0)
			{
				enemySpectatorSlots.Add(new EnemyCombatSlot(GamePlayer.Instance.Level.Nodes[playerCombatPos.Row + k, playerCombatPos.Col]));
			}
		}
		for (int l = -2; l <= 2; l += 4)
		{
			if (LevelHelper.PointInsideBounds(new LevelPoint(playerCombatPos.Row, playerCombatPos.Col + l), GamePlayer.Instance.Level.Nodes) && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row, playerCombatPos.Col + l].Elevation == playerCombatPos.Elevation && GamePlayer.Instance.Level.Nodes[playerCombatPos.Row, playerCombatPos.Col + l].ObjectTypes.Count == 0)
			{
				enemySpectatorSlots.Add(new EnemyCombatSlot(GamePlayer.Instance.Level.Nodes[playerCombatPos.Row, playerCombatPos.Col + l]));
			}
		}
		currentAttacker = null;
		playerCharacter.Animator.SetTrigger(TriggerHash.combatPrepare);
		playerCharacter.ResetPath();
		playerCharacter.PlannedPath.Clear();
		stage = CombatStage.Preparing;
	}

	public void UpdateCombat()
	{
		if (currentAttacker == null && secondaryAttacker == null && NumEnemies == 0 && NumSpectators == 0 && playerCharacter.CurrentStateNameHash() == StateHash.idle)
		{
			Debug.Log("Set Exploring");
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
			Reset();
			return;
		}
		if (currentAttacker != null && currentAttacker.Character.IsDead())
		{
			ReleaseEnemyCombatSlot(currentAttacker.Character);
		}
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.IsOccupied && enemyCombatSlot.occupiedBy.IsDead())
			{
				ReleaseEnemyCombatSlot(enemyCombatSlot.occupiedBy);
			}
		}
		if (playerCharacter.IsDead())
		{
			foreach (EnemyCombatSlot enemyCombatSlot2 in enemyCombatSlots)
			{
				if (enemyCombatSlot2.IsOccupied)
				{
					enemyCombatSlot2.occupiedBy.Animator.SetTrigger(TriggerHash.reset);
					ReleaseEnemyCombatSlot(enemyCombatSlot2.levelNode);
				}
			}
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
			playerCharacter.ShowCombatEventType(CombatSpecialEventType.CombatLost);
			Reset();
		}
		if (playerCharacter.IsInSpinjitzu())
		{
			if (CurrentAttacker != null)
			{
				CurrentAttacker.Character.TakeDamage(100, playerCharacter.transform.position, DamageType.Spinjitzu);
			}
			foreach (EnemyCombatSlot enemyCombatSlot3 in enemyCombatSlots)
			{
				if (enemyCombatSlot3.IsOccupied)
				{
					enemyCombatSlot3.occupiedBy.Animator.SetTrigger(TriggerHash.combatEnd);
					(enemyCombatSlot3.occupiedBy.Controller as AIController).agent.SetState(AIAgentState.Scared);
					ReleaseEnemyCombatSlot(enemyCombatSlot3.levelNode);
				}
			}
			foreach (EnemyCombatSlot enemySpectatorSlot in enemySpectatorSlots)
			{
				if (enemySpectatorSlot.IsOccupied)
				{
					enemySpectatorSlot.occupiedBy.Animator.SetTrigger(TriggerHash.combatEnd);
					(enemySpectatorSlot.occupiedBy.Controller as AIController).agent.SetState(AIAgentState.Scared);
					ReleaseEnemySpectatorSlot(enemySpectatorSlot.levelNode);
				}
			}
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
			Reset();
			return;
		}
		switch (stage)
		{
		case CombatStage.Preparing:
			UpdatePreparing();
			break;
		case CombatStage.CombatReady:
			UpdateCombatReady();
			break;
		case CombatStage.Duelling:
			UpdateDuelling();
			break;
		case CombatStage.EnemyAttackOption:
			UpdateEnemyAttackOption();
			break;
		case CombatStage.PlayerAttackOption:
			UpdatePlayerAttackOption();
			break;
		case CombatStage.SecondaryAttack:
			UpdateSecondaryAttack();
			break;
		case CombatStage.Knockdown:
			UpdateKnockdown();
			break;
		}
		if (NumEnemies == 0 && NumSpectators == 0 && stage != CombatStage.Knockdown)
		{
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
			playerCharacter.Animator.SetTrigger(TriggerHash.combatEnd);
			if (!playerCharacter.IsDead())
			{
				playerCharacter.ShowCombatEventType(CombatSpecialEventType.CombatWon);
			}
		}
	}

	private void UpdatePreparing()
	{
		if (IsCombatReady())
		{
			stage = CombatStage.CombatReady;
		}
		if (playerCharacter.CurrentStateNameHash() != StateHash.preparingCombat && playerCharacter.CurrentStateNameHash() != StateHash.combatReady && !playerCharacter.Animator.IsInTransition(0))
		{
			playerCharacter.Animator.SetTrigger(TriggerHash.combatPrepare);
		}
	}

	private void UpdateCombatReady()
	{
		bool flag = false;
		if (IsDuelling())
		{
			stage = CombatStage.Duelling;
		}
		else if (CurrentAttacker != null)
		{
			if (CurrentAttacker.Character.CurrentStateNameHash() == StateHash.combatReady && playerCharacter.CurrentStateNameHash() == StateHash.combatReady)
			{
				flag = true;
			}
		}
		else
		{
			AIAgent aIAgent = SelectNewCurrentAttacker();
			if (aIAgent != null)
			{
				CurrentAttacker = aIAgent;
				flag = true;
			}
		}
		if (flag)
		{
			bool flag2 = false;
			if (CurrentAttacker.Character.IsElementalBoss())
			{
				Debug.Log(string.Concat("Combat with an elemental boss ", CurrentAttacker.Character.elementalPower, "vs ", playerCharacter.elementalPower));
				if (CurrentAttacker.Character.elementalPower != playerCharacter.elementalPower)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.enemyAttack);
				playerCharacter.Animator.SetTrigger(TriggerHash.enemyAttack);
				duellingStatus = 0.5f;
				stage = CombatStage.Duelling;
				return;
			}
			CurrentAttacker.Character.transform.LookAt(playerCharacter.transform);
			playerCharacter.transform.LookAt(CurrentAttacker.Character.transform);
			ElementalBossEnemy elementalBossEnemy = (ElementalBossEnemy)CurrentAttacker;
			elementalBossEnemy.Character.ResetPath();
			elementalBossEnemy.Character.PlannedPath.Clear();
			playerCharacter.Animator.SetTrigger(TriggerHash.knockDown);
			playerCharacter.ResetPath();
			playerCharacter.PlannedPath.Clear();
			CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.evilLaughStart);
			CurrentAttacker.SetState(AIAgentState.IdleAfterKnockdown);
			_knockDownAttacker = CurrentAttacker;
			ReleaseEnemyCombatSlot(CurrentAttacker.Character);
			CurrentAttacker = null;
			_knockdownTimer = 1f;
			stage = CombatStage.Knockdown;
		}
		else if (NumEnemies == 0 && NumSpectators == 0)
		{
			playerCharacter.Animator.SetTrigger(TriggerHash.combatEnd);
			GamePlayer.Instance.SetState(GamePlayerState.Exploring);
		}
	}

	private void UpdateKnockdown()
	{
		if (_knockdownTimer > 0f)
		{
			_knockdownTimer -= Time.deltaTime;
			return;
		}
		Debug.Log("Knockback -> Preparing");
		InitializeCombat();
	}

	private void UpdateDuelling()
	{
		if (IsSecondaryAttack())
		{
			stage = CombatStage.SecondaryAttack;
		}
		if (!(playerCharacter != null))
		{
			return;
		}
		if (CurrentAttacker == null || CurrentAttacker.Character == null)
		{
			if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.Duelling))
			{
				GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.Duelling);
			}
			playerCharacter.Animator.SetTrigger(TriggerHash.duellingWon);
			stage = CombatStage.PlayerAttackOption;
		}
		else
		{
			if (CurrentAttacker.Character.Animator.IsInTransition(0) || playerCharacter.Animator.IsInTransition(0))
			{
				return;
			}
			if (duellingStatus <= 0f)
			{
				if (GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.Duelling))
				{
					CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.duellingWon);
					playerCharacter.Animator.SetTrigger(TriggerHash.duellingLost);
					stage = CombatStage.EnemyAttackOption;
				}
				else
				{
					duellingStatus = 0.5f;
					Time.timeScale = 0f;
				}
			}
			if (duellingStatus >= 1f)
			{
				if (!GameController.Instance.PlayerProfile.HasSeenBriefing(PlayerBriefingType.Duelling))
				{
					GameController.Instance.PlayerProfile.BriefingComplete(PlayerBriefingType.Duelling);
				}
				CurrentAttacker.Character.Animator.SetTrigger(TriggerHash.duellingLost);
				playerCharacter.Animator.SetTrigger(TriggerHash.duellingWon);
				stage = CombatStage.PlayerAttackOption;
			}
		}
	}

	private void UpdateEnemyAttackOption()
	{
		if (IsCombatReady())
		{
			stage = CombatStage.CombatReady;
		}
	}

	private void UpdatePlayerAttackOption()
	{
		if (IsCombatReady())
		{
			stage = CombatStage.CombatReady;
		}
	}

	private void UpdateSecondaryAttack()
	{
		if (SecondaryAttacker == null)
		{
			stage = CombatStage.CombatReady;
		}
	}

	private AIAgent SelectNewCurrentAttacker()
	{
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.occupiedBy != null && enemyCombatSlot.occupiedBy.CurrentStateNameHash() == StateHash.combatReady)
			{
				return (enemyCombatSlot.occupiedBy.Controller as AIController).agent;
			}
		}
		return null;
	}

	private bool IsCombatReady()
	{
		bool flag = false;
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.occupiedBy != null && enemyCombatSlot.occupiedBy.CurrentStateNameHash() == StateHash.combatReady)
			{
				flag = true;
				break;
			}
		}
		if (playerCharacter.CurrentStateNameHash() == StateHash.combatReady && flag)
		{
			return true;
		}
		return false;
	}

	private bool IsDuelling()
	{
		if (CurrentAttacker == null)
		{
			return false;
		}
		if (CurrentAttacker.Character.CurrentStateNameHash() == StateHash.duelling && playerCharacter.CurrentStateNameHash() == StateHash.duelling)
		{
			return true;
		}
		return false;
	}

	private bool IsEnemyAttackOption()
	{
		if (CurrentAttacker == null)
		{
			return false;
		}
		if (CurrentAttacker.Character.CurrentStateNameHash() == StateHash.enemyAttackOption && playerCharacter.CurrentStateNameHash() == StateHash.enemyAttackOption)
		{
			return true;
		}
		return false;
	}

	private bool IsPlayerAttackOption()
	{
		if (CurrentAttacker == null)
		{
			return false;
		}
		if (CurrentAttacker.Character.CurrentStateNameHash() == StateHash.playerAttackOption && playerCharacter.CurrentStateNameHash() == StateHash.playerAttackOption)
		{
			return true;
		}
		return false;
	}

	private bool IsSecondaryAttack()
	{
		if (SecondaryAttacker == null)
		{
			return false;
		}
		if (SecondaryAttacker is ElementalBossEnemy)
		{
			return false;
		}
		return true;
	}

	private IEnumerator InitiatePrimaryAttack()
	{
		playerCharacter.transform.LookAt(currentAttacker.Character.transform.position);
		if (playerCharacter.CurrentStateNameHash() != StateHash.preparingCombat && playerCharacter.CurrentStateNameHash() != StateHash.combatReady)
		{
			playerCharacter.Animator.SetTrigger(TriggerHash.combatPrepare);
		}
		while (playerCharacter.CurrentStateNameHash() != StateHash.combatReady && currentAttacker != null)
		{
			yield return 0;
		}
		if (currentAttacker != null)
		{
			currentAttacker.Character.Animator.SetTrigger(TriggerHash.enemyAttack);
			playerCharacter.Animator.SetTrigger(TriggerHash.enemyAttack);
		}
	}

	private IEnumerator InitiateSecondaryAttack()
	{
		secondaryAttacker.Character.Animator.SetTrigger(TriggerHash.initiateSecondaryAttack);
		yield return 0;
	}

	public LevelNode RequestEnemyCombatSlot(Character enemy)
	{
		float num = float.MaxValue;
		EnemyCombatSlot enemyCombatSlot = null;
		foreach (EnemyCombatSlot enemyCombatSlot2 in enemyCombatSlots)
		{
			float num2 = LevelHelper.Distance(new LevelPoint(enemyCombatSlot2.levelNode.Row, enemyCombatSlot2.levelNode.Col), new LevelPoint(enemy.CurrentPositionNode.Row, enemy.CurrentPositionNode.Col));
			if (!enemyCombatSlot2.IsOccupied && num2 < num && enemy.DoesPathExist(enemyCombatSlot2.levelNode))
			{
				num = num2;
				enemyCombatSlot = enemyCombatSlot2;
			}
		}
		if (enemyCombatSlot == null)
		{
			return null;
		}
		ReleaseEnemySpectatorSlot(enemy);
		enemyCombatSlot.occupiedBy = enemy;
		return enemyCombatSlot.levelNode;
	}

	public LevelNode RequestEnemySpectatorSlot(Character enemy)
	{
		ReleaseEnemySpectatorSlot(enemy);
		float num = float.MaxValue;
		EnemyCombatSlot enemyCombatSlot = null;
		foreach (EnemyCombatSlot enemySpectatorSlot in enemySpectatorSlots)
		{
			float num2 = LevelHelper.Distance(new LevelPoint(enemySpectatorSlot.levelNode.Row, enemySpectatorSlot.levelNode.Col), new LevelPoint(enemy.CurrentPositionNode.Row, enemy.CurrentPositionNode.Col));
			if (!enemySpectatorSlot.IsOccupied && num2 < num && enemy.DoesPathExist(enemySpectatorSlot.levelNode))
			{
				num = num2;
				enemyCombatSlot = enemySpectatorSlot;
			}
		}
		if (enemyCombatSlot == null)
		{
			return null;
		}
		enemyCombatSlot.occupiedBy = enemy;
		return enemyCombatSlot.levelNode;
	}

	public void ReleaseEnemyCombatSlot(LevelNode node)
	{
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.levelNode == node)
			{
				if (CurrentAttacker != null && enemyCombatSlot.occupiedBy == CurrentAttacker.Character)
				{
					CurrentAttacker = null;
				}
				enemyCombatSlot.occupiedBy = null;
				break;
			}
		}
	}

	public void ReleaseEnemyCombatSlot(Character enemy)
	{
		foreach (EnemyCombatSlot enemyCombatSlot in enemyCombatSlots)
		{
			if (enemyCombatSlot.occupiedBy == enemy)
			{
				enemyCombatSlot.occupiedBy = null;
				break;
			}
		}
		if (CurrentAttacker == (enemy.Controller as AIController).agent)
		{
			CurrentAttacker = null;
		}
	}

	public void ReleaseEnemySpectatorSlot(LevelNode node)
	{
		foreach (EnemyCombatSlot enemySpectatorSlot in enemySpectatorSlots)
		{
			if (enemySpectatorSlot.levelNode == node)
			{
				enemySpectatorSlot.occupiedBy = null;
				break;
			}
		}
	}

	public void ReleaseEnemySpectatorSlot(Character enemy)
	{
		foreach (EnemyCombatSlot enemySpectatorSlot in enemySpectatorSlots)
		{
			if (enemySpectatorSlot.occupiedBy == enemy)
			{
				enemySpectatorSlot.occupiedBy = null;
				return;
			}
		}
		Debug.LogWarning("Could not find spectator slot to release!");
	}

	public int GetEnemyDamageAmount(DamageType type)
	{
		int num = 10;
		float num2 = 20f;
		float num3 = 80f;
		float num4 = (float)Mathf.Clamp(GameController.Instance.PlayerProfile.Level, 0, num) / (float)num;
		int num5 = Mathf.RoundToInt(num2 + (num3 - num2) * num4);
		num5 += Mathf.RoundToInt(Random.Range((float)(-num5) * 0.1f, (float)num5 * 0.1f));
		if (type == DamageType.SecondaryAttack)
		{
			return Mathf.RoundToInt((float)num5 * 0.5f);
		}
		return num5;
	}

	public int GetPlayerDamageAmount(DamageType type)
	{
		int num = 10;
		float num2 = 80f;
		float num3 = 160f;
		float num4 = (float)Mathf.Clamp(GameController.Instance.PlayerProfile.Level, 0, num) / (float)num * GameController.Instance.PlayerProfile.DamageMultiplier;
		int num5 = Mathf.RoundToInt(num2 + (num3 - num2) * num4);
		num5 += Mathf.RoundToInt(Random.Range((float)(-num5) * 0.1f, (float)num5 * 0.1f));
		int num6 = Mathf.RoundToInt(damageMultiplier * (float)num5 * GamePlayer.Instance.Player.Character.GetWeapon().GetHitPointFactor());
		switch (type)
		{
		case DamageType.PerfectAttack:
			return num6 * 2;
		case DamageType.SneakAttack:
			return num6 * 4;
		default:
			return num6;
		}
	}

	public void Reset()
	{
		CurrentAttacker = null;
		playerCombatPos = null;
		enemyCombatSlots.Clear();
		enemySpectatorSlots.Clear();
		damageMultiplier = 1f;
		consecutiveHits = 0;
		consecutiveEnemyAttackOptions = 0;
	}

	public void ReportResult(CombatResult result)
	{
		if (result == CombatResult.PlayerAttackSuccess)
		{
			consecutiveEnemyAttackOptions = 0;
			if (prevResult == CombatResult.PlayerAttackSuccess)
			{
				consecutiveHits++;
				damageMultiplier += 0.25f;
				damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, 2f);
			}
			else
			{
				consecutiveHits = 1;
				damageMultiplier = 1f;
			}
		}
		prevResult = result;
	}

	public void SetWeaponTrailColor(Character character, MeleeWeaponTrail trail, DamageType type)
	{
		if (character.isPlayer)
		{
			int num = GamePlayer.Instance.CombatManager.consecutiveHits;
			float num2 = num;
			float num3 = 0.4f;
			float num4 = 0.2f;
			if (type == DamageType.PerfectAttack)
			{
				num3 = 1f;
				num4 = 0.4f;
			}
			else
			{
				num3 = num2 / 5f + 0.2f;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num3 < 0.2f)
				{
					num3 = 0.2f;
				}
				num4 = num3 / 2f;
			}
			if (type == DamageType.SneakAttack)
			{
				num3 = 0.5f;
			}
			if (character.elementalPower == ElementalPower.Earth)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[1];
				trail._colors[1] = character.elementalPowerSecondaryColors[1];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
			else if (character.elementalPower == ElementalPower.Fire)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[2];
				trail._colors[1] = character.elementalPowerSecondaryColors[2];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
			else if (character.elementalPower == ElementalPower.Water)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[3];
				trail._colors[1] = character.elementalPowerSecondaryColors[3];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
			else if (character.elementalPower == ElementalPower.Electricity)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[4];
				trail._colors[1] = character.elementalPowerSecondaryColors[4];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
			else if (character.elementalPower == ElementalPower.Ice)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[5];
				trail._colors[1] = character.elementalPowerSecondaryColors[5];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
			else if (character.elementalPower == ElementalPower.Energy)
			{
				trail._colors[0] = character.elementalPowerPrimaryColors[6];
				trail._colors[1] = character.elementalPowerSecondaryColors[6];
				trail._colors[0].a = num3;
				trail._colors[1].a = num4;
			}
		}
		else
		{
			AIController aIController = character.Controller as AIController;
			AIAgentType agentType = aIController.agent.GetAgentType();
			float num5 = 0.3f;
			switch (agentType)
			{
			case AIAgentType.StoneWarrior:
				num5 = 0.5f;
				trail._colors[0] = Color.red;
				trail._colors[1] = Color.red;
				trail._colors[0].a = num5;
				trail._colors[1].a = num5 / 2f;
				break;
			case AIAgentType.ScoutEnemy:
				trail._colors[0] = Color.red;
				trail._colors[1] = Color.red;
				trail._colors[0].a = num5;
				trail._colors[1].a = num5 / 2f;
				break;
			case AIAgentType.GruntEnemy:
				trail._colors[0] = character.elementalPowerPrimaryColors[0];
				trail._colors[1] = character.elementalPowerSecondaryColors[0];
				trail._colors[0].a = num5;
				trail._colors[1].a = num5 / 2f;
				break;
			case AIAgentType.WarriorEnemy:
				num5 = 0.5f;
				trail._colors[0] = Color.yellow;
				trail._colors[1] = Color.yellow;
				trail._colors[0].a = num5;
				trail._colors[1].a = num5 / 2f;
				break;
			case AIAgentType.SnakeBossEnemy:
				num5 = 0.5f;
				trail._colors[0] = Color.red;
				trail._colors[1] = Color.red;
				trail._colors[0].a = num5;
				trail._colors[1].a = num5 / 2f;
				break;
			}
		}
	}
}
