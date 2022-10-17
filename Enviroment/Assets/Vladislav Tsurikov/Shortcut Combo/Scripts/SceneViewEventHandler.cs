using System;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.ShortcutComboSystem
{
    public static class SceneViewEventHandler
    {
        private static PressedKeyboardButtons _pressedKeyboardButtons = new PressedKeyboardButtons();

        public static PressedKeyboardButtons PressedKeyboardButtons
        {
            get
            {
                return _pressedKeyboardButtons;
            }
        }

        public static void HandleSceneViewEvent(Event e)
        {
            switch(e.type)
            {
                case EventType.KeyDown:
				{
                    //When C is pressed, EventType.KeyUp does not fire
                    if(e.keyCode == KeyCode.C)
                    {
                        return;
                    }

					_pressedKeyboardButtons.OnKeyboardButtonPressed(e.keyCode);
                    break;
				}
                case EventType.KeyUp:
				{
					_pressedKeyboardButtons.OnKeyboardButtonReleased(e.keyCode);
                    break;
				}
            }

            _pressedKeyboardButtons.DeleteKeyCode(KeyCode.None);
        }
    }
}