using UnityEngine;

public class DyingFromBehind : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Character component = animator.transform.parent.GetComponent<Character>();
		if ((bool)component)
		{
			component.Visible(false);
		}
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>((!component.isPlayer) ? "Prefabs/Characters/Stnd_Nindroid_Death" : "Prefabs/Characters/Stnd_Nindroid_Death"));
		gameObject.transform.position = component.transform.position + Vector3.up * 0.1f;
		gameObject.transform.rotation = component.transform.rotation;
		MenuCharacter component2 = gameObject.GetComponent<MenuCharacter>();
		CharacterBuilder.ConfigureMinifig(component2, component.NinjaID);
		if ((bool)component2.hairGameObject)
		{
			component2.hairGameObject.AddComponent<Rigidbody>();
			component2.hairGameObject.GetComponentInChildren<Collider>().enabled = true;
		}
		if ((bool)component2.helmetGameObject)
		{
			component2.helmetGameObject.AddComponent<Rigidbody>();
			component2.helmetGameObject.GetComponentInChildren<Collider>().enabled = true;
			ParticleSystem[] componentsInChildren = component2.helmetGameObject.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
		if ((bool)component2.weaponGameObject)
		{
			component2.weaponGameObject.AddComponent<Rigidbody>();
			component2.weaponGameObject.GetComponentInChildren<Collider>().enabled = true;
		}
	}
}
