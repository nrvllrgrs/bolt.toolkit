using Ludiq;

namespace Bolt
{
	[UnitTitle("Select Bool On Flow")]
	[UnitCategory("Control")]
	public class SelectBoolOnFlow : Unit
	{
		#region Variables

		[DoNotSerialize, PortLabel("True")]
		public ControlInput enterTrue;

		[DoNotSerialize, PortLabel("False")]
		public ControlInput enterFalse;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize]
		public ValueOutput valueOut;

		protected bool m_value;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enterTrue = ControlInput("true", ProcessTrue);
			enterFalse = ControlInput("false", ProcessFalse);
			exit = ControlOutput("exit");

			valueOut = ValueOutput("value", (flow) => m_value);

			Succession(enterTrue, exit);
			Succession(enterFalse, exit);
		}		

		private ControlOutput ProcessTrue(Flow flow)
		{
			return Process(flow, true);
		}

		private ControlOutput ProcessFalse(Flow flow)
		{
			return Process(flow, false);
		}

		private ControlOutput Process(Flow flow, bool value)
		{
			m_value = value;
			return exit;
		}

		#endregion
	}
}