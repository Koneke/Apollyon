﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class Matrix<T>
    {
        Dictionary<Tuple<int, int>, T> matrix =
            new Dictionary<Tuple<int, int>, T>();

        public T Get(int _x, int _y)
        {
            if(Exists(_x, _y))
                return matrix[new Tuple<int,int>(_x,_y)];
            else return default(T);
        }

        public void Set(int _x, int _y, T _value)
        {
            //doubling like this seems a bit dumb, do a bit of thinking, mhm?
            matrix[new Tuple<int,int>(_x,_y)] = _value;
            matrix[new Tuple<int,int>(_y,_x)] = _value;
        }

        public bool Exists(int _x, int _y)
        {
            return matrix.ContainsKey(new Tuple<int, int>(_x, _y));
        }
    }

    class Faction
    {
        static Matrix<float> relations = new Matrix<float>();
        public static int IDCounter = 0;
        static List<Faction> factions = new List<Faction>();

        public static void SetRelations(
            Faction _a,
            Faction _b,
            float _relation
        ) {
            relations.Set(_a.ID, _b.ID, _relation);
        }

        public static float GetRelations(
            Faction _a,
            Faction _b
        ) {
            return relations.Get(_a.ID, _b.ID);
        }

        public string Name;
        public int ID;

        public Faction(
            string _name)
        {
            Name = _name;
            ID = IDCounter;
            SetRelations(this, this, 1f); //friendly with ourselves
            IDCounter += 1;
            factions.Add(this);
        }

        public List<Faction> GetAbove(float _value)
        {
            List<Faction> _fs = new List<Faction>();
            foreach (Faction _f in factions)
                if (GetRelations(this, _f) >= _value)
                    _fs.Add(_f);
            return _fs;
        }

        public List<Faction> GetBelow(float _value)
        {
            List<Faction> _fs = new List<Faction>();
            foreach (Faction _f in factions)
                if (GetRelations(this, _f) <= _value)
                    _fs.Add(_f);
            return _fs;
        }

        public List<Faction> GetHostiles()
        {
            return GetBelow(-1f);
        }

        public List<Faction> GetFriendlies()
        {
            return GetAbove(1f);
        }
    }
}
