using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Demo2 : MonoBehaviour
{
	public ParticleSystem monsterParticle;

	public Animator kittenAnimator;

	public TypefaceAnimator typeface;

	private void Start()
	{
		monsterParticle.GetComponent<Renderer>().sortingLayerName = "foreground";
		monsterParticle.GetComponent<Renderer>().sortingOrder = 2;
		StartCoroutine(KittenActionCoroutine());
	}

	private IEnumerator KittenActionCoroutine()
	{
		while (true)
		{
			kittenAnimator.Play("Idle");
			yield return new WaitForSeconds(2f);
			typeface.gameObject.SetActive(false);
			kittenAnimator.Play("Attack");
			yield return new WaitForSeconds(0.3f);
			monsterParticle.Play();
			yield return new WaitForSeconds(0.4f);
			typeface.gameObject.SetActive(true);
			typeface.GetComponent<Text>().text = Random.Range(5000, 9999).ToString();
		}
	}
}
