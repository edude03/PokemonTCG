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
		

		//Turn use variables
		public Card actPkm;
        public Boolean isTurn;
		public bool hasDrawn; 
		
        private string name;
		
        //Evolution use code
        private List<Card> evoZone;

        //Events 
        public event EventHandler PKMFaint;

        
        private int counter = 0;
        private Card temp;
        private string deckpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "decks" + Path.DirectorySeparatorChar;
		private string deckname;
        public Deck deck;
        public bool isFirstTurn = true;
        public bool pickPKM = false;
		public bool isFirstEnergy = true;

        //Player may get a key section for encrypting and decryting keys
     
        //Methods
		
		public Player(string name, Card[] deck)
		{
			this.name = name;
            this.deck = new Deck();
            this.deck.AddRange(deck);
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
            //this.deck = new Deck(deckpath, deckname);
            
            //Make sure the deck is nice and shuffled
            for (int i = 0; i < 7; i++)
            {
                //deck.Shuffle();
            }
            
            //Need to draw 7 cards for the hand
            //If there are less than 7 cards in the deck
            if (deck.Count < 7)
            {
                for (int i = 0; i < deck.Count; i++)
                {
                    //Draw all the cards in the deck
                    Card tmp;
                    this.draw(out tmp);
                    this.Hand.Add(tmp);
                }
            }
            //Otherwise,
            else
            {

                for (int i = 0; i < 7; i++)
                {
                    Card tmp;
                    this.draw(out tmp);
                    this.Hand.Add(tmp);
                }
            }
        }
       
		
		public void draw(int n)
		{
		  for (int i=0; i < n; i++)
		  {
              Card tmp;
              this.draw(out tmp);
              this.Hand.Add(tmp);
		  }
		}
		

        /// <summary>
        /// Takes the top card from the deck and passes it to the calling method.
        /// </summary>
       
		
		public void shuffleDeck()
		{
			deck.shuffle();
		}

        /// <summary>
        /// Calls the deck 
        /// </summary>
        /// <returns></returns>
        public bool draw(out Card draw)
        {
            bool tmpReturn = this.draw(out draw);
            return tmpReturn;
        }
		//Overloaded because eventually you should be able to draw your own prize
		public Card drawPrize(int i)
		{
			//Store the card safely befoe we remove it
			Card temp = this.Prizes[i];
			
			//Remove the card from the prizes
			this.Prizes.RemoveAt(i); 
			
			//Return it to the calling method
			return temp;
		}
		
		
		public Card drawPrize()
		{
			//Returns the first card in prizes. 
			return drawPrize(0); 
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
        public void discard(List<Card> source, int index)
        {
			//Hows this for DRY? 
            move(source, index, Discarded);
        }
		
		public void discard(Card c)
		{
			//Probably passing in the actPKM in this case
			move(c, Discarded);
		}

        public void setACTPKM(Card card)
        {
            this.actPkm = card;
        }

		//This is a repeat of makeActPKM?	
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
            this.deck.AddRange(Discarded);
            Discarded.Clear();
		}
		
		//Add randomly drawn prizes to the Prizes list. 
		public void initPrizes(int numOfPrizes)
		{
			for (int i = 0; i < numOfPrizes; i++)
			{
                Card tmp;
                if (deck.draw(out tmp))
                {
                    Prizes.Add(tmp);
                }
                else
                {
                    //TODO" something if they are out of cards 
                }
			}
		}
		
		//This method seems useless however it is also used for active pokemon which is a card type instead of a list
		//Therefore it's usefull there. 
		public void move(Card c, List<Card> dest)
		{
			dest.Add(c);
			c = null; //Are objects passed by ref in C#? //Todo: find out
		}
		
		public void move(List<Card> source, int index, List<Card> dest)
		{
			move(source[index], dest);
			source.RemoveAt(index);
		}
		
		
		//Realized you can only retreat the actPKm so no point having parameters :P
		public bool retreat() //Todo: Fix this method; it assumes that the only thing that can be attached is energies. 
		{					  
			//Check if the retreat cost can be met
		 	if (actPkm.attached.Count > actPkm.retreatCost)
			{
				//If it can remove the number of cards from the attached
				for (int i = 0; i < actPkm.retreatCost; i++)
				{
					move(actPkm.attached, 0, Discarded); //Move the first card from the attached array to discarded. 
				}
				
				//Move the card to the bench
				move(actPkm, Bench); 
				actPkm = null;
				
				return true; //<-- The operation was sucsessful 
			}
			else
			{
				return false; //<-- The requirements for the retreat cost were not met
			}
				
		}
		
		public void makeActPkm(List<Card> input, int index)
		{
			this.actPkm = input[index];
			input.RemoveAt(index);
		}
		
		
		public void attachEnergy(Card c, List<Card> source)
		{
			//Predicate is a method that will return true or flase based on the passed in input
			Predicate<Card> energy = new Predicate<Card>(isEnergy);
			//If an energy card exists in the source:
			if(source.Exists(energy))
			{
				//Keeps track of the index of the card in the array 
				int i = 0;
				//Keeps track of the number of energies in the players hand.
				int j = 0;
				//Takes the return from getValidInput();
				int chosen = 0;
				Console.WriteLine("Pick an energy:");
				foreach (Card lCard in source) //I know that is a weird variable name but I was hoping to keep it from being confusing
				{
					i++;
					if(isEnergy(lCard))
					{
						j++;
						Console.WriteLine("{0}: {1}", i, lCard.Name);
					}
				}
				chosen = Program.getValidUserInput(0,j);
				move(source, chosen, c.attached);
			}
			else
			{
				Console.WriteLine("You have no enegeries in your hand");
			}
		}
		
		//Predicates 
		
		public bool isBasic(Card c)
		{
			return (c.stage == Enums.Stage.Basic);
		}
		
		public bool isStage(Card c, Enums.Stage stage)
		{
			//Return whatever this expression evaluates to (true or false)
			return (c.stage == stage);
		}
			              
		private bool isEnergy(Card c)
		{
			if (c.stage == Enums.Stage.Energy)
			{
				return true;
			}
			//Else
				return false; 
		}


        //Evolution related code follows 
        public bool validEvol(Card from, Card to)
        {
            foreach (int i in to.evolvesFrom)
            {
                if (i == from.pNum)
                    return true;
            }
            //if none of the values of evolves from match the pokemon number of the card. 
            return false;
        }

        private void doEvolCleanUp()
        {
            //All the cards that were previously put in the evoZone need to go to disposed
            for (int i = 0; i < evoZone.Count; i++)
            {
                discard(evoZone, 0);
            }
        }

       
        //Really since you can only evolve from the active pokemon, only source and index are needed?
        public void doEvol(List<Card> source, int sIndex)
        {
            //First: Move Attachments from old card to new card
            //If there are cards attached
            if (this.actPkm.attached.Count > 0)
            {
                //Move Attachments
                for (int i = 0; i < this.actPkm.attached.Count; i++)
                {
                    //Call that super handy move method :P
                    move(this.actPkm, evoZone);
                }
            }

            //Second: Move old card to evoZone (A zone that exists under the top (activePKM) card
            move(this.actPkm, evoZone);

            //Third: Make 'to' the activePKM
            setACTPKM(source[sIndex]);

            //Forth: Bind the cleanup event. 
            if (PKMFaint == null) //TODO: do a proper check since other methods can bind PKMFaint other than cleanup
            {
                PKMFaint += new EventHandler(Player_PKMFaint);
            }
        }

        void Player_PKMFaint(object sender, EventArgs e)
        {
            doEvolCleanUp();
        }
  }
}
