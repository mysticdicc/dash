using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danklibrary.Network
{
    public class SubnetRowState
    {
        public bool Hidden { get; set; }
        public string? SearchTerm { get; set; }
        public bool FilterRowHidden { get; set; }
        public bool IcmpFilterEnabled { get; set; }
        public bool TcpFilterEnabled { get; set; }
        public enum FilterByOption { And, Or }
        public FilterByOption FilterBy { get; set; }
    }
}
