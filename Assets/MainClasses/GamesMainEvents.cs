using System;
using System.Collections;
using System.Collections.Generic;

public static class GamesMainEvents
{
    public static float audioLatency = 0.75f;
    public static int turnLength = 16;

    public static Action WinBattle;

    public static Action LoseBattle;

    public static Action StartBattle;

    public static Action<Character> InvokeDamageUIChanges;

    public static Action<string> UpdateOnBeat;
    public static Action<Character> transferCharacter;

    public static void SetAudioLatency(float delay)
    {
        audioLatency = delay;
    }

}
