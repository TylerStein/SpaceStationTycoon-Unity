using System.Collections.Generic;

namespace SST.Utilities
{
    [System.Serializable]
    public class HistoryTrackingUtility {
        public static List<T> CreateEmpty<T>(int length, T defaultValue) {
            List<T> value = new List<T>();
            for (int i = 0; i < length; i++) value.Add(defaultValue);
            return value;
        }

        public static T Peek<T>(List<T> history, T defaultValue) {
            if (history.Count == 0) return defaultValue;
            return history[history.Count - 1];
        }

        public static void Push<T>(ref List<T> history, T value) {
            for (int i = 0; i < history.Count - 1; i++) {
                history[i] = history[i + 1];
            }
            history[history.Count - 1] = value;
        }

    }
}