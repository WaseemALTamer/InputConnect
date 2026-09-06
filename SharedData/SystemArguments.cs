using System;
using System.CommandLine;



namespace InputConnect.SharedData
{
    public static class SystemArguments{

        // this fill will hold the system arguments along with other arguments definitions

        public static readonly Option<bool> HeadlessOption =
            new("--headless", "-H"){
                Description = "Run without displaying the UI"
            };

        public static readonly Option<string?> TestOption =
            new("--test", "-t"){
                Description = "Run a test of the specified type"
            };

        public static bool IsHeadless { get; private set; }
        public static string? TestType { get; private set; }


        public static ParseResult Parse(string[] args){

            var rootCommand = new RootCommand("InputConnect");
            rootCommand.Options.Add(HeadlessOption);
            rootCommand.Options.Add(TestOption);




            var parseResult = rootCommand.Parse(args);

            IsHeadless = parseResult.GetValue(HeadlessOption);
            TestType = parseResult.GetValue(TestOption);
            
            return parseResult;
        }


    }
}
