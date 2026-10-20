using DentedPixel.LTEditor;

public static class LeanTweenHelper
{
	public static float GetDuration(LeanTweenVisual ltv, bool onStart = false)
	{
		float num = 0f;
		foreach (LeanTweenGroup group in ltv.groupList)
		{
			num += group.delay;
			for (int i = 0; i < group.itemList.Count && (i != group.itemList.Count - 1 || !onStart); i++)
			{
				num += group.itemList[i].duration;
			}
		}
		return num;
	}
}
