using Ludiq;

namespace Bolt
{
	public abstract class BaseWaitMonoBehaviourUnit : MonoBehaviourUnit
	{
		#region Variables

		[DoNotSerialize, PortLabel("Finished")]
		public ControlOutput finished;

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();
			finished = ControlOutput("finished");

			Succession(enter, finished);
		}

		protected void Finish()
		{
			InvokeControlOutput(finished);
		}

		#endregion
	}
}
