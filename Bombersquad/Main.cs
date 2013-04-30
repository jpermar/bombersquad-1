using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bombersquad_ai
{
    class Program
    {
        static void Main(string[] args)
        {
            // Call Game Engine
            using (var monoGame = new GameEngine())
                monoGame.Run();
        }
    }
}
