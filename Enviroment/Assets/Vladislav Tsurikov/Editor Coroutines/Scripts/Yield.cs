#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.EditorCoroutinesSystem
{
    struct YieldDefault : ICoroutineYield
	{
		public bool IsDone(float deltaTime)
		{
			return true;
		}
	}

	struct YieldWaitForSeconds : ICoroutineYield
	{
		public float timeLeft;

		public bool IsDone(float deltaTime)
		{
			timeLeft -= deltaTime;
			return timeLeft < 0;
		}
	}

	struct YieldCustomYieldInstruction : ICoroutineYield
	{
		public CustomYieldInstruction customYield;

		public bool IsDone(float deltaTime)
		{
			return !customYield.keepWaiting;
		}
	}

	struct YieldAsync : ICoroutineYield
	{
		public AsyncOperation asyncOperation;

		public bool IsDone(float deltaTime)
		{
			return asyncOperation.isDone;
		}
	}

	struct YieldNestedCoroutine : ICoroutineYield
	{
		public EditorCoroutine coroutine;

		public bool IsDone(float deltaTime)
		{
			return coroutine.finished;
		}
	}
}
#endif