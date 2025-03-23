using System;

public static class EventManager
{
    public static event Action StopMove;

    public static void InvokeStopMove()
    {
        StopMove?.Invoke();
    }

    public static event Action ZoneTriggered;

    public static void InvokeZoneTriggered()
    {
        ZoneTriggered?.Invoke();
    }

    public static event Action Destroy;

    public static void InvokeDestroy()
    {
        Destroy?.Invoke();
    }
}