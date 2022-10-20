using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    public class DataPackage : ScriptableObject
    {
        public BasicData BasicData = new BasicData();

        public SelectedVariables SelectedVariables
        {
            get
            {
                return BasicData.SelectedVariables;
            }
        }

#if UNITY_EDITOR
        public void SaveAllData()
		{
            EditorUtility.SetDirty(this);
			BasicData.SaveAllData();
		}
#endif
    }
}