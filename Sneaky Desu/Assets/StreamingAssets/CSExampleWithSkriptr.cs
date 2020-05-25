[ExposeObjectAs("Emotions")]
public class Emotions
{
	public enum Type
	{ 
		NEUTRAL,
		HAPPY,
		SAD,
		ANGRY,
		CONFUSED
	}
	
	[ExposeFieldAs("emoType")]
	public Type emotionType{get;set;}
	
	[ExposeFieldAs("emoVal")]
	public int emotionValue{get;set;}
}

public abstract class Character
{
		[ExposeFieldAs("name")]
		protected string characterName = string.Empty();
		
		[ExposeFieldAs("currentEmotion")]
		protected Emotions.Type currentEmotion = Emotions.Type.NEUTRAL;
		
		[ExposeMethodAs("changeEmotion")]
		protected void ChangeEmotion(Emotions.Type emotionType)
		{
				currentEmotion = emotionType;
		}
}

[ExposeObjectAs("MaryCharacter")]
public class MaryCharacter : Characters
{
		
}

[ExposeObjectAs("HazelCharacter")]
public class HazelCharacter : Characters
{
	
}

[ExposeObjectAs("MoriCharacter")]
public class MoriCharacter : Characters
{
	
}