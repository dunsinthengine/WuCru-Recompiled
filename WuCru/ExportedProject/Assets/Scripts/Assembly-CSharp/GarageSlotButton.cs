using UnityEngine;

public class GarageSlotButton : MonoBehaviour
{
	private const string SELECTED_NOT_COMPLETE_SPRITE = "IncompleteActive";

	private const string UNSELECTED_NOT_COMPLETE_SPRITE = "IncompleteStatic";

	private const string SELECTED_COMPLETE_SPRITE = "CompleteActive";

	private const string UNSELECTED_COMPLETE_SPRITE = "CompleteStatic";

	private const int SELECTED_HOLDER_WIDTH = 228;

	private const int UNSELECTED_HOLDER_WIDTH = 192;

	[SerializeField]
	private UIWidget _holder;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _selectedIcon;

	[SerializeField]
	private UISprite _completedTick;

	public GoldenTrailFX _goldenFrame;

	private bool _isSelected;

	private bool _isCompleted;

	public void SetSelectedState(bool isSelected)
	{
		_isSelected = isSelected;
		RefreshSprites();
	}

	public void SetCompletedState(bool isCompleted)
	{
		_isCompleted = isCompleted;
		RefreshSprites();
	}

	private void RefreshSprites()
	{
		_background.spriteName = (_isSelected ? ((!_isCompleted) ? "IncompleteActive" : "CompleteActive") : ((!_isCompleted) ? "IncompleteStatic" : "CompleteStatic"));
		_holder.width = ((!_isSelected) ? 192 : 228);
		_completedTick.gameObject.SetActive(_isCompleted);
		_selectedIcon.gameObject.SetActive(_isSelected);
	}
}
