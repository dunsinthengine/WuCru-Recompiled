using LEGO.CoreSDK.VPC;

namespace Cape.LegoSdk.Core.View
{
	public class LegoSdkParentalGateViewProxy : IParentalGateView
	{
		public IParentalGateView Implementation { private get; set; }

		public void Show(ParentalGateModel model)
		{
			Implementation.Show(model);
		}

		public void Cancel()
		{
			Implementation.Cancel();
		}
	}
}
