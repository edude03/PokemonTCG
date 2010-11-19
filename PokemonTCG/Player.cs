using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;


namespace PokemonTCG
{
    [XmlRootAttribute("Player", IsNullable=false)]
    public class Player
    {   
        ///Card arrays, write encapsulated methods for possibly
        public List<Card> Hand = new List<Card>();
        public List<Card> Bench = new List<Card>(5);
        public List<Card> Discarded = new List<Card>(60);
		public List<Card> Prizes = new List<Card>(6);


        public Card actPkm;
        public Boolean isTurn;

        private string name;

        
        private int counter = 0;
        private Card temp;
        private string deckpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "decks" + Path.DirectorySeparatorChar;
		private string deckname;
        private Deck deck;
        public bool isFirstTurn = true;
        public bool pickPKM = false;

        //Player may get a key section for encrypting and decryting keys
     
        //Methods
		
		public Player(string name, Card[] deck)
		{
			this.name = name;
			this.deck = new Deck(deck);
		}

        /// <summary>
        /// Creates a new "player", sets the name, Loads the players deck
        /// Shuffles it 7 times, and adds 7 cards to it
        /// </summary>
        /// <param name="name">The Name of the player</param>
        public Player(string name)
        {
            //Set up the properties for the Lists
           
            this.name = name;

            //Choose A Deck file
			try
			{
				this.deckname = Program.chooseDeck(deckpath);
			}
			catch (myExceptions.DeckNotFoundException e)
			{
				Console.WriteLine(e.CustomMessage);
			}

            //Instanate the deck
            this.deck = new Deck(deckpath, deckname);
            
            //Make sure the deck is nice and shuffled
            for (int i = 0; i < 7; i++)
            {
                deck.Shuffle();
            }
            
            //Need to draw 7 cards for the hand
            //If there are less than 7 cards in the deck
            if (deck.Size < 7)
            {
                for (int i = 0; i < deck.Size; i++)
                {
                    //Draw all the cards in the deck
                    this.Hand.Add(deck.draw());
                }
            }
            //Otherwise,
            else
            {

                for (int i = 0; i < 7; i++)
                {
                    //Draw 7 cards
                    this.Hand.Add(deck.draw());
                }
            }
        }
       
		
		public void draw(int n)
		{
		  for (int i=0; i < n; i++)
		  {
			this.Hand.Add(deck.draw());
		  }
		}
		

        /// <summary>
        /// Takes the top card from the deck and passes it to the calling method.
        /// </summary>
       
	/*		
	  public void draw(int n)
        {
            if (n == 1)
            {

            }
            for (int i = 0; i < n; i++)
            {
                this.Hand[counter] = deck.draw();
                counter++;
            }
            
        }
        */
		
		public void shuffleDeck()
		{
			deck.Shuffle();
		}

        /// <summary>
        /// Calls the deck 
        /// </summary>
        /// <returns></returns>
        public Card draw()
        {
            return deck.draw();
        }
        
        /// <summary>
        /// Returns the last card that was added to the List so that information 
        /// may be extracted from it
        /// </summary>
        /// <returns>The last card drawn</returns>
        public Card getLastCard()
        {
            return (Card)Hand[Hand.Count -1];
        }

        /// <summary>
        /// Puts the card that was passed in into the discarded array.
        /// </summary>
        public void discard(Card card)
        {
            throw new System.NotImplementedException();
        }

        public void setACTPKM(Card card)
        {
            this.actPkm = card;
        }


        public void addCard(Card card)
        {
            this.Hand.Add(card);
        }

        /// <summary>
        /// Removes all cards from the hand List
        /// </summary>
        public void DiscardHand()
       {
           Hand.Clear();
       }

        /// <summary>
        /// Ecapsulated method
        /// </summary>
        /// <returns>The players name</returns>
        public string getName()
        {
            return this.name;
        }

        /// <summary>
        /// Allows the user to choose a card from the hand List
        /// </summary>
        /// <returns>The choosen Card</returns>
        public Card chooseCard()
        {
            return this.chooseCard(this.Hand);
        }

        /// <summary>
        /// Presents a menu with the availible cards in the source list
        /// </summary>
        /// <param name="source">A List</param>
        /// <returns>A card</returns>
        public Card chooseCard(List<Card> source)
        {
            /* Get choosecard to show us the cards in Hand,
             * use the index it returns to get that card and store it temporarly
             * Remove that card from hand and return it to the calling method
             */

            int intTemp = Program.choosecard(source);
            temp = source[intTemp];
            source.RemoveAt(intTemp);
            return temp;
        }

        /// <summary>
        /// Check if the card the calling method wants is valid,
        /// if it is, the card is removed from the source list and
        /// returned
        /// </summary>
        /// <param name="index">The index of the card in question</param>
        /// <returns>The chosen card or null if there is an error</returns>
        public Card chooseCard(int index)
        {
            //Check if the card it wants is valid
            if (Hand[index] != null)
            {
                temp = Hand[index];
                this.Hand.RemoveAt(index);
                return temp;
            }
            else
            {
                //Someway to tell the calling method this is not vaild
                //Since the contrustor isn't called, it will return a card that is null
                return null;
            }
        }
		
		//It's supposed to set the game back to stock
		//TODO: Write this method
		public void reset()
		{
			//Move all the cards from the Deck 
			while (Discarded[0] != null)
			{
				this.deck.add(Discarded[0]);
			}
			
		}
		
		//Add randomly drawn prizes to the Prizes list. 
		public void initPrizes(int numOfPrizes)
		{
			for (int i = 0; i < numOfPrizes; i++)
			{
				Prizes.Add(deck.draw());
			}
		}
		
  }
}
