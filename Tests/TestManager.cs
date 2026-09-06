using System.Collections.Generic;
using System;






namespace InputConnect.Tests
{



    public static class TestManager{


        // you will have to add almost all the tests manually into the dictionary since each test is going to be
        // its own class 
        private static readonly Dictionary<string, ITest> Tests =
            new(StringComparer.OrdinalIgnoreCase){
                { "connect", new ConnectionTest() }
            };










        public static void Run(string testName){
            if (!Tests.TryGetValue(testName, out var test)){
                Console.WriteLine($"Unknown test: {testName}");
                Console.WriteLine("Available tests:");

                foreach (var name in Tests.Keys){
                    Console.WriteLine($"  {name}");
                }

                return;
            }
            
            int status_code = test.Initialize();
            Console.WriteLine($"Test: {test.Name} returned code {status_code}");


            Controllers.Hook.GlobalHook.Dispose();
            Environment.Exit(0);


        }


    }
}
