#if UNITY_EDITOR
namespace VladislavTsurikov.EditorCoroutinesSystem
{
    public interface ICoroutineYield
	{
		bool IsDone(float deltaTime);
	}
}
#endif