using Ludiq;

namespace Bolt
{
	[UnitTitle("Wait For Condition")]
	[UnitCategory("Time")]
	public class WaitForConditionMonoBehaviourUnit : BaseWaitMonoBehaviourUnit
	{
		#region Variables

		[DoNotSerialize]
		public ValueInput conditionIn;
		
		[DoNotSerialize, PortLabelHidden]
		public ValueInput valueIn;

		#endregion
		
		#region Properties
		
		protected bool value { get; set; }
		
		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();
			conditionIn = ValueInput<bool>("condition");
			valueIn = ValueInput("value", true);
		}

		protected override void Process(Flow flow)
		{
			value = flow.GetValue<bool>(valueIn);
			owner.WaitWhile(
				() => flow.GetValue<bool>(conditionIn) == value,
				Finish);
		}

		#endregion
	}
}