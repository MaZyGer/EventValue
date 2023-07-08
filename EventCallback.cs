using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Maz.Unity.Events
{
	[System.Serializable]
	public class Argument
	{
		public enum ArgumentTypes { 
			NullValue, 
			ObjectValue, 
			BoolValue, 
			IntValue, 
			FloatValue, 
			StringValue 
		}

		public ArgumentTypes argumentType = ArgumentTypes.NullValue;
		public string argumentTypeName;

		public UnityEngine.Object ObjectValue;
		public int IntValue;
		public float FloatValue;
		public string StringValue;
		public bool BoolValue;
		public object GetValue()
		{
			return GetValue(argumentType);
		}

		public object GetValue(ArgumentTypes type)
		{
			switch (type)
			{
				case ArgumentTypes.BoolValue:
					return BoolValue;
				case ArgumentTypes.IntValue:
					return IntValue;
				case ArgumentTypes.FloatValue:
					return FloatValue;
				case ArgumentTypes.StringValue:
					return StringValue;
				case ArgumentTypes.ObjectValue:
					return ObjectValue;
				default:
					return null;
			}
		}

		public static ArgumentTypes FromRealType(Type type)
		{
			if (type == typeof(bool)) return ArgumentTypes.BoolValue;
			else if (type == typeof(int)) return ArgumentTypes.IntValue;
			else if (type == typeof(float)) return ArgumentTypes.FloatValue;
			else if (type == typeof(String)) return ArgumentTypes.StringValue;
			else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return ArgumentTypes.ObjectValue;
			else return ArgumentTypes.NullValue;
		}

		public static bool IsSupported(ArgumentTypes type)
		{
			return type != ArgumentTypes.NullValue;
		}
	}

	[System.Serializable]
	public abstract class EventCallback : ISerializationCallbackReceiver
	{
		public object[] _args;
		public object[] Args { get { return _args != null ? _args : _args = arguments.Select(x => x.GetValue()).ToArray(); } }

		public UnityEngine.Object targetObject;
		public Argument[] arguments;
		public int[] selectedArguments;

		public string methodName;
		public int selectedIndex;

		public bool isCached = false;

		public void OnBeforeSerialize()
		{
			//throw new NotImplementedException();

		}

		public void OnAfterDeserialize()
		{
			isCached = false;
			_args = null;
		}

		public static T CreateDelegate<T>(object target, string methodName) 
		{
			if (!string.IsNullOrEmpty(methodName))
				return (T)(object)Delegate.CreateDelegate(typeof(T), target, methodName);

			return default;
		}
	}

	public abstract class EventCallbackBase<TReturn> : EventCallback
	{
		//public void SetMethod(UnityEngine.Object target, string methodName, bool dynamic, params Argument[] args)
		//{
		//	this.targetObject = target;
		//	this.methodName = methodName;
		//	this.arguments = args;
		//	isCached = false;
		//}

		protected abstract void Factory();

		protected abstract TReturn Invoke(params object[] args);

		public TReturn Invoke()
		{
			return Invoke(Args);
		}
	}

	[System.Serializable]
	public class EventCallback<TReturn> : EventCallbackBase<TReturn>
	{
		public Func<TReturn> func = default;

		protected override void Factory()
		{
			if (!isCached)
			{
				
				func = CreateDelegate<Func<TReturn>>(targetObject, methodName);

				isCached = true;
			}
		}

		protected override TReturn Invoke(params object[] args)
		{
			Factory();
			return func != null ? func.Invoke() : default;
		}
	}


	[System.Serializable]
	public class EventCallback<T0, TReturn> : EventCallbackBase<TReturn>
	{
		public Func<T0, TReturn> func = default;

		protected override void Factory()
		{
			if (!isCached)
			{
				func = CreateDelegate<Func<T0, TReturn>>(targetObject, methodName);

				isCached = true;
			}
		}

		protected override TReturn Invoke(params object[] args)
		{
			Factory();
			return func != null ? func.Invoke((T0)args[0]) : default;
		}

		public TReturn Invoke(T0 t0)
		{
			return func.Invoke(t0);
		}
	}

	[System.Serializable]
	public class EventCallback<T0, T1, TReturn> : EventCallbackBase<TReturn>
	{
		public Func<T0, T1, TReturn> func = default;

		protected override void Factory()
		{
			if (!isCached)
			{
				func = CreateDelegate<Func<T0, T1, TReturn>>(targetObject, methodName);

				isCached = true;
			}
		}

		protected override TReturn Invoke(params object[] args)
		{
			Factory();
			return func != null ? func.Invoke((T0)args[0], (T1)args[1]) : default;
		}

		public TReturn Invoke(T0 t0, T1 t1)
		{
			return func.Invoke(t0, t1);
		}
	}

	[System.Serializable]
	public class EventCallback<T0, T1, T2, TReturn> : EventCallbackBase<TReturn>
	{
		public Func<T0, T1, T2, TReturn> func = default;

		protected override void Factory()
		{
			if (!isCached)
			{
				func = CreateDelegate<Func<T0, T1, T2, TReturn>>(targetObject, methodName);

				isCached = true;
			}
		}

		protected override TReturn Invoke(params object[] args)
		{
			Factory();
			return func != null ? func.Invoke((T0)args[0], (T1)args[1], (T2)args[2]) : default;
		}

		public TReturn Invoke(T0 t0, T1 t1, T2 t2)
		{
			return func.Invoke(t0, t1, t2);
		}
	}

	[System.Serializable]
	public class EventCallback<T0, T1, T2, T3, TReturn> : EventCallbackBase<TReturn>
	{
		public Func<T0, T1, T2, T3, TReturn> func = default;

		protected override void Factory()
		{
			if (!isCached)
			{
				func = CreateDelegate<Func<T0, T1, T2, T3, TReturn>>(targetObject, methodName);

				isCached = true;
			}
		}

		protected override TReturn Invoke(params object[] args)
		{
			Factory();
			return func != null ? func.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3]) : default;
		}

		public TReturn Invoke(T0 t0, T1 t1, T2 t2, T3 t3)
		{
			return func.Invoke(t0, t1, t2, t3);
		}
	}

}