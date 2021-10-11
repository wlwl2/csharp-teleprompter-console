// `using static` statement imports methods from one class.
// `using` imports all classes from a namespace.
using static System.Math;

// C# uses namespaces to organize types
namespace TeleprompterConsole {
    internal class TelePrompterConfig {
        private object lockHandle = new object();
        public int DelayInMilliseconds { get; private set; } = 200;

        // negative increment to speed up
        public void UpdateDelay (int increment) {
            int newDelay = Min(DelayInMilliseconds + increment, 1000);
            newDelay = Max(newDelay, 20);

            // Ensures that only a single thread can be in that code at any
            // given time. If one thread is in the locked section, other threads
            // must wait for the first thread to exit that section. The lock
            // statement uses an object that guards the lock section. This class
            // follows a standard idiom to lock a private object in the class.
            lock (lockHandle) {
                DelayInMilliseconds = newDelay;
            }
        }

        public bool Done { get; private set; }

        public void SetDone () {
            Done = true;
        }
    }
}
