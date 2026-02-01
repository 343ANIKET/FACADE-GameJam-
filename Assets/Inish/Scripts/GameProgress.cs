public static class GameProgress
{
    // Progress flags
    public static bool HasMask = false;

    // Optional (future use)
    public static int CurrentLevel = 1;

    // Call this only if you want a NEW GAME
    public static void ResetProgress()
    {
        HasMask = false;
        CurrentLevel = 1;
    }
}
