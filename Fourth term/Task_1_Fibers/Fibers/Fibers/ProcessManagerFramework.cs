namespace Fibers
{
    static class Constants
    {
        public const int MaxPriority = 10;
    }
    public enum Prioritize
    {
        WithPriority,
        WithoutPriority
    }

    public static class ProcessManager
    {
        private static List<Fiber> fiberList = new List<Fiber>();

        private static Prioritize DispatchingAlgorytm;
        public static void Switch(bool fiberFinished)
        {
            if (fiberFinished)
            {
                fiberList.RemoveAt(0);
            }
            if (!fiberList.Any())
            {
#if DEBUG
                Console.WriteLine(string.Format("------- Prime fiber -------"));
#endif

                Fiber.Switch(Fiber.PrimaryId);
            }
#if DEBUG
            Console.Write(string.Format("Fiber [{0}]. ", fiberList[0].Id));
            if (DispatchingAlgorytm == Prioritize.WithPriority)
                Console.Write(string.Format("Priority: {1}. ", fiberList[0].Id, fiberList[0].Priority));
            Console.Write("Status: ");
#endif
            Thread.Sleep(100);
            Fiber.Switch(fiberList[0].Id);
        }

        public static void Execute(List<Process> processList, Prioritize Prioritize)
        {
            DispatchingAlgorytm = Prioritize;

            switch (DispatchingAlgorytm)
            {
                case Prioritize.WithPriority:
                    foreach (var process in processList)
                        fiberList.Add(new Fiber(process.Run, process.Priority));

                    fiberList = fiberList.OrderByDescending(x => x.Priority).ToList();
                    break;

                case Prioritize.WithoutPriority:
                    foreach (var process in processList)
                        fiberList.Add(new Fiber(process.Run));

                    fiberList = fiberList.ToList();
                    break;
            }

            Switch(false);

            foreach (var fiber in fiberList)
            {
                Fiber.Delete(fiber.Id);
            }
        }
    }

    public class Process
    {
        private static readonly Random Rng = new Random();

        private const int LongPauseBoundary = 2000;
        private const int ShortPauseBoundary = 100;
        private const int WorkBoundary = 1000;
        private const int IntervalsAmountBoundary = 4;

        private readonly List<int> _workIntervals = new List<int>();
        private readonly List<int> _pauseIntervals = new List<int>();

        public int Priority { get; private set; }

        public Process()
        {
            int amount = Rng.Next(IntervalsAmountBoundary);

            for (int i = 0; i < amount; i++)
            {
                _workIntervals.Add(Rng.Next(WorkBoundary));
                _pauseIntervals.Add(Rng.Next(
                    Rng.NextDouble() > 0.9
                        ? LongPauseBoundary
                        : ShortPauseBoundary));
            }

            Priority = Rng.Next(Constants.MaxPriority);
        }

        public void Run()
        {
            for (int i = 0; i < _workIntervals.Count; i++)
            {
                Thread.Sleep(_workIntervals[i]); // work emulation
                DateTime pauseBeginTime = DateTime.Now;
                do
                {
#if DEBUG
                    Console.WriteLine(string.Format("{0} / {1} ", i + 1, _workIntervals.Count));
#endif
                    ProcessManager.Switch(false);
                } while ((DateTime.Now - pauseBeginTime).TotalMilliseconds < _pauseIntervals[i]); // I/O emulation
            }
#if DEBUG
            Console.WriteLine($"{TotalDuration} ms total.");
#endif
            ProcessManager.Switch(true);
        }

        public int TotalDuration
        {
            get { return ActiveDuration + _pauseIntervals.Sum(); }
        }

        public int ActiveDuration
        {
            get { return _workIntervals.Sum(); }
        }
    }
}