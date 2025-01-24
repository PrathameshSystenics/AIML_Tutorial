using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore_Tutorial
{
    public class AsyncEnumerable
    {
        public static async Task Main()
        {
            await ProcessData();
        }

        public static async IAsyncEnumerable<int> GetValueOneByOne()
        {

            for (int i = 0; i <= 50; i++)
            {
                Console.WriteLine("From One");
                await Task.Delay(1000);
                yield return i;
            }
        }

        public static async IAsyncEnumerable<int> GetValueOneByTwo()
        {

            for (int i = 0; i <= 50; i++)
            {
                Console.WriteLine("From Two");
                await Task.Delay(1000);
                yield return i * 2;
            }
        }

        public static async Task ProcessData()
        {
            await foreach (var no in GetValueOneByTwo())
            {
                Console.WriteLine(no);
            }
        }


    }
}
