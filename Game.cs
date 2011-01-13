using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;

//Mongo Stuff
using MongoDB;
using MongoDB.Bson;
using PokemonTCG.DataStore;

//IronPython Stuff
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;


namespace PokemonTCG
{
    
    public class Game
    {
        //Variables
        private string deckpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "decks" + Path.DirectorySeparatorChar;
        private bool done = false;
        private bool gameInPlay = true;
        private bool playAgain = true;
        private string scriptPath = "Z:\\Coding\\PokemonTCG\\PokemonTCG\\Scripts\\trainer.py";


        //Methods
        private void attach(Player player1)
        {
            //Choose a input from Hand
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

        //I like my contructors at the top
        public Game(Player player1, Player player2, Enums.gameType gameType) 
        {
            
        }

        public Game(Enums.gameType type)
        {
           
        }

        private Card[] chainload(string deckpath)
        {
            //Choose a CSV File -> Parse the CSV -> Convert the int array to input objects -> return input array.
            return intArrayDeck(LoadDeck(deckpath, chooseDeck(deckpath)));

        }

        private void check(Card card)
        {
            throw new NotImplementedException();
        }

        public int choosecard(List<Card> source)
        {
            int inputC = -1;
            if (source.Count == 0)
            {
                Console.WriteLine("Sorry! No cards available");
            }
            else
            {
                Console.WriteLine("Please select a input:");
                for (var i = 0; i <= source.Count - 1; i++)
                {
                    //This should skip any null cards however I doubt any exist in the game. This is by design. 
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

        public string chooseDeck(string deckPath)
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
                Console.WriteLine("Please select a deck to load");

                int i = 0;
                for (; i < aryFi.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, aryFi[i].Name);
                }
                //for future use
                //Console.WriteLine((i + 1) + " : [Randomly Generated]");
                int intDeckChoosen = getValidUserInput(0, aryFi.Length - 1);
                strDeckChoosen = aryFi[intDeckChoosen].Name;

                return strDeckChoosen;
            }
        }

        public void exTrainer(string scriptPath, Player player1, Player player2, Game gInst)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptRuntime runtime = engine.Runtime;
            ScriptScope scope = runtime.CreateScope();

            ScriptSource source = engine.CreateScriptSourceFromFile(scriptPath);
            scope.SetVariable("player1", player1);
            scope.SetVariable("player2", player2);
            scope.SetVariable("game", gInst);

            source.Execute(scope);

        }

        public void gameLoop(Player player1, Player player2)
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
                    player1 = new Player(player1.getName(), chainload(deckpath), this);
                    player2 = new Player(player2.getName(), chainload(deckpath), this);
                }
            } while (playAgain);
        }


        #region dataAccess
            

        /// <summary>
        /// Gets a input from the database
        /// </summary>
        /// <param name="BOGUS_ID">The ID of the input object to retreive</param>
        /// <param name="mongo">the database object to use to get the results</param>
        /// <returns>a Card object</returns>
        public Card getCardFromDB(int BOGUS_ID, IDataStore database)
        {
            
            #region oldCode 
            /*
            Document query = new Document(
            
            
            Cursor cq = new Cursor();
            c

            query["BOGUS_ID"] = "$in" + BOGUS_ID;
            Document results = mongo["pokemon"]["cards"].Find(

            //TODO: Convert the strings to their proper type internally 
            string Name = results["Name"].ToString();
            string Stage = results["Stage"].ToString();
            string Type = results["Type"].ToString();

            //Evolution code (Just noticed that the two keys in the database are in different case 
            //now (after an ugly exception) whoever did that should get punched in the head :/
            string[] evolInto = results["EvolvesFrom"].ToString().Split(',');
            string[] evolFrom = results["evolvesInto"].ToString().Split(',');



            Enums.Element type = parseType(Type);
            Enums.Stage stage = parseStage(Stage);
            int HP = 0;
            string Weakness = "";
            string Resistance = "";
            int pNum = 0;
            Attack[] atk = new Attack[2];

            if (type != Enums.Element.Trainer && type != Enums.Element.Energy)
            {
                HP = int.Parse((results["HP"] ?? 0).ToString());
                Weakness = results["Weakness"].ToString();
                Resistance = results["Resistance"].ToString();

                //Give me the pokemon number or -1
                int tryP = -1;

                if (!(int.TryParse(results["pokemonNumberInt"].ToString(), out tryP)))
                {
                    Console.Write("?");
                }


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
            //Make sure to fix input to correct this issue. 
            return new Card(BOGUS_ID, Name, HP, Stage, Weakness, Resistance, Type, atk, evolInto, evolFrom, pNum);

             * */
            #endregion
            database.connect();
            return database.getCardbyID(BOGUS_ID);

        }

        //This might be a confusing name, since status like burn and such need to be implemented as well
        public string getStatus(Card c)
        {
            string status = c.Name + " || HP: " + c.HP + " Status: " + c.Status;
            return status;
        }
