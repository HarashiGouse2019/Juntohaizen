<title> JuntohaizenDSL </title>

<author> Miijii </author>

<date> 05/05/2020 </date>

<description> The basic dialogue that will be seen in the game, Juntohaizen. </description>


//I don't even know how I'm going to get this to work...
//I mean... I have a base... but it's not that even good...

/*And then we have this...
A multi comment thingy majig...
now how is that going to work???*/
<CHARACTERS>
	Mary
	Hazel
	Mori
	Tray
	Globby
	Merchant
	Austin
	God
<END>

###Keycodes are all set in DSL Input Manager found in the Unity Editor###
<KEYCODES>
	KEYCODE.RETURN is PROCEED | "Proceed" | "Proceed to the next dialogue."
	KEYCODE.UP_ARROW is NAVIGATE_UP | "Up" | "Navigate up".
	KEYCODE.DOWN_ARROW is NAVIGATE_DOWN | "Down" | "Navigate down"
	KEYCODE.LEFT_ARROW is NAVIGATE_LEFT | "Left" | "Navigate left"
	KEYCODE.RIGHT_ARROW is NAVIGATE_RIGHT | "Right" | "Navigate right"
<END>

###--------------------------------------------------------------------------------------------------------------------###

###
	When setting a Dialogue Set to AUTO, the user must use the [HALT] command
	in order to control the flow of automatic progression.
	
	If the command is not use, you will expression a seemless continuation
	of dialogue.
###

###System Messages like Saving, and getting items from treasures###
<DIALOGUE_SET_000 | AUTO | DONT_DISTURB>  
	###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
	###Saving###
	@ [SPEED::NORMAL]You're progress has been saved.[HALT::5000]<<
<END>

<DIALOGUE_SET_001 | AUTO | DONT_DISTURB>
	###Treasure Received on Floor 1 - 200 Crystalsds###
	@ [SPEED::NORMAL]You've obtained [BOLD]200 Crystals[BOLD::END][HALT::2000]<<
<END>

###--------------------------------------------------------------------------------------------------------------------###

###Actual Dialogue in the Game###
<DIALOGUE_SET_002 | AUTO > 
	###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
	
	@??? HO!! Are you approaching me???<<
	@??? Instead of running away you are approaching me?<<
<END>