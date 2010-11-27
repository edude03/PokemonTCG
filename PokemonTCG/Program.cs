﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;
using MongoDB;

//Eventually remove all VB code.
using PokemonTCG.ServiceReference2;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;

namespace PokemonTCG
{
    internal class Program
    {

        //Variables
        private static bool gameInPlay = true;
        private static bool playAgain = true;
		private static bool done = false;
        private static Player player1, player2;
		private static string deckpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "decks" + Path.DirectorySeparatorChar;
		private static string deckname;

        private static void Main(string[] args)
        {
            //Use bubble up catches
			try
			{
				Console.WriteLine("Please Select a Game mode");
				Console.WriteLine("(1) Networked");
				Console.WriteLine("(2) Offline Multiplayer");
				Console.WriteLine("(3) Vs Computer");
				string mode;

				bool validMode = false;

				// keep taking user input until a valid mode is selected (ie. validMode == true)
				do
				{
					mode = Console.ReadLine();

					//Might get an Format exception here
					switch ((Enums.gameType)int.Parse(mode))
					{
						case Enums.gameType.Networked:
							Console.WriteLine("Sorry, this feature had yet to be implemented");
							break;
						/* Network Game Pseudo Code
							   * 1) Pick port and ipaddress
							   * 2) See if a connection can be made,
							   * 3) Exchange Player Objects
							   * 4) Start exchanging Lua Scripts
							   */
						case Enums.gameType.Offline:
							validMode = true;
							break;
						case Enums.gameType.Computer:
							Console.WriteLine("Sorry, this feature had yet to be implemented");
							break;
						default:
							Console.WriteLine("Sorry, you must select one of the options.");
							break;
					}
				} while(validMode == false);

				//Get the players name and set them up

				Console.WriteLine("Please enter your name Player 1");
				string input = Console.ReadLine();
				player1 = new Player(input, chainload(PokemonTCG.Program.deckpath));
				setup(player1);
				
				Console.WriteLine("Please enter your name Player 2");
				input = Console.ReadLine();
				player2 = new Player(input, chainload(PokemonTCG.Program.deckpath));
				setup(player2);
				
				//Get the game started.
				gameLoop();
			}
			//TODO: Make the game continue if this is thrown
			catch (NotImplementedException e)
			{
				Console.WriteLine("woops forgot to add code for {0}", e.ToString());
				Console.ReadLine();
			}
			catch (FormatException e)
			{
				Console.WriteLine("Invalid game type. Exception: {0}", e.Message);
				Console.Read();
			}
        }

        public static void gameLoop()
        {
            do
            {
                do
                {
                    TCGMenu(player1, player2);
                    //Incase the game ends while on the first player
                    if (!gameInPlay)
                    {
                        break;
                    }

                    TCGMenu(player2, player1);
                } while (gameInPlay);

                Console.WriteLine("Do you want to play again?");

                string play = Console.ReadLine();

                if (!(play == "yes" || play == "y" || play == "Yes" || play == "Y"))
                {
                    playAgain = false;
                }
				else
				{
					//TODO: Change chainload
					//TODO take into consideration that the other information about a player needs to be considered as well 
					player1 = new Player(player1.getName(), chainload(deckpath));
					player2 = new Player(player2.getName(), chainload(deckpath));
				}
            } while (playAgain);
        }

        private static void attach(Player player1)
        {
            //Choose a card from Hand
            Card temp = player1.chooseCard();

            //Polymorphic 
            if (temp.GetType() == typeof(Energy))
            {
                int add = choosecard(player1.Bench);
                player1.Bench[add].attached.Add(temp);
                temp = null;

            }
            else
            {
                player1.Hand.Add(temp);
            }
        }

