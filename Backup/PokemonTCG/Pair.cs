using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonTCG
{
    public class Pair
    {
       public Enums.Element type;
       public int value;

        public Pair(int value, Enums.Element type)
        {
            this.type = type;
            this.value = value;
        }

    }
}
