using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayTimeUtils
{
    public static string FormatPlayTime(long seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
    }
}
