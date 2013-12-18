using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollyon
{
    class WindowManager
    {
        public static List<ApWindow> Windows = new List<ApWindow>();
        public static ApWindow GetWindowByName(string _name)
        {
            return Windows.Find(x => x.Name.Equals(_name));
        }
    }
}
