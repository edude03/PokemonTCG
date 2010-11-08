using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonTCG
{
    public class Hand
    {
        private int counter = 0;
        private int max = 10;
        private Card[] hnd;
        private Card errCard;
        private Card temp;

        //Constructor
        public Hand()
        {
            //Create spaces to store new cards as they are added.
            hnd = new Card[max];
        }

        public void addCard(Card card)
        {
            if (counter < this.max - 1)
            {
                this.hnd[counter] = card;
            }
            else
            {
                //Tell the game somehow that the hand is full
            }
        }

        private void removeCard(int index)
        {
            //This might need some code to make sure this is valid but whatever
            this.hnd[index] = null;

        }

        //overloaded it incase the calling method knows which card it wants.
        public Card chooseCard()
        {
            /* Get choosecard to show us the cards in Hand,
             * use the index it returns to get that card and store it temporarly
             * Remove that card from hand and return it to the calling method
             */

            int intTemp = Program.choosecard(this.hnd);
            temp = hnd[intTemp];
            removeCard(intTemp);
            return temp;
        }

        public Card chooseCard(int index)
        {
            //Check if the card it wants is valid
            if (hnd[index] != null)
            {
                temp = hnd[index];
                removeCard(index);
                return temp;
            }
            else
            {
                //Someway to tell the calling method this is not vaild
                //Since the contrustor isn't called, it will return a card that is null
                return errCard;
            }
        }

        public void DiscardHand()
        {

        }

    }
}
