using UnityEngine;
using Ludiq;

namespace Bolt
{
	public abstract class MonoBehaviourUnit : Unit
	{
		#region Variables

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		protected IMachine m_machine;

		#endregion

		#region Properties

		protected GraphReference graphReference => GraphReference.New(m_machine, true);
		protected MonoBehaviour owner => graphReference?.gameObject.GetComponent<Variables>();

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput("enter", ProcessInternal);
			exit = ControlOutput("exit");

			Succession(enter, exit);
		}

		private ControlOutput ProcessInternal(Flow flow)
		{
			m_machine = flow.stack.machine;
			Process(flow);

			return exit;
		}

		protected abstract void Process(Flow flow);

		protected void InvokeControlOutput(ControlOutput output)
		{
			Flow.New(graphReference)?.Invoke(output);
		}

		#endregion
	}
}
