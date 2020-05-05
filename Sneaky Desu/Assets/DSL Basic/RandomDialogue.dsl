TITLE: "Hello There"

AUTHOR: "MIIJII"

DATE: "APRIL 28, 2020"

DESCRIPTION: "A scripting language that'll help you add dialogue to your game easily and swiftly."

<CHARACTERS>
Miijii is Me
Zack
Justyn
Christian

<EXPRESSIONS>
MIIJII_EXPRESSION_NEUTRAL = 0
MIIJII_EXPRESSION_HAPPY = 1
MIIJII_EXPRESSION_SAD = 2
MIIJII_EXPRESSION_MAD = 3
ZACK_EXPRESSION_NEUTRAL = 4
ZACK_EXPRESSION_HAPPY = 5
ZACK_EXPRESSION_SAD = 6
ZACK_EXPRESSION_MAD = 7
CHRISTIAN_EXPRESSION_NEUTRAL = 8
CHRISTIAN_EXPRESSION_HAPPY = 9
CHRISTIAN_EXPRESSION_SAD = 10
CHRISTIAN_EXPRESSION_MAD = 11

<POSES>
### No use for poses here ###

<SCENES>
### No use for scenes here ###


###
The starting of dialogue.
Just like a normal story, paragraphs are seperated with 
###

<DIALOGUE_SET_000>
@ Looks like [HALT::750]we gots a problem.<<
@ These people seems to be behind on a couple of things.<<
@ That's [HALT::500]perhaps [HALT::500]not a good thing.<<
@ Miijii [HALT::750](just a pen-name) [HALT::500]walks up to one of them to see if they are okay.<<

@??? Gosh dang it Daniel!<<
@Me My name isn't Daniel.<<
@??? I know.<<

@Me What seems to be the problem?<<
@??? I'm super behind on one of my projects, and I don't know what I'll be able to get it done on time.<<
@??? I honestly just feel like giving up.<<

@Me Yeah! [HALT::500]I know how that feels. [HALT::750]It's awful.<<
@Me Is there anything in particular that you are having trouble with?<<

@??? Hmmmmmmm...<<
@??? First off, [HALT:500]I'm having some trouble with my [BOLD]Inverse Kimatics.[BOLD::END]<<
@??? It's just not working the way it should.<<
@??? Also, I can't seem to get my [BOLD]enemy AI[BOLD::END] to work correctly.<<
@??? They won't go after the player, and they always end up shooting each other whenever they are at a close range...<<
@??? What should I do?[PROMPT::0]<<
<END>

<DIALOGUE_SET_001>
@Me [insert ANSWER::1 from PROMPT::0]<< ###From Prompt 0, we'll read off the first choice###
@??? Sha dude! Thanks! I really need the help!
<END>

<DIALOGUE_SET_002>
@Me [insert ANSWER::2 from PROMPT::0]<< ###From Prompt 0, we'll read off the second choice###
@??? Cheat codes?<<

@Me Yeah! The cheat codes.<<
@??? You mean like the forums?<<
@Me Yeah! Check up on a few forums. 
@Me Don't worry though. I'll help you out with this.<<
@Me We'll get you caught up in no time.<<

@??? That's good to hear. Thanks bro.<<
<END>

<DIALOGUE_SET_003>
@Me [insert ANSWER::3 from PROMPT::0]<< ###From Prompt 0, we'll read off the third choice###
@??? I checked the code many times.<<
@??? I used Debug.Log to check if the enemy is referencing the player's position...<<
@??? and I also had the "target" variable exposed in the inspector to see if it was taking any values...<<
@??? but for some reason, it's not getting anything.

@Me Don't worry. We'll take a look at it. It might just be a little mistake in code that you looked over.<<
@Me I mean, that is the most common problem that us programmers go through.<<

@??? I really appreciate it man! I really need the help.<<
<END>

<DIALOGUE_SET_004>
@Me [insert ANSWER::4 from PROMPT::0]<< ###From Prompt 0, we'll read off the fourth choice###
@??? Well, I have to have all of this turned in by Friday. I can't exactly do nothing.<<

@Me No. What I mean was that I'll come in and help you! I'll walk you through this every step of the way.<<
@??? That's pretty freaking cool of you bro! [INSERT::"XD"] You're the best!<<
<END>

###Just thinking on how EVENTS should be handled###

<PROMPT_000 : CAPACITY is 4>
1 >> Don't worry partner! We'll go through this together! | JUMP to 001
2 >> Have you check the cheat codes? | JUMP to 002
3 >> Did you check if they were actually chasing the player? | JUMP to 003
4 >> Do absolutely nothing! | JUMP to 004