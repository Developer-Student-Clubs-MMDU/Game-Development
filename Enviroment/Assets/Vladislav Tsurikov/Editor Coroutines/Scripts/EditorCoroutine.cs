#if UNITY_EDITOR
using System.Collections;

namespace VladislavTsurikov.EditorCoroutinesSystem
{
	public class EditorCoroutine
	{
		public ICoroutineYield currentYield = new YieldDefault();
		public IEnumerator routine;
		public string routineUniqueHash;
		public string ownerUniqueHash;
		public string MethodName = "";

		public int ownerHash;
		public string ownerType;

		public bool finished = false;

		public EditorCoroutine(IEnumerator routine, int ownerHash, string ownerType)
		{
			this.routine = routine;
			this.ownerHash = ownerHash;
			this.ownerType = ownerType;
			ownerUniqueHash = ownerHash + "_" + ownerType;

			if (routine != null)
			{
				string[] split = routine.ToString().Split('<', '>');
				if (split.Length == 3)
				{
					this.MethodName = split[1];
				}
			}

			routineUniqueHash = ownerHash + "_" + ownerType + "_" + MethodName;
		}

		public EditorCoroutine(string methodName, int ownerHash, string ownerType)
		{
			MethodName = methodName;
			this.ownerHash = ownerHash;
			this.ownerType = ownerType;
			ownerUniqueHash = ownerHash + "_" + ownerType;
			routineUniqueHash = ownerHash + "_" + ownerType + "_" + MethodName;
		}
	}
}
#endif