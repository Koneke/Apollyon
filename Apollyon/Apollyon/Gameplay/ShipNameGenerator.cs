using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    static class ShipNameGenerator
    {
        public static List<string> Adjectives = new List<string>()
        {
            "Jolly", "Nice", "Flaky", "Spooky", "Ghastly", "Gankin'"
            ,"Scandalous", "Terrifying", "Awful"
        };

        public static List<string> Birds = new List<string>()
        {
            "Sparrow", "Swan", "Eagle", "Crow", "Pidgeon", "Lumber Jack",
            "Snakeman", "Crocodile", "Creep", "Dentist"
        };

        public static string GenerateName()
        {
            return
                "The " +
                Adjectives[Game.Random.Next(0, Adjectives.Count)] + " " +
                Birds[Game.Random.Next(0, Birds.Count)];
        }
    }
}
