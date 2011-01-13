using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Data.OleDb;
using System.Data;
using MongoDB;



//Remove visualbasic code in the furture
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;

//using PokemonTCG.

namespace PokemonTCG
{
	//TODO: Considering adding a method for managing energies?
    public class Card
    {

        //Variables.
        //Since this code wasn't designed with encapsulation in mind, all the 
        //variables are public, however this should be changed later.
		//TODO: Just realized that cards data is overwritten, which means that certain scripts won't work.
		//TODO: Rename Variables and remove / fix get/sets
        public int BOGUS_ID;
        public string Name;
        public int HP;
        public Enums.Stage stage;
        public string Weakness;
        public string Resistance;
		public int retreatCost; 
        public Enums.Element type;
        public Enums.Condition Status; //TODO: ensure that you can have mutliple statuses at the same time. See: flags
        public List<Card> attached = new List<Card>();
        public Attack[] atk = new Attack[2];
        public int[] evolvesInto;
        public int[] evolvesFrom;
        public int pNum; 


        
        //For the attack parser
        

        //Get Sets
        public string Stage
        {
            get { return stage.ToString(); }
            set
            {
				try
				{
					switch (value)
					{
						case "Stage 1": stage = Enums.Stage.Stage1; break;
						case "Stage 2": stage = Enums.Stage.Stage2; break;
						case "Baby": stage = Enums.Stage.Baby; break;
						case "Basic": stage = Enums.Stage.Basic; break;
						default: if (value.StartsWith("Trainer"))
							{
								this.stage = Enums.Stage.Trainer;
							}
							else if (value.Contains("Energy"))
							{
								this.stage = Enums.Stage.Energy;
							}
							else if (value.Contains("Level Up"))
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
				}
				catch (myExceptions.InvalidDeckException invalDeckEx)
				{
					Console.WriteLine(invalDeckEx.CustomMessage);
				}
            }
        }
        public string Type
        {
            get { return type.ToString(); }
            //Don't forget about this
            set 
            {
				/*if (value.StartsWith("Trainer") || value.StartsWith("Energy"))
				{
					this.type = Enums.Element.Normal;
				}*/
				#region Energy Type Selection
				try
				{
					// some database entries have multiple space delimited types; only accept first type
					string[] tmpStrArr = value.Split(" ".ToCharArray());
					string firstType = tmpStrArr[0];

					switch (firstType)
					{
						case "Trainer":
							this.type = Enums.Element.Trainer;
							break;
						case "Energy":
							this.type = Enums.Element.Energy;
							break;
						case "Colorless":
							this.type = Enums.Element.Colorless;
							break;
						case "Bug":
							this.type = Enums.Element.Bug; // "bug" type does not exist in database provided
							break;
						case "Darkness":
							this.type = Enums.Element.Dark;
							break;
						case "Lightning":
							this.type = Enums.Element.Electric;
							break;
						case "Fighting":
							this.type = Enums.Element.Fight;
							break;
						case "Fire":
							this.type = Enums.Element.Fire;
							break;
						case "Flying":
							this.type = Enums.Element.Flying; // "flying" type does not exist in database provided
							break;
						case "Ghost":
							this.type = Enums.Element.Ghost; // "ghost" type does not exist in database provided
							break;
						case "Grass":
							this.type = Enums.Element.Grass;
							break;
						case "Water":
							this.type = Enums.Element.Water;
							break;
						case "Steel":
							this.type = Enums.Element.Steel; // "steel" type does not exist in database provided
							break;
						case "Rock":
							this.type = Enums.Element.Rock; // "rock" type does not exist in database provided
							break;
						case "Psychic":
							this.type = Enums.Element.Psychic;
							break;
						case "Poison":
							this.type = Enums.Element.Poison; // "poison" type does not exist in database provided
							break;
						case "Normal":
							this.type = Enums.Element.Normal; // "normal" type does not exist in database provided
							break;
						case "Metal":
							this.type = Enums.Element.Metal;
							break;
						case "Ground":
							this.type = Enums.Element.Ground; // "ground" type does not exist in database provided
							break;
						case "Ice":
							this.type = Enums.Element.Ice; // "ice" type does not exist in database provided
							break;
						default:
							throw new myExceptions.InvalidEnergyTypeException("Invalid energy type: \"" + value + "\"");
					}
				}
				catch (myExceptions.InvalidEnergyTypeException e)
				{
					Console.WriteLine(e.CustomMessage);
				}
				#endregion
            }
        }
            
        Random rnd = new Random();

        public bool meetsCrit(List<Pair> specs)
        {
            int totalEng = 0; 
            foreach (Pair i in specs)
            {
				
				/* This code finds all instances of Card, then Findall uses a delegate to compare wethere or not the found input is equal to 
				 * the requested requirement. Now we take a count of the returned vaules and if that value is not greater than or equal to 
				 * the number of cards we need we return false. 
				 * 
				 * If you noticed that this is run in a foreach loop, it is because we need to evaluate wheather or not all requirements are met 
				 * for example if 2 water and 5 leaf are needed, we evaluate the number of cards that are leaf and see if its enough then we evaluate
				 * wether or not there are enough water to meet the requirements. If both checks pass the loop exists with a return true */
                if (i.type == Enums.Element.Colorless)
                {
                    //If there isn't enough energies to satisfy the requirements 
                    if (!(attached.FindAll(delegate(Card c) { return c.stage == Enums.Stage.Energy; }).Count >= i.value))
                    {
                        return false;
                    }
                    continue;
                }
				if (!(attached.FindAll(delegate(Card c) { return c.type == i.type; }).Count >= i.value))
					//FindAll the cards; pass them into the delegate for matching; compare the number of results 
					//to the number required (passed in via params pair (which is the number (i) of type (type) required. 
                {
                    return false;
                }
            }
            return true;
        }
		
		//TODO: Fix cards to not be initalized on construtor call?
		//TODO: Make input (or its int method) accept a database object and the BOGUS_ID to allow the DB to be opened and closed outside the loop
		//TODO: make a generic database access method instead of mongo
        //Constructor
        /// <summary>
        /// The Constructor for Card,
        /// </summary>
        /// <param name="BOGUS_ID">Bogus_ID is the ID value associated with a input in the database</param>
        /// <param name="mongo">Is the database object which input uses to get the data</param>
        public Card(int BOGUS_ID, IDataStore database)
        {
			//Incase I forgot to remove this from something else
			//throw System.NotImplementedException; 
		}
		
		//Overloaded input constructor, does nothing apparently 
		//TODO: Fix energy class that references this. 
		public Card(int BOGUS_ID)
		{
			
		}
		
		//Proper input method.
        public Card(int BOGUS_ID, string name, int HP, string stage, string Weakness, string Resistance, string type, Attack[] atk, string[] evolvesInto, string[] evolvesFrom, int pNum)
        {
            this.BOGUS_ID = BOGUS_ID;
            this.Name = name;
            this.HP = HP;
            this.Stage = stage;
            this.Weakness = Weakness;
            this.Resistance = Resistance;
            this.Type = type;
            this.atk = atk;

            //Evolution parsers TODO: Just accept an Int array
        }
		
        public string getName()
        {
            return this.Name;
        }

       

        private int[] str2int(string[] input)
        {
            int[] tmp = new int[input.Length];
            try
            {
                for (int i = 0; i < input.Length; i++)
                {
                    tmp[i] = int.Parse(input[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error parsing the evolutions: {0}", e.Message);
            }

            return tmp;

        }

        //Methods
        /// <summary>
        /// Overloaded Method for getAttack; returns the amount of damage that an attack does
        /// </summary>
        /// <param name="atk">An integer related to which attack to execute</param>
        /// <returns>The amount of damanage an attack does</returns>
        public int getAttack(int atk)
        {
            return getAttack(this.atk[atk]);
        }
		

        public int getAttack(Attack atk)
        {
            int atkVal = 0;
            if (atk.damage.EndsWith("x"))
            {
                int tempAtk = int.Parse(atk.damage.Substring(0, 2));
                int tempFlip = 0;
                bool doFlip = true;

                while (doFlip)
                {
                    //Random should generate 0 or 1,
                    //If the random number is 1
                    if (rnd.Next(0,2) == 1)
                    {
                        //Add one to the number of flips
                        tempFlip = tempFlip + 1;
                    }
                    else
                    {   
                        //Otherwise 
                        doFlip = false;
                    }
                }

                atkVal = tempAtk * tempFlip;
                return atkVal;
            }
            else
            {
                return int.Parse(atk.damage);
            }


        }
        public void parseEnergy(string info)
        {
            List<Pair> requirements = new List<Pair>();

            //Enhanced Parser. 
            //Find out if there are brackets
            if (info[0] == '[')
            {
                //If the beginning of the string is a bracket
                //Find the end bracket
                int last = info.IndexOf(']');

                for (int i = 0; i < last; i++)
                {
                    //If i is a number,
                    if (info[1] >= '0' || info[1] <= '9')
                    {
                        //Add a requirement of that number of colorless energies
                        requirements.Add(new Pair((int)info[1],Enums.Element.Colorless));
                    }
                    else
                    {
                        switch (info[i])
                        {
                                
                        }
                    }
                }
            }
        }
		
    }
}