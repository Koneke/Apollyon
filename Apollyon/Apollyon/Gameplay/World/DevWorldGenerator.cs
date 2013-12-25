using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Apollyon
{
    class DevWorldGenerator : WorldGenerator
    {
        public override World Generate(World _world)
        {
            Faction _f = new Faction("The Rude Dudes");
            Faction _aifactionA = new Faction("The Lumberjack Organization");
            Faction _aifactionB = new Faction("The Lumberjack Organization B");

            Game.PlayerFaction = _f;

            Faction.SetRelations(_f, _aifactionA, 0f);
            Faction.SetRelations(_aifactionB, _aifactionA, -1f);
            
            Ship _s = new Ship(new Vector2(100, 300), _world);
            _world.SpaceObjects.Add(_s);

            _s.AddItem(ItemDatabase.Spawn(_world,
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _s.AddItem(ItemDatabase.Spawn(_world,
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _s.AddItem(ItemDatabase.Spawn(_world,
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _s.AddItem(ItemDatabase.Spawn(_world, //spawn into inventory
                ItemDatabase.Items.Find(x => x.ID == 1199)));
            ItemDatabase.Spawn(_world, //spawn into space
                ItemDatabase.Items.Find(x => x.ID == 1100))
                .SetPosition(new Vector2(100, 100));

            _s.Faction = _f;

            AISimpleMiner _AI = new AISimpleMiner();
            Game.AIs.Add(_AI);
            AISimpleFighter _AI2 = new AISimpleFighter();
            _AI2.Faction = _aifactionA;
            Game.AIs.Add(_AI2);

            AISimpleFighter _AI3 = new AISimpleFighter();
            _AI3.Faction = _aifactionB;
            Game.AIs.Add(_AI3);

            for (int i = 0; i < 3; i++)
            {
                _s = new Ship(new Vector2(
                    (float)Game.Random.NextDouble()*7000,
                    (float)Game.Random.NextDouble()*7000
                    ),
                    _world
                );
                _world.SpaceObjects.Add(_s);
                _s.AddItem(ItemDatabase.Spawn(_world,
                    ItemDatabase.Items.Find(x => x.ID == 1101)));
                _AI2.Fleet.Add(_s);
                _s.Faction = _aifactionA;

                _s = new Ship(new Vector2(
                    (float)Game.Random.NextDouble()*7000,
                    (float)Game.Random.NextDouble()*7000
                    ),
                    _world
                );
                _world.SpaceObjects.Add(_s);
                _s.AddItem(ItemDatabase.Spawn(_world,
                    ItemDatabase.Items.Find(x => x.ID == 1101)));
                _AI3.Fleet.Add(_s);
                _s.Faction = _aifactionB;
            }

            _s = new Ship(new Vector2(100, 100), _world);
            _world.SpaceObjects.Add(_s);
            _s.AddItem(ItemDatabase.Spawn(_world,
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _s.AddItem(ItemDatabase.Spawn(_world,
                ItemDatabase.Items.Find(x => x.ID == 1102)));
            _AI.Fleet.Add(_s);
            _s.Faction = _aifactionA;

            Asteroid _a = new Asteroid("Asteroid", _world);
            _a.Position = new Vector2(400, 400);

            _a = new Asteroid("Asteroid", _world);
            _a.Position = new Vector2(0, 80);

            Container _c = new Container("Generic Container", _world);

            return _world;
        }
    }
}
