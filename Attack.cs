﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using System.Text.RegularExpressions;

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
					//TODO: Fix areas of code that make assumptions like this. 
                    energy.Add(info[i]);
                }

                do
                {
                    //Do something with the char
					int intOut;
					if (int.TryParse(energy[0].ToString(), out intOut)) //if (energy[0] >= '0' || energy[0] <= '9')
                    {
                        //Colorless energy ++
                        if (energy.Count > 1)
                        {
                            int temp = int.Parse(energy[0].ToString());

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
                            case 'P': requirements.Add(new Pair(1, Enums.Element.Psychic));
                                break;
                            case 'D': requirements.Add(new Pair(1, Enums.Element.Dark));
                                break;
                            case 'M': requirements.Add(new Pair(1, Enums.Element.Metal));
                                break;
                            default:
								throw new myExceptions.InvalidEnergyTypeException();
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

				// regex parser for name and info
				if (info.Substring(0, 1) == "[")
				{
					// remove attack info inside square brackets
					string parsedInfo = Regex.Replace(info, @"^\[.*\] ", "", RegexOptions.IgnoreCase);
					// remove item between name and description; name and desc. are seperated by a tilde
					parsedInfo = Regex.Replace(parsedInfo, @" (\(.{1,4}\)|-)($| )", "~", RegexOptions.IgnoreCase);
					name = Regex.Replace(parsedInfo, @"~.*", "", RegexOptions.IgnoreCase); // destroy everything after the tilde
				}
             
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

	}
}
