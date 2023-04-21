namespace Fibers
{
    public static class ProcessManager
    {
        private static List<Fiber> fiberList = new List<Fiber>();

        public static void Switch(bool fiberFinished)
        {
            if (fiberFinished)
            {
                fiberList.RemoveAt(0);
            }
            if (!fiberList.Any())
            {
                Fiber.Switch(Fiber.PrimaryId);
            }

            Fiber.Switch(fiberList[0].Id);
        }

        public static void Execute(List<Process> processList)
        {
            foreach (var process in processList)
            {
                fiberList.Add(new Fiber(process.Run));
            }

            fiberList = fiberList.ToList();

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
        private const int IntervalsAmountBoundary = 10;

        private readonly List<int> _workIntervals = new List<int>();
        private readonly List<int> _pauseIntervals = new List<int>();

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
            Console.WriteLine(TotalDuration);
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