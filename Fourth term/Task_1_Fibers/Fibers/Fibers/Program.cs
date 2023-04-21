namespace Fibers
{
    internal class Program
    {
        public static void Main()
        {
            var processList = new List<Process>();

            for (int i = 0; i < 5; i++)
            {
                processList.Add(new Process());
            }

            ProcessManager.Execute(processList, Prioritize.WithPriority);
            ProcessManager.Execute(processList, Prioritize.WithoutPriority);
        }
    }
}