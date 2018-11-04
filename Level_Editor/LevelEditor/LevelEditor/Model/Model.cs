using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class Model
    {
        //Adjustable ints for the height/width of levels in tiles
        public static int tileH;
        public static int tileW;

    }

    /// <summary>
    /// Simple helper class for 3D coordinates 
    /// </summary>
    public class Triple
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Triple(int a, int b, int c)
        {
            x = a;
            y = b;
            z = c;
        }
    }

}
