public class CreditLine
{
	public CreditType type;

	public string data;

	public string data2;

	public CreditLine(CreditType type, string data, string data2)
	{
		this.type = type;
		this.data = data;
		this.data2 = data2;
	}

	public static string typeToText(CreditType t)
	{
		switch (t)
		{
		case CreditType.Title:
			return "title";
		case CreditType.SingleColumn:
			return "singlecolumn";
		case CreditType.Image:
			return "texture";
		case CreditType.EmptySpace:
			return "space";
		default:
			return "twocolumns";
		}
	}

	public static CreditType textToType(string t)
	{
		switch (t)
		{
		case "title":
			return CreditType.Title;
		case "singlecolumn":
			return CreditType.SingleColumn;
		case "texture":
			return CreditType.Image;
		case "space":
			return CreditType.EmptySpace;
		default:
			return CreditType.TwoColumns;
		}
	}
}
