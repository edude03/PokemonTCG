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

        
        private Card temp;
        private string deckpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "decks" + Path.DirectorySeparatorChar;
		private string deckname;
        public Deck deck;
        public bool isFirstTurn = true;
        public bool pickPKM = false;
		public bool isFirstEnergy = true;
        Game gInstance; 

        //Player may get a key section for encrypting and decryting keys
     
        //Methods
        //Constructor
		public Player(string name, Card[] deck, Game gInst)
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
        public Player(string name, Game gInst)
        {
            //Set up the properties for the Lists
           
            this.name = name;

            //Choose A Deck file
			try
			{
				this.deckname = gInstance.chooseDeck(deckpath);
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
		
        /// <summary>
        /// Takes the top input from the deck and returns it to the calling method.
        /// </summary>
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
        /// Shuffles the deck
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
            bool tmpReturn = deck.draw(out draw);
            return tmpReturn;
        }

        //Overloaded because eventually you should be able to draw your own prize
        #region drawPrize
        /// <summary>
		/// Draws a perticular prize. 
		/// </summary>
		/// <param name="i">The index of the prize you want to draw</param>
		/// <returns>The selected card</returns>
        public Card drawPrize(int i)
		{
			//Store the input safely befoe we remove it
			Card temp = this.Prizes[i];
			
			//Remove the input from the prizes
			this.Prizes.RemoveAt(i); 
			
			//Return it to the calling method
			return temp;
		}
		
        /// <summary>
        /// Draws a card from the prizes
        /// </summary>
        /// <returns>The next prize</returns>
		public Card drawPrize()
		{
			//Returns the first input in prizes. 
			return drawPrize(0); 
		}
        #endregion

        /// <summary>
        /// Returns the last input that was added to the List so that information 
        /// may be extracted from it
        /// </summary>
        /// <returns>The last input drawn</returns>
        public Card getLastCard()
        {
            return (Card)Hand[Hand.Count -1];
        }

        /// <summary>
        /// Puts the input that was passed in into the discarded array.
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
        /// Allows the user to choose a input from the hand List
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
        /// <returns>A input</returns>
        public Card chooseCard(List<Card> source)
        {
            /* Get choosecard to show us the cards in Hand,
             * use the index it returns to get that input and store it temporarly
             * Remove that input from hand and return it to the calling method
             */

            int intTemp = gInstance.choosecard(source);
            temp = source[intTemp];
            source.RemoveAt(intTemp);
            return temp;
        }

        /// <summary>
        /// Check if the input the calling method wants is valid,
        /// if it is, the input is removed from the source list and
        /// returned
        /// </summary>
        /// <param name="index">The index of the input in question</param>
        /// <returns>The chosen input or null if there is an error</returns>
        public Card chooseCard(int index)
        {
            //Check if the input it wants is valid
            if (Hand[index] != null)
            {
                temp = Hand[index];
                this.Hand.RemoveAt(index);
                return temp;
            }
            else
            {
                //Someway to tell the calling method this is not vaild
                //Since the contrustor isn't called, it will return a input that is null
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
		
		//This method seems useless however it is also used for active pokemon which is a input type instead of a list
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
					move(actPkm.attached, 0, Discarded); //Move the first input from the attached array to discarded. 
				}
				
				//Move the input to the bench
				move(actPkm, Bench); 
				actPkm = null;
				
				return true; //<-- The operation was sucsessful 
			}
			else
			{
				return false; //<-- The requirements for the retreat cost were not met
			}
				
		}
		
		public void setActPkm(List<Card> input, int index)
		{
			this.actPkm = input[index];
			input.RemoveAt(index);
		}
		
		public void attachEnergy(Card c, List<Card> source)
		{
			//Predicate is a method that will return true or flase based on the passed in input
			Predicate<Card> energy = new Predicate<Card>(isEnergy);
			//If an energy input exists in the source:
			if(source.Exists(energy))
			{
				//Keeps track of the index of the input in the array 
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
				chosen = gInstance.getValidUserInput(0,j);
				move(source, chosen, c.attached);
			}
			else
			{
				Console.WriteLine("You have no enegeries in your hand");
			}
		}

        //Predicates 
        #region Predicates
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
        #endregion

        //Evolution related code follows
        #region Evolution_Code
        /// <summary>
        /// Checks that the evolution is valid
        /// </summary>
        /// <param name="from">The card that is in play</param>
        /// <param name="to">The card you want to evolve to</param>
        /// <returns>True or false</returns>
        public bool validEvol(Card from, Card to)
        {
            foreach (int i in to.evolvesFrom)
            {
                if (i == from.pNum)
                    return true;
            }
            //if none of the values of evolves from match the pokemon number of the input. 
            return false;
        }

        /// <summary>
        /// disposes of the cards that were in play before the evolution. 
        /// </summary>
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
            //First: Move Attachments from old input to new input
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

            //Second: Move old input to evoZone (A zone that exists under the top (activePKM) input
            move(this.actPkm, evoZone);

            //Third: Make 'to' the activePKM
            setActPkm(source,sIndex);

            //Forth: Bind the cleanup event. 
            //If the PKMFaint doesn't have any events bound to it, bind PKMFaint.
            //This needs to be fixed because if another event has bound to faint then the evoZone won't get cle
            if (PKMFaint == null) //TODO: do a proper check since other methods can bind PKMFaint other than cleanup
            {
                PKMFaint += new EventHandler(Player_PKMFaint);
            }
        }

        void Player_PKMFaint(object sender, EventArgs e)
        {
            doEvolCleanUp();
        }
        #endregion
    }
}
