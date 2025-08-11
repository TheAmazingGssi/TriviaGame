using System;

[Serializable]
public class PlayerInformation
{
    public string Name;
    public int Score;
    public float ThinkingTime;
    public int CurrentQuestion;
    public bool WaitingForProgress;
    public bool Active;
}