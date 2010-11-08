﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;

namespace PokemonTCG
{
    public class Attack
    {
        public string damage;
        public string info;
        public string name;
        public List<Pair> requirements;
        public string type;
        public PokemonTCG.Enums.Condition effects;
        Random rnd = new Random();

        //Constructor
        public Attack(string info)
        {
            requirements = new List<Pair>();
            //Determine the energy requirements
            if (info[0] == '[')
            {
                //Get the energy requirements
                List<char> energy = new List<char>();
                for (int i = 1; i < info.IndexOf(']'); i++)
                {
                    energy.Add(info[i]);
                }

                do
                {
                    //Do something with the char
                    if (energy[0] >= '0' || energy[0] <= '9')
                    {
                        //Colorless energy ++
                        if (energy.Count > 1)
                        {
                            int temp = energy[0];

                            while (temp > 0)
                            {
                                requirements.Add(new Pair(1,Enums.Element.Colorless));
                                temp--;
                            }
                        }
                        else
                        {
                            requirements.Add(new Pair(1, Enums.Element.Colorless));
                        }
                        
                    }
                    else
                    {
                        //specific energy ++
                        switch(energy[0])
                        {
                            case 'W': requirements.Add(new Pair(1, Enums.Element.Water));
                                break;
                            case 'G': requirements.Add(new Pair(1, Enums.Element.Grass));
                                break;
                            case 'R': requirements.Add(new Pair(1, Enums.Element.Fire));
                                break;
                            case 'L': requirements.Add(new Pair(1, Enums.Element.Electric));
                                break;
                            case 'F': requirements.Add(new Pair(1, Enums.Element.Fight));
                                break;
                            case 'P': requirements.Add(new Pair(1, Enums.Element.Psychc));
                                break;
                            case 'D': requirements.Add(new Pair(1, Enums.Element.Dark));
                                break;
                            case 'M': requirements.Add(new Pair(1, Enums.Element.Metal));
                                break;
                                
                        }
                    }
                    energy.RemoveAt(0);
                } while (energy.Count > 0);

                /* //Apparently the new parser is broken, resorting to using the old one
                //Get attack damage
                if (info.Contains('(')) // && info.i) <-- What?
                {
                    damage = (info.Substring(info.IndexOf('(')+1, (info.IndexOf(')') - info.IndexOf('('))));
                }

                //Get Attack name
                //If there is energy requirements
                if (info.Contains(']'))
                {
                    //The name Starts after the ']'
                    int index = info.IndexOf(']');
                    name = info.Substring(index+2, info.IndexOfAny(new char[] {'-','('}) - index+2);

                }
                 * */
             
            }
             
        }

        private void oldParser()
        {
            int fBracket = 0;
            int fName = 0;
            int sBracket = 0;
            int UBound = info.Length - 1;

            //C# doesn't auto int these values to 0
            int firstB = 0;
            int lastB = 0;

            //Loop through the entire string
            for (int i = 0; i <= UBound; i++)
            {
                //If a "[" is found,
                if (Conversions.ToString(info[i]) == "[")
                {
                    //Store its index
                    firstB = i;
                }
                //Else if a "]" is found,
                else if (Conversions.ToString(info[i]) == "]")
                {
                    //Store its Index
                    lastB = i;
                }
            }

            int UB1 = info.Length - 1;
            for (int i = lastB + 1; i <= UB1; i++)
            {
                if (Conversions.ToString(info[i]) == "-")
                {
                    if (fName <= 0)
                    {
                        fName = i;
                    }
                }
                else if (Conversions.ToString(info[i]) == "(")
                {
                    if (fBracket <= 0)
                    {
                        fBracket = i;
                    }
                }
                else if ((Conversions.ToString(info[i]) == ")") && (sBracket <= 0))
                {
                    sBracket = i;
                }
            }
            //this.requirements = info.Substring(firstB + 1, (lastB - firstB) - 1);
            if (!((sBracket == 0) & (fBracket == 0)) & (fName <= 0))
            {
                this.damage = info.Substring(fBracket + 1, (sBracket - fBracket) - 1);
                this.name = info.Substring(lastB + 2, (sBracket - lastB) - 6);
            }
            else
            {
                this.name = info.Substring(lastB + 2, fName - 5);
            }
            if (((sBracket > 0) & (fName == 0)) & ((sBracket + 1) != info.Length))
            {
                this.info = info.Substring(sBracket + 2);

            }
            if (((sBracket > 0) & (fName == 0)) & ((sBracket + 1) == info.Length))
            {
                this.info = this.name + " (" + this.damage + ")";
            }
            this.info = info.Substring(fName + 2);
        }

        public bool meetsReqs (List<Pair> reqs, List<Card> attached)
        {
            foreach (Pair p in reqs)
            {
                //Find p in the calling pokemon attached array

                //Loop through attached
                if (!find(p,attached)) //If find returns false
                {
                    //Cleans up after the find method before exiting
                    foreach (Energy e in attached)
                    {
                        e.found = false;
                    }
                    return false;
                }

            } //if it exits the foreach loop then the required cards where found


            //Cleans up after the find method before exiting
            foreach (Energy e in attached)
            {
                e.found = false;
            }
            
            //If it made it this far then everything should be good
            return true;
        }
        
        /// <summary>
        /// Find the Card of type and value listed in the pair P
        /// </summary>
        /// <param name="p">A Value,Type Pair</param>
        /// <param name="attached">A list of cards</param>
        /// <returns>Boolean based on wheter or not the card was found</returns>
        private bool find(Pair p, List<Card> attached)
        {
            foreach (Energy e in attached)
            {
                if (e.type == p.type)
                {
                    //Makes sure if the loop is repeated its not counted twice
                    e.found = true;

                    //If it finds a matching energy exit the loop and return true
                    return true;
                }
            }
            //If the for loop doesn't find a match, then return false
            return false;
        }

    }
}
