namespace Silgred.Win.Pages
{
    public static class PageHelper
    {
        public static JoinSessionPage JoinSessionPage { get; private set; } = new JoinSessionPage();
        public static StartPage StartPage { get; } = new StartPage();
        public static StartSessionPage StartSessionPage { get; private set; } = new StartSessionPage();

        public static void Init()
        {
            JoinSessionPage = new JoinSessionPage();
            StartSessionPage = new StartSessionPage();
        }
    }
}