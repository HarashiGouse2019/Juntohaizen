March 20th was when DSL project began.
March 29th was when DSL was created.

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
	###Treasure Received on Floor 1 - 200 Crystals###
	@ [SPEED::NORMAL]You've obtained [BOLD]200 Crystals[BOLD::END][HALT::2000]<<
<END>

###--------------------------------------------------------------------------------------------------------------------###

###Actual Dialogue in the Game###
<DIALOGUE_SET_002 | AUTO > 
	###The game will proceed to next dialgoue automatically, and player is still allowed to move.###

	@??? [ACTION::"JUMPS"][HALT::500][SPEED::NORMAL] Hey there! [HALT::500]This is Austin. [HALT::500]And welcome to my game.[HALT::750]<< CALL PROMPT 0
		CASE OPTION 1:
			@??? What da faq?<<
			@??? Is this the kind of answer I get?<<
			@??? That freaking sucks.<<
			@??? That was the worst thing I have ever tasted...<<
			@??? And now... my day is ruined.<<
		BREAK
		
		CASE OPTION 2:
			@??? Ha wa ha wa ha wa!!!<<
			@??? Feast your eyes upon your gracefulness.<<
			@??? Is that even an actual thing?<<
			@??? No! I mean yes!!! It is an actual thing!<<
			@??? Did you... Just lie?<<
			@??? How could a kind like me lie in such a way?<< CALL PROMPT 4
				CASE OPTION 1:
					@??? I knew it.<<
					@??? I wouldn't kill him because of it though...<<
					@??? Unless if it were to harm the people.<<
					@??? Then I would not hesistate to have the man executed.<<
					@??? But of course... He doesn't need to know that.<<
					
					@??? Know about what?<<
					
					@??? Nothing sir! Just thinking to myself.<<
					
					@??? Well quite thinking, and start NOTICING ME SENPAI!!!!<<
				BREAK
				
				CASE OPTION 2:
					@??? You would never lie my good sir!<<
					
					@??? Excellent!!!<<
				BREAK
				
				CASE OPTION 3:
					@??? RANDOM STUFF<<
				BREAK
			OUT
			@??? Now, with that out of the way, let's enjoy ourselves.<<
			@??? It'd be a shame if our whole entire day was ruined.<<
			@??? Now that would be absurd!<<
			@??? Absolutely absurd!<<
			@??? Yes!!<<
		BREAK
		
		CASE OPTION 3:
			@??? WARIO!!!!<<
		BREAK
		
		CASE OPTION 4:
			@??? WALUIGI!!!<<
		BREAK
	OUT
	
	@??? [ACTION::"GASPS"][HALT::500] I see that you are doing well.[HALT::750]<< CALL PROMPT 1
		CASE OPTION 1:
			@??? What da faq?<<
		BREAK
	OUT
	
	@Austin Yeah! [HALT::1000]More or less.[HALT::750]<<
	@Austin Gotta go clear out this area. [HALT::750]It's absoulutely dreadful over here.[HALT::750]<< CALL PROMPT 2
		CASE OPTION 1:
			@??? What da faq?<< CALL PROMPT 3
				CASE OPTION 1:
					@??? DO DA FAQ WHAT?!?!?!<<
					@??? DO DA FAQ WHAT?!?!?!<<
				BREAK
				@??? Crazy huh?!<<
				@??? CRAZY HUH?!<<
				@??? PUT THEM HANDS UP!<<
				@??? PUT THEM PAWS UP!!!<<
			OUT
			@??? Alrighty then. Sorry that I got so salty.<<
			@??? It won't happen again my dude.<<
			@??? Hello internet, and welcome to best trends.<<
			@??? Today, we'll be going over...<<
			@??? 5 Fortnite Youtubers... who've sworn...<<
		BREAK
		
		CASE OPTION 2:
			@??? NANI DA FAQ???<<
			@??? I have no idea what that's even suppose to mean.<<
			@??? But I guess that is okay. I don't need anyone's help after all of this.<<
		BREAK
	OUT

	@??? Oh...[HALT::2000]<<
	@??? You should [INSERT::"uh..."] [HALT::1000]probably see how [BOLD]Mary[BOLD::END] is doing.[HALT::2000]<<

	@Austin I have no idea where she is...<<

	@??? [SPEED::FASTEST]WELL, GO FIND HER, GOSH DARN IT!!![HALT::1000]<<

	@Austin [SPEED::NORMAL][INSERT::"Geez!"] [HALT::750][INSERT::"ALRIGHT!"] [HALT::500][INSERT::"ALRIGHT!"] [HALT::1000]I'll go find her.[HALT::1500]<<
	@Austin [HALT::1000]Jackass...[HALT::500]<<
	
<END>

###-----------------------------------------------------------------------------------------------------------------------###

