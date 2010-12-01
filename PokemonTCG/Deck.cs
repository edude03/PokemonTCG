using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualBasic;
using MongoDB;

namespace PokemonTCG
{
    
    public class Deck
    {
        private Card[] deck;

        //Variables
        int counter = 0;
        private Card temp;
        private int[] intDeck;

        /// <summary>
        /// Number of items in the deck
        /// </summary>
        public int Size;
		
		
		//Deck assuming that a premade deck is passed in
		public Deck(Card[] deck)
		{
			this.deck = deck;
		}
		
		//Deck with a CSV Stream
	    public Deck(int[] intDeck)
		{
		}
		
		
		
		
		
        //Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deckpath">Path to the decks folder</param>
        /// <param name="deckname">Name of the file to use</param>
        /// <remarks>The combination of deckpath and deckname is used to open the file</remarks>
        public Deck(string deckpath, string deckname)
        {
            //Makes sure the deck is created with only the amount of cards that where passed in.
            

            intDeck = Program.LoadDeck(deckpath, deckname);
            this.Size = intDeck.Length;
      
            //Init the cards to their proper values

            deck = new Card[this.Size];
			
			//TODO: If mongo can't connect for some reason find out why and correct the issue,
			//To improve performance and to ensure that we don't run out of connections 
			//A universal connection is created outside the loop, then closed after the deck is created. 
			
			//Instanate the connection 
			Mongo mongo = new Mongo();
			
			try 
			{
			mongo.Connect();
			
            	for (int k = 0; k < Size; k++)
            	{
               		 this.deck[k] = new Card(this.intDeck[k], mongo);
            	}
			}
			catch (MongoDB.MongoConnectionException m)
			{
				Console.WriteLine("Error: {0} Please try again.", m.ToString());
				System.Environment.Exit(-1);
			}
			finally
			{
				mongo.Disconnect();
			}
			
			//Close the connection
			mongo.Disconnect();
		}
		

        /// <summary>
        /// Overloaded Shuffle Method. Shuffles the deck when called without parameters
        /// </summary>
        public void Shuffle()
        {
            Shuffle(this.deck);
        }

        /// <summary>
        /// Overloaded Shuffle Method. Shuffle the source list when called. 
        /// </summary>
        /// <param name="deck"></param>
        public void Shuffle(Card[] deck)
        {
            //Card[] a = new Card[deck.Length -1];
            Card temp;
            Random rnd = new Random();



            for (int i = 0; i < deck.Length -1; i++)
            {
                int rand = rnd.Next(0, deck.Length - 1);
                temp = deck[i];
                deck[i] = deck[rand];
                deck[rand] = temp;

            }
        }
        
        public Card draw()
        {
            temp = this.deck[counter];
			if (counter < deck.Length)
				counter++;
            return temp;
        }
		
		
		//Adds a card to the deck list. 
		public void add(Card card)
		{
			this.add(card);
		}
		

    }
}
