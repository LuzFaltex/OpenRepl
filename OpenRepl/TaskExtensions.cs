﻿using System.Threading.Tasks;

namespace OpenRepl
{
    public static class TaskExtensions
    {
        public static T AsSync<T>(this Task<T> task, bool configureAwait)
        {
            return task.ConfigureAwait(configureAwait).GetAwaiter().GetResult();
        }

        public static void AsSync(this Task task, bool configureAwait)
        {
            task.ConfigureAwait(configureAwait).GetAwaiter().GetResult();
        }
    }
}
