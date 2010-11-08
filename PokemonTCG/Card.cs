using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.Data.OleDb;
using System.Data;
using Microsoft.VisualBasic.CompilerServices;

//using PokemonTCG.

namespace PokemonTCG
{
    public class Card
    {

        //Variables.
        //Since this code wasn't designed with encapsulation in mind, all the 
        //variables are public, however this will be changed later.

        public int BOGUS_ID;
        public string Name;
        public int HP;
        public Enums.Stage stage;
        public string Weakness;
        public string Resistance;
        public Enums.Element type;
        public Enums.Condition Status;
        public List<Card> attached = new List<Card>();
        public Attack[] atk = new Attack[2];
        
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
								throw new myExceptions.InvalidDeckException("Unhandled card type loaded! (\"Level Up\")");
							}
							else
							{
								Console.WriteLine("What?");
								throw new myExceptions.InvalidDeckException("An invalid card was loaded");
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

        public bool meetsCrit(params Pair[] specs)
        {
            foreach (Pair i in specs)
            {
                //**** I wish I remembered what this code does : /
                if (!(attached.FindAll(delegate(Card c) { return c.type == i.type; }).Count >= i.value))
                {
                    return false;
                }
            }
            return true;
        }

        //Constructor
        /// <summary>
        /// The Constructor for Card,
        /// </summary>
        /// <param name="BOGUS_ID">Bogus_ID is the ID value associated with a card in the database</param>
        public Card(int BOGUS_ID)
        {
            DataSet oDS = Program.RunDBCommand("SELECT * FROM PokemonTCG WHERE BOGUS_ID = " + Conversions.ToString(BOGUS_ID));
            this.Name = Conversions.ToString(oDS.Tables[0].Rows[0]["Name"]);
            if (Information.IsDBNull(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(oDS.Tables[0].Rows[0]["HP"]))))
            {
                this.HP = 0;
            }
            else
            {
                this.HP = Conversions.ToInteger(oDS.Tables[0].Rows[0]["HP"]);
            }
            if (Information.IsDBNull(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(oDS.Tables[0].Rows[0]["Resistance"]))))
            {
                this.Resistance = "";
            }
            else
            {
                this.Resistance = Conversions.ToString(oDS.Tables[0].Rows[0]["Resistance"]);
            }
            if (Information.IsDBNull(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(oDS.Tables[0].Rows[0]["Weakness"]))))
            {
                this.Weakness = "";
            }
            else
            {
                this.Weakness = Conversions.ToString(oDS.Tables[0].Rows[0]["Weakness"]);
            }
            this.Type = Conversions.ToString(oDS.Tables[0].Rows[0]["Type"]);
            this.Stage = Conversions.ToString(oDS.Tables[0].Rows[0]["Stage"]);
            this.BOGUS_ID = BOGUS_ID;

            if (this.stage != Enums.Stage.Energy && this.stage != Enums.Stage.Trainer)
            {
                //Get the information for the card then parse through it

                this.atk[0] = new Attack(Conversions.ToString(oDS.Tables[0].Rows[0]["attack1"]));
                
                if (this.atk[0].damage == null)
                {
                    this.atk[0].damage = Conversions.ToString(oDS.Tables[0].Rows[0]["TypicalDamage1"]);
                }
                if (!Information.IsDBNull(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(oDS.Tables[0].Rows[0]["attack2"]))))
                {
                    
                    //Conversions.ToString(oDS.Tables[0].Rows[0]["atk2"]);
                    this.atk[1] = new Attack(Conversions.ToString(oDS.Tables[0].Rows[0]["attack2"]));
                    
                    //this.atk;
                    if (this.atk[1].damage == null)
                    {
                        this.atk[1].damage = Conversions.ToString(oDS.Tables[0].Rows[0]["TypicalDamage2"]);
                    }
                }
            }
        }

        public string getName()
        {
            return this.Name;
        }

        //Methods
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

        public bool meetsEnergy(Attack atk)
        {
            //compare the number of energies in each array
            //and return true / false
            List<Pair> temp = new List<Pair>();


            return true;
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
