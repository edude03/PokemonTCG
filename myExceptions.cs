using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PokemonTCG
{
    public class myExceptions
    {
        // Base exception which all custom exceptions should inherit from
        public class BaseStepException : ApplicationException
        {
            private string _customMessage;

            // Gets or Sets the custom error message detailing the error
            public string CustomMessage
            {
                get { return this._customMessage; }
                set { this._customMessage = value; }
            }
        }

        /*

        // Exception resulting from a failure in the FTP cleanup process
        public class FTPCleanupException : BaseStepException
        {
            // Constructor (default error message)
            public FTPCleanupException()
            {
                this.CustomMessage = "Exception cleaning-up the temporary files in the FTP process.";
            }

            /// Overloaded constructor (initialize custom error message)
            public FTPCleanupException(string strCustomMessage)
            {
                this.CustomMessage = strCustomMessage;
            }
        }
         * */

        //Invaild Deck Exception
        public class InvalidDeckException : BaseStepException
        {
            //Constructor
            public InvalidDeckException()
            {
                this.CustomMessage = "An Invalid Deck was selected.";
            }

            //Overloaded Constructor 
            public InvalidDeckException(string str)
            {
                this.CustomMessage = str;
            }
        }

        public class DeckNotFoundException : BaseStepException
        {
            public DeckNotFoundException()
            {
                this.CustomMessage = "Deck Not Found";
            }

            public DeckNotFoundException(string str)
            {
                this.CustomMessage = str;
            }
        }

		public class InvalidEnergyTypeException : BaseStepException
		{
			public InvalidEnergyTypeException()
			{
				this.CustomMessage = "Invalid energy type!";
			}

			public InvalidEnergyTypeException(string str)
			{
				this.CustomMessage = str;
			}
		}
    }
}