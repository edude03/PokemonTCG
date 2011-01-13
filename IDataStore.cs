using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonTCG
{
    public interface IDataStore
    {
        Card getCardbyID(int BOGUS_ID);
        List<Card> getCardArray(int[] intArray);
        
        //I guess?
        void connect();
        void disconnect(); 
    }
}
