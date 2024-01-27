using KBot.State;

namespace KBot.Factories
{
    public class BotFactory
    {
        int Size = 64;

        public BotFactory(int size=64) 
        {
            Size = size;
        }
    }

    public static class Factories
    {
        private static BotFactory __BotFactory = null;

        public static BotFactory BotFactory => __BotFactory ??= new();
    
        
    }
}
