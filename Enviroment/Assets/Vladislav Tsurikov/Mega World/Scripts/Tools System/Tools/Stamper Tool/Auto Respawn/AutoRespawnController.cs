#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    public class AutoRespawnController
    {
        private StamperTool _stamperTool;
        private AutoRespawn _autoRespawn;
        private Timer _timer = new Timer();

        public AutoRespawnController()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public void StartAutoRespawn(Group group, StamperTool stamperTool)
        {
            _autoRespawn = new AutoRespawn(group, stamperTool);
            _stamperTool = stamperTool;
            _timer.StartTimer(_stamperTool.StamperToolControllerSettings.DelayAutoRespawn);
        }

        public void StartAutoRespawn(PrototypeTerrainDetail proto, StamperTool stamperTool)
        {
            _autoRespawn = new AutoRespawn(proto, stamperTool);
            _stamperTool = stamperTool;
            _timer.StartTimer(_stamperTool.StamperToolControllerSettings.DelayAutoRespawn);
        }   

        private void Update()
		{
            if(_stamperTool == null || _autoRespawn == null)
            {
                return;
            }

            if(_stamperTool.StamperToolControllerSettings.AutoRespawn)
			{
                _timer.UpdateTimer(() => 
                {
                    _autoRespawn.TypeHasChanged();
                    _autoRespawn = null;
                });
			}
            else
            {
                _autoRespawn = null;
            }
		}
    }
}
#endif