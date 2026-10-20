using UnityEngine;
using UnityEngine.UI;

public class CompletionObjectContainerGUI : MonoBehaviour
{
	public GameObject root;

	public Image imageCrystal;

	public Image imageCrystalEarth;

	public Image imageCrystalFire;

	public Image imageCrystalIce;

	public Image imageCrystalElectricity;

	public Image imageCrystalEnergy;

	public Image imageCrystalWater;

	public Image imageChestWood;

	public Image imageChestSilver;

	public Image imageChestGold;

	public Image imageNotFound;

	public Image imageFound;

	private Material failedMaterial;

	private void Awake()
	{
		failedMaterial = new Material(imageCrystal.material);
		failedMaterial.color = new Color(1f, 1f, 1f, 0.5f);
		imageCrystal.enabled = false;
		imageCrystalEarth.enabled = false;
		imageCrystalFire.enabled = false;
		imageCrystalIce.enabled = false;
		imageCrystalElectricity.enabled = false;
		imageCrystalEnergy.enabled = false;
		imageCrystalWater.enabled = false;
		imageChestWood.enabled = false;
		imageChestSilver.enabled = false;
		imageChestGold.enabled = false;
		imageNotFound.enabled = false;
		imageFound.enabled = false;
	}

	public void SetCompleted(CompletionObjectType type, bool completed)
	{
		if (type == CompletionObjectType.Crystal)
		{
			imageCrystal.enabled = true;
		}
		if (type == CompletionObjectType.CrystalEarth)
		{
			imageCrystalEarth.enabled = true;
		}
		if (type == CompletionObjectType.CrystalFire)
		{
			imageCrystalFire.enabled = true;
		}
		if (type == CompletionObjectType.CrystalIce)
		{
			imageCrystalIce.enabled = true;
		}
		if (type == CompletionObjectType.CrystalElectricity)
		{
			imageCrystalElectricity.enabled = true;
		}
		if (type == CompletionObjectType.CrystalEnergy)
		{
			imageCrystalEnergy.enabled = true;
		}
		if (type == CompletionObjectType.CrystalWater)
		{
			imageCrystalWater.enabled = true;
		}
		if (type == CompletionObjectType.ChestWood)
		{
			imageChestWood.enabled = true;
		}
		if (type == CompletionObjectType.ChestSilver)
		{
			imageChestSilver.enabled = true;
		}
		if (type == CompletionObjectType.ChestGold)
		{
			imageChestGold.enabled = true;
		}
		imageNotFound.enabled = !completed;
		imageFound.enabled = completed;
		if (!completed)
		{
			imageCrystal.material = failedMaterial;
			imageCrystalEarth.material = failedMaterial;
			imageCrystalFire.material = failedMaterial;
			imageCrystalIce.material = failedMaterial;
			imageCrystalElectricity.material = failedMaterial;
			imageCrystalEnergy.material = failedMaterial;
			imageCrystalWater.material = failedMaterial;
			imageChestWood.material = failedMaterial;
			imageChestSilver.material = failedMaterial;
			imageChestGold.material = failedMaterial;
		}
	}
}
