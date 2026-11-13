using UnityEngine;

public static class UnityHelper
{
    public static T Unwrap<T>(this T self) where T : Component
    {
        if (self == null)
            return null;
        return self;
    }
}
