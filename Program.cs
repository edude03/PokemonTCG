using System;
using System.Collections.Generic;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;
using MongoDB;

//IronPython Stuff
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;


//Events


namespace PokemonTCG
{
    internal class Program
    {

        //Variables

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
                            //Get the game started.
                            Game theGame = new Game(Enums.gameType.Offline);
                            theGame.Main();
							break;
						case Enums.gameType.Computer:
							Console.WriteLine("Sorry, this feature had yet to be implemented");
							break;
						default:
							Console.WriteLine("Sorry, you must select one of the options.");
							break;
					}
				} while(validMode == false);

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
    }
}