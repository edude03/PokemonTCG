using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonTCG
{
    public class StatusEngine
    {
        private Enums.Condition status;
        private Random rng;

        public StatusEngine()
        {
            rng = new Random();
        }
        
        public Enums.Condition Status
        {
            get
            {
                return status;
            }
            set
            {
                switch (value)
                {
                    case Enums.Condition.Burn:
                        break;
                    case Enums.Condition.Frozen:
                        break;
                    case Enums.Condition.Poison:
                        break;
                    case Enums.Condition.Faint:
                        break;
                    case Enums.Condition.Paralyzed:
                
                        break;
                    case Enums.Condition.Confused:
                        status = value; 
                        break;
                    case Enums.Condition.Sleep:
                        status = value;
                        break;
                }
            }
        }

        public void poisoned(Card input)
        {
            //Subtract ten from the Cards health
            input.HP -= 10; 
        }

        public void burn(Card input)
        {
            //If tails, zero is heads by convention
            if (flipCoin() == 0)
            {
                input.HP -= 20;
            }
        }

        public void confused(Card input)
        {
            throw new System.NotImplementedException();
        }

        public void paralyzed(Card input)
        {
            throw new System.NotImplementedException();
        }

        //Same thing as the one in player, I just got lazy
        private int flipCoin()
        {
            return rng.Next(0, 1);
        }
    }
}
