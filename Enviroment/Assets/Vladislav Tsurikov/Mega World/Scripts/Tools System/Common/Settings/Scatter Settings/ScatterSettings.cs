using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class ScatterSettings : BaseSettings
    {
        public ScatterStack Stack = new ScatterStack();

		public override void Init(ScriptableObject asset)
		{
			if(!Stack.Init)
            {
                Stack.AddSettings(typeof(Grid), asset);
                Stack.AddSettings(typeof(FailureRate), asset);

                Stack.Init = true;
            }
		}

#if UNITY_EDITOR
		private ScatterStackEditor _settingsStackEditor = null;
		public override void OnGUI() 
		{

		}

		public void OnGUI(Group group) 
		{
			if(_settingsStackEditor == null || _settingsStackEditor.Stack == null )
			{
				_settingsStackEditor = new ScatterStackEditor(new GUIContent("Scatter Operations"), Stack, group);
			}

			_settingsStackEditor.OnGUI(group);
		}
#endif
    }
}