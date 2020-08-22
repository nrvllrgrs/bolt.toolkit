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

		protected GraphReference m_graphReference;

		#endregion

		#region Properties

		protected MonoBehaviour owner => m_graphReference?.gameObject.GetComponent<Variables>();

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
			m_graphReference = flow.stack.AsReference();
			Process(flow);

			return exit;
		}

		protected abstract void Process(Flow flow);

		protected void InvokeControlOutput(ControlOutput output)
		{
			var flow = Flow.New(m_graphReference);
			flow?.Invoke(output);
		}

		#endregion
	}
}
