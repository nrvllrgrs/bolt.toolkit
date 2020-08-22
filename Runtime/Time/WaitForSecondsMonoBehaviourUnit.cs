using Ludiq;

namespace Bolt
{
	[UnitTitle("Wait For Seconds")]
	[UnitCategory("Time")]
	public class WaitForSecondsMonoBehaviourUnit : BaseWaitMonoBehaviourUnit
	{
		#region Variables

		[DoNotSerialize]
		public ValueInput delayIn;

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();
			delayIn = ValueInput("delay", 0f);
		}

		protected override void Process(Flow flow)
		{
			float delay = flow.GetValue<float>(delayIn);
			owner.WaitForSeconds(delay, Finish);
		}

		#endregion
	}
}