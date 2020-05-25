<begin SMProxy>

<title> JuntohaizenDSL </title>

<author> Miijii </author>

<date> 05/05/2020 </date>

<description> The basic dialogue that will be seen in the game, Juntohaizen. </description>


//I don't even know how I'm going to get this to work...
//I mean... I have a base... but it's not that even good...

/*And then we have this...
A multi comment thingy majig...
now how is that going to work???*/
<characters>
	Mary
	Hazel
	Mori
	Tray
	Globby
	Merchant
	Austin
	God
</characters>

/*Keycodes are all set in DSL Input Manager found in the Unity Editor*/
<keycodes>
	KEYCODE.RETURN is PROCEED | "Proceed" | "Proceed to the next dialogue."
	KEYCODE.UP_ARROW is NAVIGATE_UP | "Up" | "Navigate up".
	KEYCODE.DOWN_ARROW is NAVIGATE_DOWN | "Down" | "Navigate down"
	KEYCODE.LEFT_ARROW is NAVIGATE_LEFT | "Left" | "Navigate left"
	KEYCODE.RIGHT_ARROW is NAVIGATE_RIGHT | "Right" | "Navigate right"
</keycodes>

//Binding objects


/*--------------------------------------------------------------------------------------------------------------------*/

/*
	When setting a Dialogue Set to AUTO, the user must use the [HALT] command
	in order to control the flow of automatic progression.
	
	If the command is not use, you will expression a seemless continuation
	of dialogue.
*/

//System Messages like Saving, and getting items from treasures
<dialogueSet 0 | auto | dontDetain>  
	/*The game will proceed to next dialgoue automatically, and player is still allowed to move.
	Saving*/
	@ [speed::NORMAL]You're progress has been saved.[halt::5000]<<
</dialogueSet>

<dialogueSet 1 | auto | dontDetain>
	//Treasure Received on Floor 1 - 200 Crystals
	@ [speed::NORMAL]You've obtained [bold]200 Crystals[/bold][halt::2000]<<
</dialogueSet>

/*--------------------------------------------------------------------------------------------------------------------*/

//Actual Dialogue in the Game
<dialogueSet 2 | auto>

	@ [speed::NORMAL]Welcome to my game.[halt::1000]<<

	@ I'll tell you this right now... [halt::750]but you are trapped here.[halt::750]<<
	@ What is this place you asked??? [halt::1000]well... [halt::750]this is my castle.[halt::750]<<
	@ You feel coozy don't you?[halt::750]<<

	@ I say welcome to my humble abode.[halt::1000]<<
	@ Ohhhh...[halt::750]<<
	@ So you want out?[halt::750]<<

	@ Well, I'm sorry to say this but... [halt::1000]like I said... you're trapped. [halt::500]just like everyone else.[halt::750]<<
	@ Good luck trying to find the way out, because you'll never see the light of day.[halt::1000]<<

</dialoguSet>