        public static void firstTurn(Player player1)
        {
            int choosen = 0;
            Card chsn;
            bool bench = true;
            bool bCont = true;

            // The user wants to put cards in their bench
            while (bench == true)
            {
                Console.WriteLine("Please select a card to put on your bench (" + player1.getName() + ")" + ControlChars.NewLine);
                //choosen = choosecard(player1.Hand);

                if (!(choosen == -1))
                {
                    do
                    {
                        chsn = player1.chooseCard();
                        if (chsn == null || chsn.Stage != "Basic")
                        {
                            Console.WriteLine("The selection was invalid, please slect a basic pokemon");

                            //Put the card back in the hand
                            player1.Hand.Add(chsn);
                        }
                        else
                        {
                            player1.Bench.Add(chsn);
                            bCont = false;
                        }
                    } while (bCont);
                }
                else if (choosen == -1)
                {
                    bench = false;
                    break; // TODO: might not be correct. Was : Exit While
                }
                Console.WriteLine("Would you like to put another card on the bench?" + ControlChars.NewLine);
                string inputCs = Console.ReadLine();

                if (inputCs.ToLower() == "no" | inputCs.ToLower() == "n")
                {
                    bench = false;
                }
            }
            //Tell the player to select an active pokemon

            Console.WriteLine("Please select an active pokemon");

            //Call the overloaded chooseCard method which will iterate the cards in the source (bench)
            player1.setACTPKM(player1.chooseCard(player1.Bench));

            //choosecard method will be launched before the setActPKM command
            player1.isFirstTurn = false;
        }

        public static string chooseDeck(string deckPath)
        {
            string strDeckChoosen = "";
            //string strFileSize = "";
            DirectoryInfo di = new DirectoryInfo(deckPath);

            FileInfo[] aryFi = di.GetFiles("*.csv");

            if (aryFi.Length == 0)
            {
                throw new myExceptions.DeckNotFoundException("No Decks Found!");
            }
            else
            {
                Console.WriteLine(ControlChars.NewLine);
                Console.WriteLine("Please select a deck to load");

                int i = 0;
                for (; i < aryFi.Length; i++)
                {
                    Console.WriteLine(i + ": " + aryFi[i].Name);
                }
                //for future use
                //Console.WriteLine((i + 1) + " : [Randomly Generated]");
				int intDeckChoosen = getValidUserInput(0, aryFi.Length - 1);
				strDeckChoosen = aryFi[intDeckChoosen].Name;

                return strDeckChoosen;
            }
        }

        public static int choosecard(List<Card> source)
        {
            int inputC = -1;
            if (source.Count == 0)
            {
                Console.WriteLine("Sorry! No cards availible");
            }
            else
            {
                Console.WriteLine("Please select a card:");
                for (var i = 0; i <= source.Count - 1; i++)
                {
                    if ((source[i] == null)) continue;
                    Console.Write(i + ": " + source[i].Name);
                    switch (source[i].Stage)
                    {
                        case "Basic":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "Stage 1":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "Stage 2":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case "Trainer":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case "Energy":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }
                    Console.WriteLine(" " + source[i].Stage);
                    Console.ResetColor();
                }
				inputC = getValidUserInput(0, source.Count - 1);
            }
            return inputC;
        }
		
		public static void printCards(List<Card> input)
		{
			int i = 0;
			foreach (Card c in input)
			{
				i++;
				Console.Write("{0}: {1}", i, c.Name);
			}
		}
		
		public static void check(Card c)
		{
			
		}
		
		public static string getStatus(Card c)
		{
			string status = c.Name + " || HP: " + c.HP + " Status: " + c.Status;  
			return status;
		}
		
