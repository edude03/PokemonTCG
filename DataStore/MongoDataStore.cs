using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace PokemonTCG.DataStore
{
    class MongoDataStore : IDataStore
    {
        MongoServer mg = new MongoServer("mongodb://pokemon:theGame@db.melenion.org");
        //Empty Constructor.
        public MongoDataStore() { }

        public Card getCardbyID(int BOGUS_ID)
        {  
           IMongoQuery query = Query.EQ("BOGUS_ID", BOGUS_ID);
           BsonDocument temp = mg["Pokemon"]["cards"].FindOne(query);
           return parseMongoResult(temp);
        }

        public List<Card> getCardArray(int[] intArray)
        {
            List<Card> temp = new List<Card>();
            MongoCollection<BsonDocument> cards = mg["pokemon"].GetCollection("cards"); 
            
            var query = Query.In("BOGUS_ID", BsonArray.Create(intArray));
            foreach (BsonDocument result in cards.Find(query))
            {
                temp.Add(parseMongoResult(result));
            }

            return temp;
        }

        public void connect()
        {
            try
            {
                mg.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void disconnect()
        {
            mg.Disconnect();
        }

        private Card parseMongoResult(BsonDocument results)
        {  
            //TODO: Convert the strings to their proper type internally 
            string Name = results["Name"].ToString();
            string Stage = results["Stage"].ToString();
            string Type = results["Type"].ToString();

            //Evolution code (Just noticed that the two keys in the database are in different case 
            //now (after an ugly exception) whoever did that should get punched in the head :/
            string[] evolInto = results["EvolvesFrom"].ToString().Split(',');
            string[] evolFrom = results["evolvesInto"].ToString().Split(',');

            //Ugliest hack of life, I'm so ashamed of myself right now. 
            //TODO: Do something so that parseType is in card or something accessible 
            Game parse = new Game(Enums.gameType.Offline); 
            Enums.Element type = parse.parseType(Type);
            Enums.Stage stage = parse.parseStage(Stage);
           

            int HP = 0;
            string Weakness = "";
            string Resistance = "";
            int pNum = 0;
            int BOGUS_ID = results["BOGUS_ID"].ToInt32();
            Attack[] atk = new Attack[2];

            if (type != Enums.Element.Trainer && type != Enums.Element.Energy)
            {
                HP = int.Parse((results["HP"] ?? 0).ToString());
                Weakness = results["Weakness"].ToString();
                Resistance = results["Resistance"].ToString();

                //Give me the pokemon number or -1
                int tryP = -1;

                if (!(int.TryParse(results["pokemonNumberInt"].ToString(), out tryP)))
                {
                    Console.Write("?");
                }


                if (!(results["Attack1"].ToString() == string.Empty)) //If there is an attack 1
                {
                    atk[0] = new Attack(results["Attack1"].ToString());
                    atk[0].damage = results["TypicalDamage1"].ToString();
                }

                if (!(results["Attack2"].ToString() == string.Empty))
                {
                    atk[1] = new Attack(results["Attack2"].ToString());
                    atk[1].damage = (results["TypicalDamage2"].ToString());
                }
            }
            //Make sure to fix input to correct this issue. 
            return new Card(BOGUS_ID, Name, HP, Stage, Weakness, Resistance, Type, atk, evolInto, evolFrom, pNum);
        }
    }
}
