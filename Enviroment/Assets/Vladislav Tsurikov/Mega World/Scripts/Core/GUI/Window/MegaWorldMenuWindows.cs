#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace  VladislavTsurikov.MegaWorldSystem
{
    public static class MegaWorldMenuWindows
    {
        [MenuItem("Window/Vladislav Tsurikov/Mega World/Patreon", false, 8000)]
        public static void Patreon()
        {
            Application.OpenURL("https://www.patreon.com/user/posts?u=62137729");
        }

        [MenuItem("Window/Vladislav Tsurikov/Mega World/Discord Server", false, 8000)]
        public static void Discord()
        {
            Application.OpenURL("https://discord.gg/fVAmyXs8GH");
        }

        [MenuItem("Window/Vladislav Tsurikov/Mega World/Documentation", false, 8002)]
        public static void Documentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1o_wtpxailmEGdtEwp5BGIyV8SXklvlJQp9vY2YoTBx4/edit?usp=sharing");
        }
    }
}
#endif