		public static void menu_Hand(Player curPlayer)
		{
			//Show the user all of their cards.
				printCards(curPlayer.Hand);
			
			//The user can chose any of there cards
				int chosen = getValidUserInput(1, curPlayer.Hand.Count); //Number of cards in your hand lol
			//If the user chooses a card
				//If the card is a basic pokemon 
			
				if (curPlayer.Hand[chosen].stage == Enums.Stage.Basic)
				{
					//Allow user to check, put on bench or go back
					Console.WriteLine("1. Check \n 2. Bench 3. \n Back \n");
					switch (getValidUserInput(1,3))
					{
						case 1:
						check(curPlayer.Hand[chosen]);
						break;
						
						case 2:
						curPlayer.move(curPlayer.Hand, chosen, curPlayer.Bench);
						break;
					
						case 3: break;
					}
				}
				else if (curPlayer.Hand[chosen].stage == Enums.Stage.Trainer)
				{
					Console.WriteLine("Sorry Trainers haven't been implemented yet");
				}
				else if (curPlayer.Hand[chosen].stage == Enums.Stage.Energy)
			    {
					//If this is the first energy attached
					if (curPlayer.isFirstEnergy == true)
					{
						//Allow the user to attach the card to a Pokemon
						Console.WriteLine("Please pick a pokemon to attach this to");
						printCards(curPlayer.Bench); //<-- TODO: Code something incase there is nothing in the bench or other card source
						curPlayer.isFirstEnergy = false;
					}
					else
					{
						//Tell them they can't do it and go back
						Console.WriteLine("Sorry, you can only attach one energy per turn");
					}
				}
				//If it is some other card
				else
				{
					//Check if it is an evolution and allow the user to change it out?
					Console.WriteLine("Sorry, I'm still working on evolutions as well ;_;"); //TODO: Write evolution code 
				}
			
		}

        public static void TCGMenu(Player curPlayer, Player player2)
        {
			do {
			done = false; //The menu loops until the player is done. 
				
            Console.WriteLine("1. Hand\t 2. Check\t 3.Retreat");
            Console.WriteLine("4. Attack\t 5.Pkmn Power\t 6.Done");
			switch (getValidUserInput(1,6))
            {
                case 1: //If the user picks Hand
				menu_Hand(curPlayer);
				break;
	
				case 2: //If the user picks Check
					//Show the user data on whatever card they choose?
				break;
					
				case 3: //If the user picks Retreat
					//Check if the user can ever retreat
					if (curPlayer.retreat()) //If the retreat is sucsessful 
					{
						Console.WriteLine("Card retreated, please pick a new active pokemon");
						//Chose a card from player 1s hand 
						curPlayer.makeActPkm(curPlayer.Bench, choosecard(curPlayer.Bench));
					}
					else
					{
						Console.WriteLine("Sorry, you don't have enough energies to retreat");
					}
				break;
				
				case 4: //If the user picks Attack
					//Make sure they have an active pokemon
				if (curPlayer.actPkm == null)
				{
					//if the user doesn't have an active pokemon
							//Tell them they are a dumbass
					Console.WriteLine("You need to have an active pokemon to attack");
				}
				else
				{
					//Let the user pick an attack
					Console.WriteLine("0: {0}, Damage: {1}", curPlayer.actPkm.atk[0].name, curPlayer.actPkm.atk[0].damage);
                    if (curPlayer.actPkm.atk[1] != null)
                    {
                       Console.WriteLine("1: {0}, Damage: {1}", curPlayer.actPkm.atk[1].name, curPlayer.actPkm.atk[1].damage);
                    }
                    int chosen = getValidUserInput(1, 2);
                    if (curPlayer.actPkm.meetsCrit(curPlayer.actPkm.atk[chosen].requirements))
                    {
                        //They have enough energies execute that b**ch
                        player1.actPkm.HP -= player1.actPkm.getAttack(chosen);
                        done = true;
                    }
                    else
                    {
                        //Remind the user that they were an accident in life
                        Console.WriteLine("Sorry you don't have enough energies to execute this attack");
                    }

                    /* I like this code sinpette for some reason.
					// if (atk[1] != null) { atk = getValidUserInput(0, 1)} else {atk = getValidUserInput(0, 0)}
						int atk = curPlayer.actPkm.atk[1] != null ? getValidUserInput(0, 1) : getValidUserInput(0, 0);
						*/ 	    
				}

					
					//Check if they have enough energies to make the attack
						//If they do execute the attack
					
						//End their turn 
						//else tell them they are a dumbass and go back to the menu
				break;
				
				case 5: //Pokemon Power 
				//Not Implemented Yet
				break;
				
				case 6: //Done
                done = true;
				break;               
			} 
		} while (!done);
		}

