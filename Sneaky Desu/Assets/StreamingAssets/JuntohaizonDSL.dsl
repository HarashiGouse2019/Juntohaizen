TITLE: "JuntohaizenDSL"

AUTHOR: "Miijii"

DATE: "05/05/2020"

DESCRIPTION: "The basic dialogue that will be seen in the game, Juntohaizen."

<CHARACTERS>
Mary
Hazel
Mori
Tray
Globby
Merchant
God

###Keycodes are all set in DSL Input Manager found in the Unity Editor###
<KEYCODES>
KEYCODE.X is PROCEED | "Proceed" | "Proceed to the next dialogue."
KEYCODE.Y is PRESET_0 | "Something" | "We don't know what this does."
KEYCODE.Z is PRESET_1 | "FIRE" | "You fire shit with it."

###--------------------------------------------------------------------------------------------------------------------###

###
	When setting a Dialogue Set to AUTO, the user must use the [HALT] command
	in order to control the flow of automatic progression.
	
	If the command is not use, you will expression a seemless continuation
	of dialogue.
###

###System Messages like Saving, and getting items from treasures###
<DIALOGUE_SET_000 | AUTO | DONT_DISTURB>  ###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
###Saving###
@ [SPEED::NORMAL]You're progress has been saved.[HALT::5000]<<
<END>

<DIALOGUE_SET_001 | AUTO | DONT_DISTURB>
###Treasure Received on Floor 1 - 200 Crystals###
@ [SPEED::NORMAL]You've obtained [BOLD]200 Crystals[BOLD::END][HALT::2000]<<
<END>

###--------------------------------------------------------------------------------------------------------------------###
###Actual Dialogue in the Game###
<DIALOGUE_SET_002> ###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
@ [SPEED::NORMAL]Welcome to my game.[HALT::750]<<

@ I see that you are doing well.[HALT::750]<<

@ Yeah! [HALT::1000]More or less.[HALT::750]<<
@ Gotta go clear out this area. [HALT::750]It's absoulutely dreadful over here.[HALT::750]<<

@ Oh...[HALT::2000]<<
@ You should probably see how [BOLD]Mary[BOLD::END] is doing.[HALT::2000]<<

@ I have no idea where she is...<<

@ [SPEED::FASTEST]WELL, GO FIND HER, GOSH DARN IT!!![HALT::1000]<<

@ [SPEED::NORMAL]Geez! Alright. Alright. I'll go find her.[HALT::1500]<<
@ [ACTION::"Whispers"][HALT::1000]Jackass...[HALT::250]<<
<END>

<DIALOGUE_SET_003>

<END>

<DIALOGUE_SET_004>

<END>

<DIALOGUE_SET_005>

<END>