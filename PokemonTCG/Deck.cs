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
        Random rnd = new Random();
        private Card[] deck;

        //Variables
        int counter = 0;
        private Card temp;
        private int[] intDeck;

        /// <summary>
        /// Number of items in the deck
        /// </summary>
        public int Size;

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
            

            intDeck = Program.LoadDeck2(deckpath, deckname);
            this.Size = intDeck.Length;
            
            #region olddeck parser
            /*

            try
            {

                //ByVal deckname As String, ByRef PlayersDeck As Array, extra code
                //Get values from the deck.csv file and load them into the myplayer.deck array using a for loop
                //Writing this because the current code I've seen is useless :|



                string strContents = null;
                StreamReader objReader = default(StreamReader);
                int[] returnError = { -1 };
                int[] dlength = { 0 };
                int i = 0;
                

                
                    objReader = new StreamReader((deckpath + deckname));
                    strContents = objReader.ReadToEnd();
                    objReader.Close();
                    char del = ',';
                    //Parse thru the items read in
                    string[] itemsRead = strContents.Split(del);
                    //Console.Write("Deck Import complete")
                    //Console.Write(ControlChars.NewLine)

                    //Since we can assume that only numbers will be imported then we should force the return of an integer array :D 
                    int[] intLoadDeck = new int[Information.UBound(itemsRead,1) + 1];

                    //Filter out inappropriate values, and parse the correct values into the array.
                    //int intItemsRead = 0;
                    this.Size = itemsRead.Length - 1;

                    if (!(Information.UBound(itemsRead,1) <= 0))
                    {
                        //Do until we have parsed 60 cards (the max for a deck) or 
                        while (!(((i == 60) | (i == itemsRead.Length))))
                        {
                            if (int.Parse(itemsRead[i]) > 4086)
                            {
                                
                                
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("The Value " + itemsRead[i] + " Cannot be used an is invalid");
                                    Console.ResetColor();
                                
                            }
                            else if (int.Parse(itemsRead[i]) <= 0)
                            {
                               
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("The Value " + itemsRead[i] + " Cannot be used an is invalid");
                                    Console.ResetColor();
                                
                            }
                            else
                            {
                                intLoadDeck[i] = int.Parse(itemsRead[i]);
                            }
                            i += 1;
                            //Needed because its not a forloop therefore i doesn't auto increment
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Sorry, this deck cannot be used as it has less than 1 card");
                        Console.ResetColor();
                        
                    }


                    //Since zeros get put in where data cannot be loaded
                    //we will do a final check of the array just to be sure
                    int firstI = 0;
                    int j = 0;

                    //Step 1: Loop thru the deck to find a position with a zero, then replace it with the first position without a zero
                    //until all the zeros are at the end of the array
                    for (i = 0; i <= (Information.UBound(intLoadDeck,1) - 1); i++)
                    {
                        if (intLoadDeck[i] == 0)
                        {
                            firstI = i;
                            j = i;
                            while (!(intLoadDeck[j] > 0 | j == Information.UBound(intLoadDeck,1)))
                            {
                                j = j + 1;
                            }
                            intLoadDeck[firstI] = intLoadDeck[j];
                            intLoadDeck[j] = 0;
                        }
                    }

                    //Step 2: Alright, now that all the zeros are at the end of the array, make sure they disapear, 
                    //this is done by find the first element without a zero, and shrinking the array to that location
                    j = Information.UBound(intLoadDeck,1);
                    while (!(intLoadDeck[j] > 0))
                    {
                        j --;
                    }
                    if (j + 1 < Information.UBound(intLoadDeck,1))
                    {
                       
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ControlChars.NewLine);
                            Console.WriteLine("The deck contained a number of invalid values, or was too large therefore the deck has been shrunk to " + j + 1 + " cards.");
                        
                        Array.Resize(ref intLoadDeck, j+1);
                        this.Size = j+1;
                    }
                    intDeck = intLoadDeck;
                    deck = new Card[this.Size];

                    for (int k = 0; k < Size; k++)
                    {
                        this.deck[k] = new Card(this.intDeck[k]);
                    }

                   
                }


                catch (System.FormatException Ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Sorry, the deck you have selected is invalid, please select a different one");
                    Console.ResetColor();
                    throw new myExceptions.InvalidDeckException();
                }
             * 
             * */
            #endregion
            //Init the cards to their proper values

            deck = new Card[this.Size];
			
			//TODO: If mongo can't connect for some reason find out why and correct the issue,
			//To improve performance and to ensure that we don't run out of connections 
			//A universal connection is created outside the loop, then closed after the deck is created. 
			
			//Instanate the connection 
			Mongo mongo = new Mongo();
			mongo.Connect();
			
            for (int k = 0; k < Size; k++)
            {
                this.deck[k] = new Card(this.intDeck[k], mongo);
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

    }
}
