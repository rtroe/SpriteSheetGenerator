using System;
using System.Collections.Generic;

namespace SpriteSheetGenerator
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Sprite Gen - Light Weight Sprite Sheet Generator");
            Console.WriteLine("======================================");
            Console.WriteLine("Specifiy the following arguments: ");
            Console.WriteLine("-w [width] -h [height] -dir [path/to/files] -output [path/to/output/dir]");

            var args = Environment.GetCommandLineArgs();
            
            List<string> launchArgs = new List<string>();

            foreach (var arg in args)
                launchArgs.Add(arg);

            // If there's one or less args, then 
            if(launchArgs.Count <= 1)
            {
                string input = Console.ReadLine();

                var newArgs = input.Split(' ');

                foreach (var arg in newArgs)
                    launchArgs.Add(arg);
            }
            else
            {
                Console.Write("args: ");
                for (int i = 1; i < args.Length; i++)
                    Console.Write(args[i] +" ");

                Console.WriteLine("\nStarting GUI...");
            }

            using (var game = new SpriteSheetGenerator(launchArgs.ToArray()))
                game.Run();
        }
    }
}
