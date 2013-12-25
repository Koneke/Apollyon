using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using IrrKlang;

namespace Apollyon
{
    static class Audio
    {
        public static ISoundEngine soundEngine = new ISoundEngine();
        public static string ContentRoot;

        public static Vector3 ListenerPosition;
        public static float LinearFalloffStart = 2000;
        public static float LinearFalloffEnd = 4700;

        public static bool Mute = false;

        static List<string> playedLog = new List<string>();

        public static ISound bgm;

        public static void UpdateSettings()
        {
            soundEngine.Default3DSoundMinDistance = 80;
        }

        public static void UpdateListenerPosition()
        {
            ListenerPosition = new Vector3
            (
                Game.Camera.ScreenToWorld(
                    new Vector2(Game.ScreenSize.X / 2, 0)
                ).X,
                Game.Camera.ScreenToWorld(
                    new Vector2(0, Game.ScreenSize.Y / 2)
                ).Y,
                -600 / Game.Camera.GetZoom()
            );
            soundEngine.SetListenerPosition(
                ListenerPosition.X,
                ListenerPosition.Y,
                ListenerPosition.Z,
                0, 0, 1
            );
        }

        public static void SetSoundPosition(
            ISound _sound,
            Vector2 _position
        ) {
            _sound.Position = new Vector3D(
                _position.X,
                _position.Y,
                0);
        }

        public static ISound PlaySoundAtPosition(
            string _file,
            Vector2 _position,
            bool _paused = false,
            bool _looped = false
        ) {
            return PlaySoundAtPosition(
                _file,
                new Vector3(_position.X, _position.Y, 0),
                _paused,
                _looped
            );
        }
        public static ISound PlaySoundAtPosition(
            string _file,
            Vector3 _position,
            bool _paused = false,
            bool _looped = false
        ) {
            if (!System.IO.File.Exists(ContentRoot + _file))
                throw new System.IO.FileNotFoundException
                    (_file+" does not exist.");

            var a = soundEngine.Play3D(
                ContentRoot + _file,
                _position.X,
                _position.Y,
                //always start paused so we can apply volume
                _position.Z, _looped, true 
            );

            float _d = Vector3.Distance(_position, ListenerPosition);
            if (_d > LinearFalloffStart)
            {
                if (_d < LinearFalloffEnd)
                {
                    a.Volume =  1 - (_d / LinearFalloffEnd);
                }
                else a.Volume = 0;
            }

            if(!_paused)
                a.Paused = false;

            //debugging reasons
            if (!playedLog.Contains(_file.ToLower()))
                playedLog.Add(_file.ToLower());

            return a;
        }

        public static ISound Play(
            string _file,
            float _volume = 1,
            bool _paused = false,
            bool _looped = false
        ) {
            if (!System.IO.File.Exists(ContentRoot + _file))
                throw new System.IO.FileNotFoundException
                    (_file+" does not exist.");

            var a = soundEngine.Play2D(
                ContentRoot+_file, _looped, true);
            a.Volume = _volume;

            if (!_paused)
                a.Paused = false;

            return a;
        }
    }
}
