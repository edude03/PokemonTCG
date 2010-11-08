﻿using System;
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
                        else
                        {
                            Console.WriteLine("What?");
                                throw new myExceptions.InvalidDeckException("An invalid card was loaded");
                        }
                        break;
                }
            }
                    

        }
        public string Type
        {
            get { return type.ToString(); }
            //Don't forget about this
            set 
            {
                if (value.StartsWith("Trainer") || value.StartsWith("Energy"))
                {
                    this.type = Enums.Element.Normal;
                }
                
		 
	
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
                        this.atk[1].damage = Conversions.ToString(oDS.Tables[0].Rows[0]["TypicalDamage1"]);
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
