﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class Game
    {
        public static Camera Camera;
        public static Vector2 ScreenSize;

        public static World World;

        public static List<Ship> Fleet = new List<Ship>();

        public static float TickTime = 1000 / 60; //time in ms per time tick

        public static Boolean Verbose = false;

        public static Random Random = new Random();

        public static int DoubleClickTime = 200; //dct in ms

        public static bool HasFocus = true;

        public static List<Ship> Selected;
        public static List<Ship> Targeted;

        public enum TargetingType {
            Cycle,
            Weakest,
            Strongest,
            Random
        }

        public static void Log(string _message)
        {
            //ApUI.CombatLog.Log.Add(_message);
            ((ApLogWindow)WindowManager.GetWindowByName("Combat Log")).
                Log.Add(_message);
        }
    }
}