#endregion
             

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
        /// A <see cref="System.it :/Int32"/> equal to th input (assuming it's valid) 
        /// </returns>
        /// Might cause a fenchpost error but I'll look into that later TODO: fix this issue.
        public int getValidUserInput(int lowerBounds, int upperBounds)
        {
            bool validInput = false;
            int intOut = 0;

            do // do until valid input is entered.
            {
                try
                {
                    int intInput = int.Parse(Console.ReadLine());
                    if (isValidUserInput(intInput, lowerBounds, upperBounds))
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

        private Card[] intArrayDeck(int[] intDeck)
        {
            int Size = intDeck.Length;
            Card[] deck = new Card[Size];

            //TODO: If mongo can't connect for some reason find out why and correct the issue,
            //To improve performance and to ensure that we don't run out of connections 
            //A universal connection is created outside the loop, then closed after the deck is created. 
            //Instanate the connection 
            MongoDataStore ms = new MongoDataStore();

            try
            {
                ms.connect();
                deck = ms.getCardArray(intDeck).ToArray(); //TODO: Figure out why this method (intArrayDeck) returns a Card array instead of a list?
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Unable to connect to the database, please try again.");
                deck = null;
                System.Environment.Exit(-1);
            }
           
            ms.disconnect();
            return deck;

        }

        public bool isValidUserInput(int intInput, int lowerBounds, int upperBounds)
        {
            if (intInput <= upperBounds && intInput >= lowerBounds)
            {
                return true;
            }
            return false;
        }

        public int[] LoadDeck(string deckpath, string deckname)
        {
            int[] returnError = { -1 };
            int numRecords;

            try
            {
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
                    if (!(temp < MIN | temp > MAX) & temp != 0)
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

        public void Main()
        {
            //Get the players name and set them up
            Console.WriteLine("Please enter your name Player 1");
            string input = Console.ReadLine();
            Player player1 = new Player(input, chainload(deckpath), this);
            setup(player1);

            Console.WriteLine("Please enter your name Player 2");
            input = Console.ReadLine();
            Player player2 = new Player(input, chainload(deckpath), this);
            setup(player2);

            //Get the game started.
            gameLoop(player1, player2);
        }

        public void menu_Hand(Player curPlayer)
        {
            //Show the user all of their cards.
            printCards(curPlayer.Hand);

            //The user can chose any of there cards
            int chosen = getValidUserInput(0, curPlayer.Hand.Count); //Number of cards in your hand lol
            //If the user chooses a input
            //If the input is a basic pokemon 

            if (curPlayer.Hand[chosen].stage == Enums.Stage.Basic)
            {
                //Allow user to check, put on bench or go back
                Console.WriteLine("1. Check \n 2. Bench 3. \n Back \n");
                switch (getValidUserInput(1, 3))
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
                    //Allow the user to attach the input to a Pokemon
                    Console.WriteLine("Please pick a pokemon to attach this to");
                    printCards(curPlayer.Bench); //<-- TODO: Code something incase there is nothing in the bench or other input source
                    Console.WriteLine("A for ActivePkm");

                    //The next code block is probably a bad idea TODO: Clean / Fix this. 
                    string temp = Console.ReadLine();
                    int tryP;
                    int.TryParse(temp, out tryP);
                    //This exemplfies why getValidUserInput was written in the first place 
                    if ((temp != "a" && temp != "A") || tryP > curPlayer.Bench.Count || tryP < 0)//Serious bidness if you enter something wrong here. 
                    {
                        Console.WriteLine("That input was invalid");
                    }
                    else if (temp == "A" || temp == "a")
                    {
                        curPlayer.move(curPlayer.Hand, chosen, curPlayer.actPkm.attached);
                        curPlayer.isFirstEnergy = false;
                    }
                    else
                    {
                        curPlayer.move(curPlayer.Hand, chosen, curPlayer.Bench[int.Parse(temp)].attached);
                        curPlayer.isFirstEnergy = false;
                    }


                }
                else
                {
                    //Tell them they can't do it and go back
                    Console.WriteLine("Sorry, you can only attach one energy per turn");
                }
            }
            //If it is some other input
            else
            {
                //Check if it is an evolution and allow the user to change it out?
                Console.WriteLine("Sorry, I'm still working on evolutions as well ;_;"); //TODO: Write evolution code
                int evolT = choosecard(curPlayer.Hand);

                //If the chose input is a valid evolution
                if (curPlayer.validEvol(curPlayer.actPkm, curPlayer.Hand[evolT]))
                {
                    curPlayer.doEvol(curPlayer.Hand, evolT);
                }
            }

        }

        public Enums.Stage parseStage(string strStage)
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
                        throw new myExceptions.InvalidDeckException("Unhandled input type loaded! (\"Level Up\")");
                    }
                    else
                    {
                        Console.WriteLine("What?");
                        throw new myExceptions.InvalidDeckException("An invalid input was loaded");
                    }
                    break;
            }
            return stage;
        }

        public Enums.Element parseType(string strType)
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

        public void printCards(List<Card> input)
        {
            int i = 0;
            foreach (Card c in input)
            {
                Console.Write("{0}: {1} ", i, c.Name);
                Console.Write("\n");
                i++;
            }
        }

        public void setup(Player currentPlayer)
        {
            //Shuffle the opponets deck
            currentPlayer.shuffleDeck();

            //Draw seven cards
            currentPlayer.draw(7);

            //Check that the user got some basics in the draw
            //While the current player doesn't have any basics in their hand
            while (!(currentPlayer.Hand.Exists(p => p.stage == Enums.Stage.Basic)))
            {
                Console.WriteLine("You have no basic cards in your hand; a new hand will be drawn");
                currentPlayer.deck.AddRange(currentPlayer.Hand);
                currentPlayer.Hand.Clear();
                currentPlayer.deck.shuffle();
                currentPlayer.draw(7);

                //Offer player 2 two more cards 
                //TODO: ^
            }


            //Place Prizes
            currentPlayer.initPrizes(6);

            //Pick a basic pokemon (Hand to active)
            int choosen = 0;
            Card chsn;
            bool bench = true;
            bool bCont = true;

            // The user wants to put cards in their bench
            /* --- Code beyond this point is a cluster**** and I barely know what it does anymore :/ --- */
            while (bench == true)
            {
                Console.WriteLine("Please select a input to put on your bench (" + currentPlayer.getName() + ")");

                if (choosen != -1)
                {
                    do
                    {
                        int check = choosecard(currentPlayer.Hand);
                        if (currentPlayer.Hand[check] == null || currentPlayer.Hand[check].Stage != "Basic")
                        {
                            Console.WriteLine("The selection was invalid, please slect a basic pokemon");
                        }
                        else
                        {
                            currentPlayer.move(currentPlayer.Hand, check, currentPlayer.Bench);
                            bCont = false;
                        }
                    } while (bCont);
                }
                else if (choosen == -1)
                {
                    bench = false;
                    break;
                }
                Console.WriteLine("Would you like to put another input on the bench?");
                string inputCs = Console.ReadLine();

                //Convert the input to lowercase so that we have less possibilties that they can enter.
                if (inputCs.ToLower() == "no" | inputCs.ToLower() == "n")
                {
                    bench = false;
                }
            }

            //Tell the player to select an active pokemon
            Console.WriteLine("Please select an active pokemon");

            //Call the overloaded chooseCard method which will iterate the cards in the source (bench)
            currentPlayer.setActPkm(currentPlayer.Bench, choosecard(currentPlayer.Bench));


            //choosecard method will be launched before the setActPKM command
            currentPlayer.isFirstTurn = false;



            //Up to 5 basic pkm on the bench



            //Flip coin
            //--Heads player1, Tails Player2
        }

        public void TCGMenu(Player curPlayer, Player otherPlayer)
        {
            //If the player hasn't drawn yet (this turn):
            if (curPlayer.hasDrawn == false)
            {
                //Deck will return the topmost input in the deck so add it to the Hand (straight forward)
                Card tmp; //I hate temp variables but what can you do 
                if (curPlayer.draw(out tmp))
                {
                    curPlayer.Hand.Add(tmp); //<-- If the player can't draw they lose (TODO: implement (proably using out)
                }
                else
                {
                    //TODO: What happens when the player loses
                }
                //Set player hasDrawn to true so that they don't draw again.
                curPlayer.hasDrawn = true; //<--TODO: replace these with a state machine?

                //Print the name of the input they drew.
                Console.WriteLine("{0} drew {1}", curPlayer.getName(), curPlayer.getLastCard().Name);
            }


            do
            {
                done = false; //The menu loops until the player is done. 
                if (curPlayer.actPkm == null)
                {
                    Console.WriteLine("[{0}]", curPlayer.getName());
                    Console.WriteLine("Please pick an active Pokemon:");
                    curPlayer.setActPkm(curPlayer.Hand, choosecard(curPlayer.Hand));

                }

                Console.WriteLine("[{0}, Active PKM: {1}]", curPlayer.getName(), getStatus(curPlayer.actPkm));
                Console.WriteLine("1. Hand\t 2. Check\t 3.Retreat");
                Console.WriteLine("4. Attack\t 5.Pkmn Power\t 6.Done");
                switch (getValidUserInput(1, 7)) //7 is secret trainer. 
                {
                    case 1: //If the user picks Hand
                        menu_Hand(curPlayer);
                        break;

                    case 2: //If the user picks Check
                        //Show the user data on whatever input they choose?
                        break;

                    case 3: //If the user picks Retreat
                        //Check if the user can ever retreat
                        if (curPlayer.retreat()) //If the retreat is sucsessful 
                        {
                            Console.WriteLine("Card retreated, please pick a new active pokemon");
                            //Chose a input from player 1s hand 
                            curPlayer.setActPkm(curPlayer.Bench, choosecard(curPlayer.Bench));
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
                            int chosen = getValidUserInput(0, 1);
                            if (curPlayer.actPkm.meetsCrit(curPlayer.actPkm.atk[chosen].requirements))
                            {
                                //They have enough energies execute that b**ch
                                //TODO: Extract this out to a method, directly subtracting the damage in the game loop prevents
                                //us from hooking an event in here (for example a pokemon power)
                                otherPlayer.actPkm.HP -= curPlayer.actPkm.getAttack(chosen);
                                //If the damage done is 0 it should say "But it failed" after this
                                Console.WriteLine("{0} used {1}!", curPlayer.actPkm.Name, curPlayer.actPkm.atk[chosen].name);

                                if (otherPlayer.actPkm.HP <= 0)
                                {
                                    //Bitch fainted. TODO handle it.
                                    //Player 2's activePkm goes to discarded pile
                                    otherPlayer.discard(otherPlayer.actPkm);

                                    //You get to draw a prize
                                    if (curPlayer.Prizes.Count > 0)
                                    {
                                        curPlayer.Hand.Add(curPlayer.drawPrize());
                                    }
                                    else
                                    {
                                        //You win
                                        gameInPlay = false;
                                        Console.WriteLine("You win {0}!", curPlayer.getName());
                                    }

                                    //If the other player doesn't have any cards on their bench 
                                    if (otherPlayer.Bench.Count == 0)
                                    {
                                        //You win.
                                        gameInPlay = false;
                                        Console.WriteLine("You win {0}!", curPlayer.getName());
                                    }
                                    //else do nothing but end the turn
                                }
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

                    case 7: //Trainer
                        exTrainer(scriptPath, curPlayer, otherPlayer, this);
                        break;

                }
            } while (!done);
            //Reset the turn variables 
            curPlayer.hasDrawn = false;
            curPlayer.isFirstEnergy = true;
        }
    }
}
