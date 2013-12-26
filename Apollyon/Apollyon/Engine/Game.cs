using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    static class Game
    {
        public static Camera Camera;
        public static Vector2 ScreenSize;

        public static World World;

        public static float TickTime = 1000 / 60; //time in ms per time tick

        public static Boolean Verbose = false;

        public static Random Random = new Random();

        public static int DoubleClickTime = 200; //dct in ms
        public static int MouseWheelDelta;

        public static Faction PlayerFaction;

        public static bool HasFocus = true;

        //such fukn hack
        public static SpaceState SpaceState {
            get {
                return (SpaceState)GetState("space");
            }
            set {
                AddState("space", value);
            }
        }

        public static List<GameState> ActiveStates
            = new List<GameState>();
        static Dictionary<string, GameState> States
            = new Dictionary<string,GameState>();

        public static void AddState(string _state, GameState _gs)
        {
            if (States.ContainsKey(_state.ToLower()))
                States[_state] = _gs;
            else States.Add(_state, _gs);
        }
        public static GameState GetState(string _state)
        {
            if (States.ContainsKey(_state.ToLower()))
                return States[_state.ToLower()];
            return null;
        }

        public enum TargetingType {
            Cycle,
            Weakest,
            Strongest,
            Random
        }

        public static void Log(string _message)
        {
            ((ApLogWindow)
                GetState("space")
                .WindowManager.GetWindowByName("Combat Log"))
                .Log.Add(_message);
        }

        public static List<AISimpleBrain> AIs = new List<AISimpleBrain>();
    }
}
