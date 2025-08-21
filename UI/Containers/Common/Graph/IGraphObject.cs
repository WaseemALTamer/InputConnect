using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.UI.Containers.Common
{
    public interface IGraphObject
    {

        // this class needs to be attached to an object before you try to place it on a graph
        // this will insure that the master graph can handel the object and  while placing it
        // and moving it when you need to


        public double? X { get; set; }
        public double? Y { get; set; }

        public double? GraphWidth { get; set; }
        public double? GraphHeight { get; set; }
    }
}
