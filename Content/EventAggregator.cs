using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonTCG.Content
{
   public sealed class EventAggregator
   {
	    public static readonly EventAggregator Instance = new EventAggregator();

	    private EventAggregator()
	    {
	    }

        //Events 
	    public event EventHandler PlayerDied;
        public event EventHandler PlayTurnEnd;

	    public void OnPlayerDied(Player player)
	    {
		    if(PlayerDied != null)
			    PlayerDied(player, EventArgs.Empty);
	    }
   }

    public class Game
    {
	    public void SendPlayerDiedEvent(Player player)
	    {
		    EventAggregator.Instance.OnPlayerDied(player);
	    }
    }

    public class player_objects : IDisposable
    {
        public player_objects()
        {
		    EventAggregator.Instance.PlayerDied += HandlePlayerDied;
        }
   
        public void Dispose() 
        {
		    EventAggregator.Instance.PlayerDied -= HandlePlayerDied;
        }
   
        public void HandlePlayerDied(object sender, EventArgs e)
        {
            /* Do something with here */
        } 
    }
}
