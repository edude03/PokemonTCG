using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualBasic;
using MongoDB;
using System.Linq;

namespace PokemonTCG
{

    public class Deck : List<Card>
    {
        Random rng = new Random();
        //Custom Method(s)
        public Card draw()
        {
            //Returns the top card.
            Card temp = this[0];
            this.RemoveAt(0);
            return temp;
        }
        public void shuffle() { shuffle(this); }
        public void shuffle(List<Card> list)
        {
           
           int n = list.Count;
           while (n > 1)
           {
               n--;
               int k = rng.Next(n + 1);
               Card value = list[k];
               list[k] = list[n];
               list[n] = value;
           }
        }
    }
}