using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderableState
{
    public static float consumePower;
    public static float remainingFuel;

    public static void Initialize()
    {
        remainingFuel = 1;
    }
}
