using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualBasic;
using MongoDB;

namespace PokemonTCG
{

    public class Deck : List<Card>
    {

        //Custom Method(s)
        public Card draw()
        {
            //Returns the top card. 
            return this[0];
        }
    }
}