using System;

namespace ChuckAmuckPlatformer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ChuckAmuckGame game = new ChuckAmuckGame())
            {
                game.Run();
            }
        }
    }
#endif
}

