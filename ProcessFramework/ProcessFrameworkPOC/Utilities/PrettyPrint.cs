namespace ProcessFrameworkPOC.Utilities
{
    public static class PrettyPrint
    {
        public static void Print(string message, ConsoleColor backgroundcolor, ConsoleColor foregroundcolor)
        {
            Console.BackgroundColor = backgroundcolor;
            Console.ForegroundColor = foregroundcolor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