<DIALOGUE_SET_003 | AUTO | DONT_DISTURB>
	###The game will proceed to next dialgoue automatically, and player is still allowed to move.###
	###This is just something that I experimented with on the spot. I want to have the Basic-DSL parser something like this.###

	@??? [ACTION::"JUMPS"][HALT::500][SPEED::NORMAL]Hey there! [HALT::500]This is Austin. [HALT::500]And welcome to my game.[HALT::750]<<

	@??? [ACTION::"GASPS"] [HALT::500]I see that you are doing well.[HALT::750]<<

	@Austin Yeah! [HALT::1000]More or less.[HALT::750]<<
	@Austin Gotta go clear out this area. [HALT::750]It's absoulutely dreadful over here.[HALT::750]<<

	@??? Oh...[HALT::2000]<<
	@??? You should [INSERT::"uh..."] [HALT::1000]probably see how [BOLD]Mary[BOLD::END] is doing.[HALT::2000]<<

	@Austin I have no idea where she is...<<

	@??? [SPEED::FASTEST]WELL, GO FIND HER, GOSH DARN IT!!![HALT::1000]<<

	@Austin [SPEED::NORMAL][INSERT::"Geez!"] [HALT::750][INSERT::"ALRIGHT!"] [HALT::500][INSERT::"ALRIGHT!"] [HALT::1000]I'll go find her.[HALT::1500]<<
	@Austin [HALT::1000][ITALIZE]Jackass...[ITALIZE::END][HALT::500]<<

	@Austin What should I do?<< CALL PROMPT 0
		###Basic-DSL must in fact keep track with how many tabs there are, and be able to follow all of this accordingly.
		You want to think of this in this way:
		
		In PROMPT 0, what position is OPTION 1? If I chose Prompt 0, Option 1, Jump to the line after it.
		This might help you.###
		CASE OPTION 1: ###If first option was selected...###
			@Austin Should I really be making myself home?<<
			@Austin [HALT::3000]You know what...<<
			@Austin I think I will make myself home...<<
			@Austin Alright! Let's get the bread!!!<<
		BREAK
	
		###With the stylers for Basic-DSL, it must be about to display these options as buttons###
		CASE OPTION 2: ###If second option was selected...###
			@Austin I don't think... That even helps...<<
			@Austin But really, I don't know what I should even do.<<
			@Austin This is why I should of became an accountant instead of being in this...<<
			@Austin Game thing...<<
			@Austin What even is this game suppose to be about??<<
			@Austin I guess we'll never know for sure.<<
		BREAK
	
		###When you click on an option, the Dialogue System should have an object that it must access
		like for instance, a Prompt class and a Options class to keep track of what prompt associates with
		which options.###
		CASE OPTION 3: ###If third option was selected...###
			@Austin What does the national anthem have to do with this?<<
			@Austin Whatever. I think I know what I gotta do from here on.<<
			@Austin And that is... to be Austin.<<
		BREAK
	
		###It gets even more complicated with nested options/prompts.
		Basic-DSL must understand how to do that.###
		CASE OPTION 4: ###If fourth option was selected...###
			@Austin A thing?<<
			@Austin [HALT::2000]You... [HALT::1000]want me... [HALT::1000]to do a thing?<< CALL PROMPT 1
				CASE OPTION 1: ###If first option was selected...###
					@Austin Don't you fuck with me like that.<<
					@Austin That's not very nice. Seriously man...<<
					@Austin I really need to know what I'm suppose to do.<<
					@Austin Otherwise... I might just flip out.<< CALL PROMPT 0 | OMIT PARENT OPTION
				BREAK
		
				CASE OPTION 2: ###If second option was selected...###
					@Austin Oh!<<
					@Austin Well okay... Then what should I do?<< CALL PROMPT 0 | OMIT PARENT OPTION ###This is a Recursive Prompt Call, but when displaying the options, it will grey out the 4th option###
				BREAK
			OUT
		BREAK
	OUT
	###This is just a short project to do in the future in order to give Basic-DSL more functionality.
	We must also figure how to implement Events as well regarding Unity x Basic-DSL###
<END>

###Along with the project, there's new tags to read. We learned how to do this for
KeyCodes and Dialogue Sets. With this, we'll create a Prompt object with a List of Options.
One Prompt object (when called) will display a set of buttons based on the configuration of choices.
whether that'd be Horizontal Choices or Quad Choices.###
<PROMPT_000>
	OPTION 1 >> Make yourself at home.
	OPTION 2 >> Do whatever.
	OPTION 3 >> Sing the national anthem.
	OPTION 4 >> Do a thing.
<END>

###You can also choose to explicitly set a capacity for the prompt.
The default amount is 4, but you can override the size of Options with the
keyword "CAPACITY" and setting a number###
<PROMPT_001 | CAPACITY is 2>
	OPTION 1 >> Yes!
	OPTION 2 >> No. I'm was joking.
<END>

<PROMPT_002 | CAPACITY is 8>
	OPTION 1 >> This is Option 1.
	OPTION 2 >> This is Option 2.
	OPTION 3 >> This is Option 3.
	OPTION 4 >> This is Option 4.
	OPTION 5 >> This is Option 5.
	OPTION 6 >> This is Option 6.
	OPTION 7 >> This is Option 7.
	OPTION 8 >> This is Option 8.
<END>

<PROMPT_003 | CAPACITY is 3>
	OPTION 1 >> KYAAA!!!
	OPTION 2 >> GAAAAAAAA!
	OPTION 3 >> ...
<END>

<PROMPT_004 | CAPACITY is 2>
	OPTION 1 >> He is lying! Kill the man!
	OPTION 2 >> He is completely innocent!
<END>