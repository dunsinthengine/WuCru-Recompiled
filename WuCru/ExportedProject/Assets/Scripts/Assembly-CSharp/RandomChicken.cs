using UnityEngine;

public class RandomChicken : MonoBehaviour
{
	public Rigidbody[] _rbs;

	private float _t;

	private bool _triggeredPhysics;

	private float _timer;

	private void OnEnable()
	{
		_t = 0.05f;
	}

	private void Update()
	{
		_t -= Time.deltaTime;
		if (_t < 0f && !_triggeredPhysics)
		{
			int num = _rbs.Length;
			for (int i = 0; i < num; i++)
			{
				_rbs[i].isKinematic = false;
				_rbs[i].AddForce(Vector3.up * 5f, ForceMode.Impulse);
			}
			AudioPlayer.PlaySound("props/bush/chicken", base.gameObject);
			_triggeredPhysics = true;
			_timer = 1.25f;
		}
		else if (_t < -3f)
		{
			float num2 = Mathf.Clamp01(_t + 4f);
			int num3 = _rbs.Length;
			for (int j = 0; j < num3; j++)
			{
				_rbs[j].transform.localScale = Vector3.one * num2 * 0.01f;
			}
		}
		else
		{
			_timer -= Time.deltaTime;
			if (_timer < 0f && Random.Range(0, 100) < 5)
			{
				AudioPlayer.PlaySound("props/bush/chicken", base.gameObject);
				_timer = 1.25f;
			}
		}
	}
}
