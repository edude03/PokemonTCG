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
        public bool draw(out Card draw)
        {
            if (this.Count > 0)
            {
                //Returns the top input.
                draw = this[0];
                this.RemoveAt(0);
                return true;
            }
            draw = null;
            return false;   
        }
        public void shuffle() { shuffle(this); }
		
		/// <summary>
		/// Shuffles a list of objects 
		/// </summary>
		/// <param name="list">
		/// A <see cref="List<Card>"/>
		/// </param>
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