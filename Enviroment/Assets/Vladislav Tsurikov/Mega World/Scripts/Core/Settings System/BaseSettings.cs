using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BaseSettings : ScriptableObject
    {
        public virtual void Init(ScriptableObject asset){}
        public virtual void OnGUI(){}

    }
}
