using UnityEngine;

namespace Bolt.Toolkit
{
	public abstract class ToggleRegisterMonoBehaviourEventUnit<K, TArgs> : ToggleRegisterEventUnit<K, TArgs>
		where K : MonoBehaviour
	{
		#region Variables

		[UnitHeaderInspectable("Auto Add Component")]
		public bool autoAddComponent = true;

		#endregion

		#region Methods

		protected override void CheckMonitored(Flow flow)
		{
			if (m_monitored == null && autoAddComponent)
			{
				var obj = flow?.GetValue<GameObject>(monitored);
				if (obj != null)
				{
					obj.TryAddComponent(out m_monitored);
				}
			}

			base.CheckMonitored(flow);
		}

		#endregion
	}
}