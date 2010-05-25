using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMA.Rhino;

namespace yrender
{
    class Utils
    {
        public static void print(string expr)
        {
            RhUtil.RhinoApp().Print(string.Format("{0}\n", expr));
        }
    }
}
