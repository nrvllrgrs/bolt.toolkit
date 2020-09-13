using UnityEngine;
using Ludiq;

namespace Bolt.Toolkit
{
	public abstract class ToggleRegisterEventUnit<K, TArgs> : EventUnit<TArgs>
		where K : Object
	{
		#region Variables

		[UnitHeaderInspectable("Start On")]
		public bool startOn = true;

		[DoNotSerialize, AllowsNull]
		public ControlInput on, off;

		[DoNotSerialize, PortLabelHidden, NullMeansSelf]
		public ValueInput monitored;

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput eventArgs;

		[DoNotSerialize]
		public ControlOutput turnedOn, turnedOff;

		private ControlOutput m_trigger;

		private GraphReference m_graphReference;
		protected K m_monitored;
		protected TArgs m_eventArgs;
		private bool m_registered;

		#endregion

		#region Properties

		public override bool isControlRoot => true;
		protected sealed override bool register { get; }
		protected virtual string hookName { get; }

		protected GraphReference graphReference => m_graphReference;
		protected MonoBehaviour owner => graphReference?.gameObject.GetComponent<Variables>();

		protected virtual bool showMonitored => true;
		protected virtual bool showEventArgs => true;
		protected virtual System.Func<bool> registerPredicate => null;

		#endregion

		#region Methods

		protected override void Definition()
		{
			// Inputs
			on = ControlInput("on", TurnOn);
			off = ControlInput("off", TurnOff);

			if (showMonitored)
			{
				monitored = ValueInput<K>("monitored", null);
			}

			// Outputs
			m_trigger = ControlOutput("trigger");

			if (showEventArgs)
			{
				eventArgs = ValueOutput("eventArgs", (flow) => { return m_eventArgs; });
			}

			turnedOn = ControlOutput("turnedOn");
			turnedOff = ControlOutput("turnedOff");

			// Error handling
			Succession(on, turnedOn);
			Succession(off, turnedOff);
		}

		private ControlOutput TurnOn(Flow flow)
		{
			return AttemptRegister(flow)
				? turnedOn
				: null;
		}

		private ControlOutput TurnOff(Flow flow)
		{
			return AttemptUnregister()
				? turnedOff
				: null;
		}

		protected bool AttemptRegister(Flow flow)
		{
			// TODO: Fix problem where macro units are only registered once
			if (m_registered)
				return false;

			if (showMonitored)
			{
				CheckMonitored(flow);
			}

			if (registerPredicate == null)
			{
				FinalizeRegister();
			}
			else
			{
				owner.WaitUntil(registerPredicate, () =>
				{
					FinalizeRegister();
				});
			}

			return true;
		}

		private void FinalizeRegister()
		{
			m_registered = true;
			Register();
		}

		protected virtual void CheckMonitored(Flow flow)
		{
			if (m_monitored == null)
			{
				m_monitored = flow?.GetValue<K>(monitored);

				if (m_monitored == null)
				{
					throw new System.ArgumentException(string.Format("Monitored object does not contain {0}!", typeof(K).ToString()));
				}
			}
		}

		protected bool AttemptUnregister()
		{
			if (!m_registered)
				return false;

			m_registered = false;
			Unregister();
			return true;
		}

		public override void StartListening(GraphStack stack)
		{
			//m_machine = stack.machine;
			m_graphReference = stack.ToReference();

			if (startOn && AttemptRegister(Flow.New(graphReference)))
			{
				base.StartListening(stack);
			}
		}

		public override void StopListening(GraphStack stack)
		{
			if (AttemptUnregister())
			{
				base.StopListening(stack);
			}
		}

		protected override bool ShouldTrigger(Flow flow, TArgs args)
		{
			return CheckShouldTrigger(flow, out Data data);
		}

		protected bool CheckShouldTrigger(Flow flow, out Data data)
		{
			data = flow.stack.GetElementData<Data>(this);
			if (!data.isListening)
				return false;

			m_monitored = flow.GetValue<K>(monitored);
			return true;
		}

		protected virtual void ProcessTrigger(object sender, TArgs args)
		{
			InvokeTrigger(args);
		}

		protected virtual void InvokeTrigger(TArgs args)
		{
			m_eventArgs = args;
			InvokeTrigger();
		}

		protected virtual void ProcessTrigger(object sender)
		{
			InvokeTrigger();
		}

		protected virtual void InvokeTrigger()
		{
			if (graphReference != null)
			{
				Flow.New(graphReference)?.Invoke(m_trigger);
			}
		}

		protected abstract void Register();
		protected abstract void Unregister();

		#endregion
	}
}
