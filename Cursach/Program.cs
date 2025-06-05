namespace AutoServiceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager.InitializeDatabase();
            UserInterface.ShowMainMenu();
        }
    }
}
