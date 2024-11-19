using System;
using System.Collections.Generic;
using OpenCvSharp;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    public class ResourceHelper
    {
        private ResourceManager _rm;
        public ResourceHelper(string resourcePath)
        {
            _rm = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
        }

        public string this[string resourceName]
        {
            get
            {
                var retVal = _rm.GetString(resourceName);
                if (retVal == null)
                    retVal = String.Empty;
                return retVal;
            }
        }

        public Point Point(string resourceName)
        {
            var coordinates = this[resourceName];
            var positions = coordinates.Split(',');
            return new Point(Int32.Parse(positions[0]), Int32.Parse(positions[1]));
        }
    }
}
