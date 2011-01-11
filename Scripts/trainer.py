import random 
#for the coin toss, its not in the app yet 

#Example of Trainer card using Pokemon Reversal.

def onplay():
	explain()
	#You could call bindEvents here, which should be defined in the script if you need to
	if conFlip == 1 :
		print "player flipped heads"
		print "Choose a benched pokemon to switch your active with"
        chosen = program.choosecard(player2.bench)

def coinFlip():
	return random.randrange(2)
	
def explain():
	print "Flip a coin. If heads, your opponent switches 1 of his or her Active Pokémon with 1 of his or her Benched Pokémon."
	