using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Zero.Application.Config
{
    public class ZeroOption
    {
        public string Connection { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public int AuthExpired { get; set; }
    }
}
