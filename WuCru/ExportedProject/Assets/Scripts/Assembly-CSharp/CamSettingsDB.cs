using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CamSettingsDB : ScriptableObject
{
	[SerializeField]
	public DictionaryCamStateToCamSettings stateSettings = new DictionaryCamStateToCamSettings();

	[SerializeField]
	public List<Vector3> combatPositionOffsets = new List<Vector3>();

	public List<Vector3> explorationPositionOffsets = new List<Vector3>();

	public void SetupDefaults()
	{
		CamSettings camSettings = new CamSettings();
		camSettings.positionOffset = new Vector3(-3.5f, 2f, 0f);
		camSettings.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings.dist = 5f;
		camSettings.interpolationSpeed = 2f;
		camSettings.transitionOutTime = 1f;
		stateSettings.Add(InGameCamState.Starting, camSettings);
		CamSettings camSettings2 = new CamSettings();
		camSettings2.positionOffset = new Vector3(-3.5f, 6f, 0f);
		camSettings2.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings2.dist = 14f;
		camSettings2.interpolationSpeed = 2f;
		camSettings2.transitionOutTime = 0.3f;
		stateSettings.Add(InGameCamState.Exploring, camSettings2);
		CamSettings camSettings3 = new CamSettings();
		camSettings3.positionOffset = new Vector3(-2.5f, 5f, 0f);
		camSettings3.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings3.dist = 5.2f;
		camSettings3.interpolationSpeed = 3f;
		camSettings3.transitionOutTime = 0.5f;
		stateSettings.Add(InGameCamState.Combat, camSettings3);
		CamSettings camSettings4 = new CamSettings();
		camSettings4.positionOffset = new Vector3(-6.5f, 8f, 0f);
		camSettings4.lookAtOffset = new Vector3(0f, 1f, 0f);
		camSettings4.dist = 15f;
		camSettings4.interpolationSpeed = 0.5f;
		camSettings4.transitionOutTime = 0.8f;
		stateSettings.Add(InGameCamState.Stealth, camSettings4);
		CamSettings camSettings5 = new CamSettings();
		camSettings5.positionOffset = new Vector3(-1.4f, 8.2f, 0f);
		camSettings5.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings5.dist = 5f;
		camSettings5.interpolationSpeed = 2f;
		camSettings5.transitionOutTime = 1f;
		stateSettings.Add(InGameCamState.InterestZone, camSettings5);
		CamSettings camSettings6 = new CamSettings();
		camSettings6.positionOffset = new Vector3(-5.5f, 5f, 0f);
		camSettings6.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings6.dist = 7f;
		camSettings6.interpolationSpeed = 2f;
		camSettings6.transitionOutTime = 1f;
		stateSettings.Add(InGameCamState.InterestPoint, camSettings6);
		CamSettings camSettings7 = new CamSettings();
		camSettings7.positionOffset = new Vector3(-3.5f, 6f, 0f);
		camSettings7.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings7.dist = 14f;
		camSettings7.interpolationSpeed = 0.5f;
		camSettings7.transitionOutTime = 0.3f;
		stateSettings.Add(InGameCamState.Traversal, camSettings2);
		CamSettings camSettings8 = new CamSettings();
		camSettings8.positionOffset = new Vector3(-3.5f, 2f, 0f);
		camSettings8.lookAtOffset = new Vector3(0f, 0.6f, 0f);
		camSettings8.dist = 5f;
		camSettings8.interpolationSpeed = 2f;
		camSettings8.transitionOutTime = 1f;
		stateSettings.Add(InGameCamState.StartingTraversal, camSettings8);
		combatPositionOffsets.Add(camSettings3.positionOffset);
		combatPositionOffsets.Add(new Vector3(0f, 5f, 0f));
		combatPositionOffsets.Add(new Vector3(2.5f, 5f, 0f));
		combatPositionOffsets.Add(new Vector3(-5.5f, 4f, 1.5f));
		combatPositionOffsets.Add(new Vector3(5.5f, 4f, 1.5f));
		combatPositionOffsets.Add(new Vector3(-7.5f, 3.5f, 2.5f));
		combatPositionOffsets.Add(new Vector3(7.5f, 3.5f, 2.5f));
		combatPositionOffsets.Add(new Vector3(-7.5f, 3.5f, 5f));
		combatPositionOffsets.Add(new Vector3(7.5f, 3.5f, 5f));
		explorationPositionOffsets.Add(camSettings2.positionOffset);
		explorationPositionOffsets.Add(new Vector3(-7.5f, 6f, 7f));
		explorationPositionOffsets.Add(new Vector3(-14f, 6f, 14f));
		explorationPositionOffsets.Add(new Vector3(7.5f, 6f, 7f));
		explorationPositionOffsets.Add(new Vector3(14f, 6f, -4f));
	}
}
