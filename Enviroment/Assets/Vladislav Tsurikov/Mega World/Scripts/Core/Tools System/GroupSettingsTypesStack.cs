using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class GroupSettingsTypesStack 
    {  
        private List<System.Type> _settingsTypesList = new List<System.Type>();

        public void AddTypesSettings(List<System.Type> settingsTypes)
        {
            if(settingsTypes == null)
            {
                return;
            }

            foreach (System.Type type in settingsTypes)
            {
                if(!_settingsTypesList.Contains(type))
                {
                    _settingsTypesList.Add(type);
                }
            }
        }

        public void GetSettingsTypes(Func<System.Type, bool> func)
        {
            foreach (System.Type type in _settingsTypesList)
            {
                func.Invoke(type);
            }
        }
    }
}