using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maz.Unity.Events
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EventValueChangedAttribute : PropertyAttribute
	{

	}
	public interface IEventValue
	{
		void Raise();
		void OnChange(object value);
	}

	[System.Serializable]
	public class EventValue<T> : IEventValue
	{
		[SerializeField, EventValueChanged]
		T value;
		public T Value
		{
			get => value;
			set
			{
				if (this.value == null || !this.value.Equals(value))
				{
					OnChange(value);
				}
			}
		}

		// Needed for Editor.
		public void OnChange(object value)
		{
			this.value = (T)value;
			changed = true;
			Raise();
		}

		bool changed;
		/// <summary>
		/// Return true if value changed.
		/// Next time it will return false if nothing changed.
		/// </summary>
		public bool Changed
		{
			get
			{
				bool retValue = changed;

				if (changed)
					changed = false;

				return retValue;
			}

		}

		public UnityEvent<T> OnChangedEvent = default;

		public EventValue(T t)
		{
			value = t;
		}

		public void Raise()
		{
			OnChangedEvent?.Invoke(value);
		}


	}


}