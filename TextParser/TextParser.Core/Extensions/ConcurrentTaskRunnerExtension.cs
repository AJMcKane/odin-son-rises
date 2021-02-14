using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TextParser.Core.Extensions {
    public static class ConcurrentTaskRunnerExtension {
        public static async Task<IList<TResult>> FunctionCollectionWithMaxConcurrency<T, TResult>(this IEnumerable<T> collection, Func<T, Task<TResult>> functionTask, int maxConcurrency) {
            var taskList = new List<Task<TResult>>();
            foreach (var item in collection) {
                var task = functionTask(item);
                taskList.Add(task);
                if (taskList.Count(p => !p.IsCompleted) >= maxConcurrency) {
                    await Task.WhenAny(taskList.Where(p => !p.IsCompleted).ToArray());
                }
            }

            await Task.WhenAll(taskList.ToArray());
            return taskList.Select(p => p.Result).ToList();
        }
    }
}
