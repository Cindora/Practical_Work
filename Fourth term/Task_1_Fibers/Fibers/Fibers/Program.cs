namespace Fibers
{
    internal class Program
    {
        public static void Main()
        {
            var processList = new List<Process>();

            for (int i = 0; i < 3; i++)
            {
                processList.Add(new Process());
            }

            ProcessManager.Execute(processList);
        }
    }
}