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
KEYCODE.RETURN is PROCEED

###--------------------------------------------------------------------------------------------------------------------###
###System Messages like Saving, and getting items from treasures###
<DIALOGUE_SET_000 | AUTO | DONT_DISTURB>  ###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
###Saving###
@ [SPEED::NORMAL]You're progress has been saved.<<
<END>

<DIALOGUE_SET_001 | AUTO | DONT_DISTURB>
###Treasure Received on Floor 1 - 200 Crystals###
@ [SPEED::NORMAL]You've obtained [BOLD]200 Crystals[BOLD::END]<<
<END>

###--------------------------------------------------------------------------------------------------------------------###
###Actual Dialogue in the Game###
<DIALOGUE_SET_002 | AUTO | DONT_DISTURB> ###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
@Hazel [SPEED::NORMAL]Welcome to my game.<<
@Mori I see that you are doing well.<<
<END>

<DIALOGUE_SET_003>

<END>

<DIALOGUE_SET_004>

<END>

<DIALOGUE_SET_005>

<END>