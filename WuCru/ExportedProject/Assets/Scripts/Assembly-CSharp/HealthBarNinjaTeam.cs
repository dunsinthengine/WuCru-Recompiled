using UnityEngine;
using UnityEngine.UI;

public class HealthBarNinjaTeam : MonoBehaviour
{
	public Image imageFill;

	private int maxHealth = 100;

	private void Start()
	{
		imageFill.fillAmount = 1f;
	}

	public void UpdateMaxHealth(int maxHealth)
	{
		this.maxHealth = maxHealth;
	}

	public void UpdateHealth(int absoluteHealth)
	{
		if (imageFill != null)
		{
			imageFill.fillAmount = (float)absoluteHealth / (float)maxHealth;
		}
	}

	public void UpdateHealth(float healthFactor)
	{
		if ((bool)imageFill)
		{
			imageFill.fillAmount = healthFactor;
		}
	}
}