        public static void setup(Player currentPlayer)
        {
            //Shuffle the opponets deck
			currentPlayer.shuffleDeck();

            //Draw seven cards
			currentPlayer.draw(7);
			
			//Place Prizes
			currentPlayer.initPrizes(6);
			
			//--For now, right here the "firstTurn" code is run
			
			//TODO: Move first turn code over here. 
            //Pick a basic pokemon (Hand to active)
           

            //Up to 5 basic pkm on the bench

            

            //Flip coin
            //--Heads player1, Tails Player2
        }

        public static int[] LoadDeck(string deckpath, string deckname)
        {
            int[] returnError = { -1 };

            try
            {
                //Loop counters
                int i = 0;
                int j = 0;

                int numRecords = 0;
                int invalid = 0;

                const int MIN = 1;
                const int MAX = 4086;

                //To read the number of records
                StreamReader objReader = new StreamReader(deckpath + deckname);
                string strContents = objReader.ReadToEnd();
                objReader.Close();
                string[] itemsRead = strContents.Split(new char[] { ',' });

                numRecords = itemsRead.Length;

                int temp;
                List<int> intLoadDeck = new List<int>();

                foreach (string s in itemsRead)
                {
                    temp = int.Parse(s);
                    if (!(temp < MIN | temp > MAX))
                    {
                        intLoadDeck.Add(temp);
                    }
                }

                return intLoadDeck.ToArray();
            }
            catch
            {
                return returnError;
            }
        }

