#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace VladislavTsurikov.EditorCoroutinesSystem
{
	public class EditorCoroutines
	{
		private static EditorCoroutines s_instance = null;

		private Dictionary<string, List<EditorCoroutine>> _coroutineDict = new Dictionary<string, List<EditorCoroutine>>();
		private List<List<EditorCoroutine>> _tempCoroutineList = new List<List<EditorCoroutine>>();

		private Dictionary<string, Dictionary<string, EditorCoroutine>> _coroutineOwnerDict =
			new Dictionary<string, Dictionary<string, EditorCoroutine>>();

		private DateTime _previousTimeSinceStartup;

		/// <summary>Starts a coroutine.</summary>
		/// <param name="routine">The coroutine to start.</param>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static EditorCoroutine StartCoroutine(IEnumerator routine, object thisReference)
		{
			CreateInstanceIfNeeded();
			return s_instance.GoStartCoroutine(routine, thisReference);
		}

		/// <summary>Starts a coroutine.</summary>
		/// <param name="methodName">The name of the coroutine method to start.</param>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static EditorCoroutine StartCoroutine(string methodName, object thisReference)
		{
			return StartCoroutine(methodName, null, thisReference);
		}

		/// <summary>Starts a coroutine.</summary>
		/// <param name="methodName">The name of the coroutine method to start.</param>
		/// <param name="value">The parameter to pass to the coroutine.</param>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static EditorCoroutine StartCoroutine(string methodName, object value, object thisReference)
		{
			MethodInfo methodInfo = thisReference.GetType()
				.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (methodInfo == null)
			{
				Debug.LogError("Coroutine '" + methodName + "' couldn't be started, the method doesn't exist!");
			}
			object returnValue;

			if (value == null)
			{
				returnValue = methodInfo.Invoke(thisReference, null);
			}
			else
			{
				returnValue = methodInfo.Invoke(thisReference, new object[] {value});
			}

			if (returnValue is IEnumerator)
			{
				CreateInstanceIfNeeded();
				return s_instance.GoStartCoroutine((IEnumerator) returnValue, thisReference);
			}
			else
			{
				Debug.LogError("Coroutine '" + methodName + "' couldn't be started, the method doesn't return an IEnumerator!");
			}

			return null;
		}

		/// <summary>Stops all coroutines being the routine running on the passed instance.</summary>
		/// <param name="routine"> The coroutine to stop.</param>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static void StopCoroutine(IEnumerator routine, object thisReference)
		{
			CreateInstanceIfNeeded();
			s_instance.GoStopCoroutine(routine, thisReference);
		}

		/// <summary>
		/// Stops all coroutines named methodName running on the passed instance.</summary>
		/// <param name="methodName"> The name of the coroutine method to stop.</param>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static void StopCoroutine(string methodName, object thisReference)
		{
			CreateInstanceIfNeeded();
			s_instance.GoStopCoroutine(methodName, thisReference);
		}

		/// <summary>
		/// Stops all coroutines running on the passed instance.</summary>
		/// <param name="thisReference">Reference to the instance of the class containing the method.</param>
		public static void StopAllCoroutines(object thisReference)
		{
			CreateInstanceIfNeeded();
			s_instance.GoStopAllCoroutines(thisReference);
		}

		static void CreateInstanceIfNeeded()
		{
			if (s_instance == null)
			{
				s_instance = new EditorCoroutines();
				s_instance.Initialize();
			}
		}

		void Initialize()
		{
			_previousTimeSinceStartup = DateTime.Now;
			EditorApplication.update += OnUpdate;
		}

		void GoStopCoroutine(IEnumerator routine, object thisReference)
		{
			GoStopActualRoutine(CreateCoroutine(routine, thisReference));
		}

		void GoStopCoroutine(string methodName, object thisReference)
		{
			GoStopActualRoutine(CreateCoroutineFromString(methodName, thisReference));
		}

		void GoStopActualRoutine(EditorCoroutine routine)
		{
			if (_coroutineDict.ContainsKey(routine.routineUniqueHash))
			{
				_coroutineOwnerDict[routine.ownerUniqueHash].Remove(routine.routineUniqueHash);
				_coroutineDict.Remove(routine.routineUniqueHash);
			}
		}

		void GoStopAllCoroutines(object thisReference)
		{
			EditorCoroutine coroutine = CreateCoroutine(null, thisReference);
			if (_coroutineOwnerDict.ContainsKey(coroutine.ownerUniqueHash))
			{
				foreach (var couple in _coroutineOwnerDict[coroutine.ownerUniqueHash])
				{
					_coroutineDict.Remove(couple.Value.routineUniqueHash);
				}
				_coroutineOwnerDict.Remove(coroutine.ownerUniqueHash);
			}
		}

		EditorCoroutine GoStartCoroutine(IEnumerator routine, object thisReference)
		{
			if (routine == null)
			{
				Debug.LogException(new Exception("IEnumerator is null!"), null);
			}
			EditorCoroutine coroutine = CreateCoroutine(routine, thisReference);
			GoStartCoroutine(coroutine);
			return coroutine;
		}

		void GoStartCoroutine(EditorCoroutine coroutine)
		{
			if (!_coroutineDict.ContainsKey(coroutine.routineUniqueHash))
			{
				List<EditorCoroutine> newCoroutineList = new List<EditorCoroutine>();
				_coroutineDict.Add(coroutine.routineUniqueHash, newCoroutineList);
			}
			_coroutineDict[coroutine.routineUniqueHash].Add(coroutine);

			if (!_coroutineOwnerDict.ContainsKey(coroutine.ownerUniqueHash))
			{
				Dictionary<string, EditorCoroutine> newCoroutineDict = new Dictionary<string, EditorCoroutine>();
				_coroutineOwnerDict.Add(coroutine.ownerUniqueHash, newCoroutineDict);
			}

			// If the method from the same owner has been stored before, it doesn't have to be stored anymore,
			// One reference is enough in order for "StopAllCoroutines" to work
			if (!_coroutineOwnerDict[coroutine.ownerUniqueHash].ContainsKey(coroutine.routineUniqueHash))
			{
				_coroutineOwnerDict[coroutine.ownerUniqueHash].Add(coroutine.routineUniqueHash, coroutine);
			}

			MoveNext(coroutine);
		}

		EditorCoroutine CreateCoroutine(IEnumerator routine, object thisReference)
		{
			return new EditorCoroutine(routine, thisReference.GetHashCode(), thisReference.GetType().ToString());
		}

		EditorCoroutine CreateCoroutineFromString(string methodName, object thisReference)
		{
			return new EditorCoroutine(methodName, thisReference.GetHashCode(), thisReference.GetType().ToString());
		}

		void OnUpdate()
		{
			float deltaTime = (float) (DateTime.Now.Subtract(_previousTimeSinceStartup).TotalMilliseconds / 1000.0f);

			_previousTimeSinceStartup = DateTime.Now;
			if (_coroutineDict.Count == 0)
			{
				return;
			}

			_tempCoroutineList.Clear();
			foreach (var pair in _coroutineDict)
				_tempCoroutineList.Add(pair.Value);

			for (var i = _tempCoroutineList.Count - 1; i >= 0; i--)
			{
				List<EditorCoroutine> coroutines = _tempCoroutineList[i];

				for (int j = coroutines.Count - 1; j >= 0; j--)
				{
					EditorCoroutine coroutine = coroutines[j];

					if (!coroutine.currentYield.IsDone(deltaTime))
					{
						continue;
					}

					if (!MoveNext(coroutine))
					{
						coroutines.RemoveAt(j);
						coroutine.currentYield = null;
						coroutine.finished = true;
					}

					if (coroutines.Count == 0)
					{
						_coroutineDict.Remove(coroutine.routineUniqueHash);
					}
				}
			}
		}

		static bool MoveNext(EditorCoroutine coroutine)
		{
			if (coroutine.routine.MoveNext())
			{
				return Process(coroutine);
			}

			return false;
		}

		// returns false if no next, returns true if OK
		static bool Process(EditorCoroutine coroutine)
		{
			object current = coroutine.routine.Current;
			if (current == null)
			{
				coroutine.currentYield = new YieldDefault();
			}
			else if (current is WaitForSeconds)
			{
				float seconds = float.Parse(GetInstanceField(typeof(WaitForSeconds), current, "m_Seconds").ToString());
				coroutine.currentYield = new YieldWaitForSeconds() {timeLeft = seconds};
			}
			else if (current is CustomYieldInstruction)
			{
				coroutine.currentYield = new YieldCustomYieldInstruction()
				{
					customYield = current as CustomYieldInstruction
				};
			}
			else if (current is WaitForFixedUpdate || current is WaitForEndOfFrame)
			{
				coroutine.currentYield = new YieldDefault();
			}
			else if (current is AsyncOperation)
			{
				coroutine.currentYield = new YieldAsync {asyncOperation = (AsyncOperation) current};
			}
			else if (current is EditorCoroutine)
			{
				coroutine.currentYield = new YieldNestedCoroutine { coroutine= (EditorCoroutine) current};
			}
			else
			{
				Debug.LogException(
					new Exception("<" + coroutine.MethodName + "> yielded an unknown or unsupported type! (" + current.GetType() + ")"),
					null);
				coroutine.currentYield = new YieldDefault();
			}
			return true;
		}

		static object GetInstanceField(Type type, object instance, string fieldName)
		{
			BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			FieldInfo field = type.GetField(fieldName, bindFlags);
			return field.GetValue(instance);
		}
	}
}
#endif