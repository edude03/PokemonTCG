using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;

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
        private static Player player1, player2;


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
				player1 = new Player(input);

				Console.WriteLine("Please enter your name Player 2");
				input = Console.ReadLine();
				player2 = new Player(input);
				
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
                    menu(player1, player2);
                    //Incase the game ends while on the first player
                    if (!gameInPlay)
                    {
                        break;
                    }

                    menu(player2, player1);
                } while (gameInPlay);

                Console.WriteLine("Do you want to play again?");

                string play = Console.ReadLine();

                if (!(play == "yes" || play == "y" || play == "Yes" || play == "Y"))
                {
                    playAgain = false;
                }
            } while (playAgain);
        }

        public static void menu(Player player1, Player player2)
        {
            bool draw = true;
            Card mycard;

            if (player1.isFirstTurn)
            {
                firstTurn(player1);
            }
            Console.WriteLine("ACK: {0} ||  HP: {1}, || Status: {2}", player1.actPkm.Name,
                              (player1.actPkm.HP).ToString(), player1.actPkm.Status);
            if (draw)
            {
                player1.draw();
                draw = false;
            }


            Console.WriteLine("{0} Drew {1}", player1.getName(), player1.getLastCard().Name);
            Console.ResetColor();
            Console.WriteLine("\r\n");
            Console.WriteLine("[{0}]", player1.getName());
            Console.Write("1. Hand\t\n");
            Console.Write("2. Check\r\n");
            Console.Write("3. Hand\r\n");
            Console.Write("4. Retreat\r\n");
            Console.Write("5. Pkm Power\r\n");
            Console.Write("6. Lookup **demo function**\r\n");
            Console.Write("7. Done\r\n");
            Console.Write("8. Attach\n");
            Console.Write("Enter selection here: ");
			int inputC = getValidUserInput(1, 8);
            Console.Write("\r\n");

            switch (inputC)
            {
                case 1:
                    {
                        if (player1.actPkm.Name == null)
                        {
                            Console.WriteLine("Please play an active pokemon");
                            player1.setACTPKM(player1.chooseCard());

                            //If its not a basic pokemon,
                            if ((player1.actPkm.Stage != "Basic") | (player1.actPkm.Stage == "Trainer"))
                            {
                                //Force the user to choose a nother card
                                Console.Write("Sorry that selection is invalid");
                                player1.setACTPKM(player1.chooseCard());
                            }
                        }
                        Console.WriteLine("please choose an attack");
                        Console.WriteLine("0: {0}, Damage: {1}", player1.actPkm.atk[0].name,
                                          player1.actPkm.atk[0].damage);
                        if (player1.actPkm.atk[1] != null)
                        {
                            Console.WriteLine("1: {0}, Damage: {1}", player1.actPkm.atk[1].name,
                                              player1.actPkm.atk[1].damage);
                        }
						int atk = player1.actPkm.atk[1] != null ? getValidUserInput(0, 1) : getValidUserInput(0, 0);

                        if (player2.isFirstTurn)
                        {
                            Console.WriteLine("Sorry, You cannot attack as they have not played an active pokemon");
                        }
                        else
                        {
                            //Check if the player's pokemon has status issues
                            //Do it later since there isn't energies or status problems yet : /

                            //Subtract the attack damage from the players HP, though take resistance and weakness into account.
                            int lostHealth = player1.actPkm.getAttack(atk);
                            player2.actPkm.HP -= lostHealth;

                            Console.WriteLine("{0} used {1}!", player1.actPkm.Name, player1.actPkm.atk[atk].name);
                            Console.WriteLine("{0}'s {1} lost {2} health", player2.getName(), player2.actPkm.Name,
                                              lostHealth);


                            if (player2.actPkm.HP <= 0)
                            {
                                Console.WriteLine("{0}'s {1} has fainted", player2.getName(), player2.actPkm.Name);
                                //Actually it should be sent to the discard
                                player2.setACTPKM(null);

                                if (player2.Bench.Count == 0)
                                {
                                    Console.WriteLine("Congratulations {0}! You win!", player1.getName());
                                }
                                gameInPlay = false;
                            }
                        }
                        player1.isTurn = false;
                        break;
                    }

                case 2:
                    {
                        int L1 = player1.Hand.Count;
                        for (int i = 0; i <= L1; i++)
                        {
                            Console.Write("HP: {0}, Resistance: {1}, Stage: {2}" + player1.Hand[i].Stage);
                            Console.WriteLine("\r\n");
                        }
                        break;
                    }
                case 3:
                    {
                        //L2 equal to the number of cards in the struct
                        int L2 = player1.Hand.Count;
                        for (int i = 0; i <= L2; i++)
                        {
                            Console.WriteLine("Name: " + player1.Hand[i].Name);
                            Console.WriteLine("HP: {0} " + player1.Hand[i].HP.ToString() + " Resistance: " +
                                              player1.Hand[i].Resistance + " Stage : " + player1.Hand[i].Stage);
                            Console.WriteLine("\r\n");
                        }
                        break;
                    }
                case 4:
                    Console.Write("This function has not been implemented");
                    break;

                case 5:
                    break;
				
				
				//Todo: Create a new pokedex function 
				
                case 6: break;
				/*
                    {
                        Console.WriteLine("Please enter a number between 0 and 4086");
                        int demo = Conversions.ToInteger(Console.ReadLine());
                        //mycard = new Card(demo);
                        Console.WriteLine(mycard.Name);
                        Console.WriteLine("HP: " + mycard.HP.ToString() + " Resistance: " + mycard.Resistance +
                                          " Stage :" + mycard.Stage);
                        Console.WriteLine("\r\n");
                        break;
                    }
                    */
				
                case 7:
                    player1.isTurn = false;
                    return;

                case 8:
					attach(player1);
                    break;

                default:
                    Console.WriteLine("You must enter the one of the values.");
                    break;
            }
            draw = true;
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

        public static DataSet RunDBCommand(string SQL)
        {
			OleDbConnection oConn = new OleDbConnection("Data Source='" + FileSystem.CurDir() +
									@"~\PKMDB.mdb';Provider='Microsoft.Jet.OleDb.4.0'");
			OleDbCommand oCmd = new OleDbCommand(SQL, oConn);
			oCmd.CommandType = CommandType.Text;
			OleDbDataAdapter oDA = new OleDbDataAdapter(oCmd);
			DataSet oDS = new DataSet();
			oDA.Fill(oDS, "Result");
			oDA.Dispose();
			oCmd.Dispose();
			oConn.Dispose();
			return oDS;
        }
        
        /*
        #region LoadDeck
        public static int[] LoadDeck(string deckpath, string deckname)
        {
            int[] LoadDeck;
            int[] returnError = { -1 };

            try
            {
                int i = 0;
                int j = 0;
                StreamReader objReader = new StreamReader(deckpath + deckname);
                string strContents = objReader.ReadToEnd();
                objReader.Close();
                string[] itemsRead = strContents.Split(new char[] { ',' });
                int[] intLoadDeck = new int[Information.UBound(itemsRead, 1) + 1];

                if (Information.UBound(itemsRead, 0) > 0)
                {
                    while (!((i == 60) | (i == itemsRead.Length)))
                    {
                        if (Conversions.ToDouble(itemsRead[i]) > 4086.0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The Value " + itemsRead[i] + " Cannot be used an is invalid");
                            Console.ResetColor();
                        }
                        else if (Conversions.ToDouble(itemsRead[i]) <= 0.0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The Value " + itemsRead[i] + " Cannot be used an is invalid");
                            Console.ResetColor();
                        }
                        else
                        {
                            intLoadDeck[i] = int.Parse(itemsRead[i]);
                        }
                        i++;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sorry, this deck cannot be used as it has less than 1 card");
                    Console.ResetColor();
                    return returnError;
                }

                int k = Information.UBound(intLoadDeck, 1) - 1;
                for (i = 0; i <= k; i++)
                {
                    if (intLoadDeck[i] == 0)
                    {
                        int firstI = i;
                        j = i;
                        while (!((intLoadDeck[j] > 0) | (j == Information.UBound(intLoadDeck, 1))))
                        {
                            j++;
                        }
                        intLoadDeck[firstI] = intLoadDeck[j];
                        intLoadDeck[j] = 0;
                    }
                }
                j = Information.UBound(intLoadDeck, 1);
                while (intLoadDeck[j] <= 0)
                {
                    j--;
                }
                if ((j + 1) < Information.UBound(intLoadDeck, 1))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\r\n");
                    Console.WriteLine(
                        "The deck contained a number of invalid values, or was too large therefore the deck has been shrunk to " +
                        int.parse((int) (j + 1)) + " cards.");
                }
                intLoadDeck = (int[]) Utils.CopyArray((Array) intLoadDeck, new int[j + 1]);


                LoadDeck = intLoadDeck;
            }
            catch (FormatException exception1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Sorry, the deck you have selected is invalid, please select a different one");
                Console.ResetColor();
                LoadDeck = returnError;
            }
            return LoadDeck;
        }
        #endregion
         * */

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

        public static void TCGMenu()
        {
            Console.WriteLine("1. Hand\t 2. Check\t 3.Retreat");
            Console.WriteLine("4. Attack\t 5.Pkmn Power\t 6.Done");
            int selection = int.Parse(Console.ReadLine());
            switch (selection)
            {
                case 1: //Hand
                    break;
                case 2: //Check
                    break;
                case 3: //retreat
                    break;
                case 4: // Attack
                    break;
                case 5: // Pkm Power
                    break;
                case 6://done
                    break;
                    
            }

            
        }

        public static void setup()
        {
            //Shuffle the opponets deck

            //Draw seven cards

            //Pick a basic pokemon (Hand to active)
            //Play
            //--Puts a card whereever is supposed to go
            //Check
            //--Displays stats on the pokemon

            //Up to 5 basic pkm on the bench

            //Place Prizes

            //Flip coin
            //--Heads player1, Tails Player2
        }

        public static int[] LoadDeck2(string deckpath, string deckname)
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


                #region CPWay

                //The C++ way to do it, but lets use C# methods
                /* 
                int[] intLoadDeck = new int[numRecords];

                for (i = 0; i < numRecords; i++)
                {
                    //Used to hold the converted int while we decide what to do with it
                    int tempInt;

                    tempInt = int.Parse(itemsRead[i]);

                    if (tempInt < MIN || tempInt > MAX)
                    {
                        intLoadDeck[i] = 0;
                        invalid++;

                    }
                    else
                    {
                        intLoadDeck[i] = tempInt;
                    }
                 * */
                #endregion

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
    }
}