		/// <summary>
		/// Makes sure the input is within the bounds. 
		/// </summary>
		/// <param name="lowerBounds">
		/// A <see cref="System.Int32"/> equal to the lowest accepable value
		/// </param>
		/// <param name="upperBounds">
		/// A <see cref="System.Int32"/> equal the the highest acceptable value
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> equal to th input (assuming it's valid) 
		/// </returns>
		public static int getValidUserInput(int lowerBounds, int upperBounds)
		{
			bool validInput = false;
			int intOut = 0;

			do // do until valid input is entered.
			{
				try
				{
					int intInput = int.Parse(Console.ReadLine());
					if (intInput <= upperBounds && intInput >= lowerBounds)
					{
						intOut = intInput;
						validInput = true;
					}
					else
						Console.WriteLine("Input \"{0}\" is invalid! Please try again.", intInput);
				}
				catch (FormatException e)
				{
					Console.WriteLine(e.Message + "\nPlease try again.");
				}
				catch (OverflowException e)
				{
					Console.WriteLine(e.Message + "\nPlease try again.");
				}
			} while (!validInput);

			return intOut;
		}
		
		private static Card[] intArrayDeck(int[] intDeck)
		{
			int Size = intDeck.Length;
			Card[] deck = new Card[Size];
			
			//TODO: If mongo can't connect for some reason find out why and correct the issue,
			//To improve performance and to ensure that we don't run out of connections 
			//A universal connection is created outside the loop, then closed after the deck is created. 
			//Instanate the connection 
			Mongo mongo = new Mongo();
			mongo.Connect();
			
            for (int k = 0; k < Size; k++)
            {
                deck[k] = getCardFromDB(intDeck[k], mongo);
            }
			
			//Close the connection
			mongo.Disconnect();
			
			return deck;
		}
		
		public static Card getCardFromDB(int BOGUS_ID, Mongo mongo) 
		{
			Document query = new Document();
			query["BOGUS_ID"] = BOGUS_ID;
			Document results = mongo["pokemon"]["cards"].FindOne(query);
			
			//TODO: Convert the strings to their proper type internally 
			string Name = results["Name"].ToString();
			string Stage = results["Stage"].ToString();
			string Type = results["Type"].ToString();
			Enums.Element type = parseType(Type);
			Enums.Stage stage = parseStage(Stage);
			int HP = 0;
			string Weakness = "";
			string Resistance = "";
			Attack[] atk = new Attack[2];

			
			if (type != Enums.Element.Trainer && type != Enums.Element.Energy)
			{
				HP = int.Parse((results["HP"] ?? 0).ToString());
				Weakness = results["Weakness"].ToString();
				Resistance = results["Resistance"].ToString();
				
				
				//TODO: Requirements (energies) 
				if (!(results["Attack1"].ToString() == string.Empty)) //If there is an attack 1
				{
					atk[0] = new Attack(results["Attack1"].ToString());
					atk[0].damage = results["TypicalDamage1"].ToString();
				}
					                         
				if (!(results["Attack2"].ToString() == string.Empty))
			    {
					atk[1] = new Attack(results["Attack2"].ToString());
					atk[1].damage = (results["TypicalDamage2"].ToString());
				}
			}
			//Make sure to fix card to correct this issue. 
			return new Card(BOGUS_ID, Name, HP, Stage, Weakness, Resistance, Type, atk); 
		}
		
		//This is to keep the code working the same way until I get around to abstracting the Deck Card and player instanation methods. 
		//TODO: Fix this code. (maybe make it an option?)
		private static Card[] chainload(string deckpath)
		{
			//Choose a CSV File -> Parse the CSV -> Convert the int array to card objects -> return card array.
			return intArrayDeck(LoadDeck(deckpath, chooseDeck(deckpath)));
			
		}
		
		private static Enums.Stage parseStage(string strStage)
		{
			Enums.Stage stage;
			switch (strStage)
			{
				case "Stage 1": stage = Enums.Stage.Stage1; break;
				case "Stage 2": stage = Enums.Stage.Stage2; break;
				case "Baby": stage = Enums.Stage.Baby; break;
				case "Basic": stage = Enums.Stage.Basic; break;
				default: if (strStage.StartsWith("Trainer"))
					{
						stage = Enums.Stage.Trainer;
					}
					else if (strStage.Contains("Energy"))
					{
						stage = Enums.Stage.Energy;
					}
					else if (strStage.Contains("Level Up"))
					{
						throw new myExceptions.InvalidDeckException("Unhandled card type loaded! (\"Level Up\")");
					}
					else
					{
						Console.WriteLine("What?");
						throw new myExceptions.InvalidDeckException("An invalid card was loaded");
					}
					break;
			}
			return stage;
		}
		
		private static Enums.Element parseType(string strType)
		{
			Enums.Element type;
			// some database entries have multiple space delimited types; only accept first type
					string[] tmpStrArr = strType.Split(" ".ToCharArray());
					string firstType = tmpStrArr[0];

					switch (firstType)
					{
						case "Trainer":
							type = Enums.Element.Trainer;
							break;
						case "Energy":
							type = Enums.Element.Energy;
							break;
						case "Colorless":
							type = Enums.Element.Colorless;
							break;
						case "Bug":
							type = Enums.Element.Bug; // "bug" type does not exist in database provided
							break;
						case "Darkness":
							type = Enums.Element.Dark;
							break;
						case "Lightning":
							type = Enums.Element.Electric;
							break;
						case "Fighting":
							type = Enums.Element.Fight;
							break;
						case "Fire":
							type = Enums.Element.Fire;
							break;
						case "Flying":
							type = Enums.Element.Flying; // "flying" type does not exist in database provided
							break;
						case "Ghost":
							type = Enums.Element.Ghost; // "ghost" type does not exist in database provided
							break;
						case "Grass":
							type = Enums.Element.Grass;
							break;
						case "Water":
							type = Enums.Element.Water;
							break;
						case "Steel":
							type = Enums.Element.Steel; // "steel" type does not exist in database provided
							break;
						case "Rock":
							type = Enums.Element.Rock; // "rock" type does not exist in database provided
							break;
						case "Psychic":
							type = Enums.Element.Psychic;
							break;
						case "Poison":
							type = Enums.Element.Poison; // "poison" type does not exist in database provided
							break;
						case "Normal":
							type = Enums.Element.Normal; // "normal" type does not exist in database provided
							break;
						case "Metal":
							type = Enums.Element.Metal;
							break;
						case "Ground":
							type = Enums.Element.Ground; // "ground" type does not exist in database provided
							break;
						case "Ice":
							type = Enums.Element.Ice; // "ice" type does not exist in database provided
							break;
						default:
							throw new myExceptions.InvalidEnergyTypeException("Invalid energy type: \"" + strType + "\"");
					}
			return type;
		}
    }
}