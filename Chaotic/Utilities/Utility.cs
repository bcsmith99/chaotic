using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    internal class Utility
    {
        internal static string ResourceLocation(string resolution, string folderName = "")
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return $"Resources/Assets/{resolution}/";
            else
                return $"Resources/Assets/{resolution}/{folderName}/";
        }
        internal static string ImageResourceLocation(string imgName, string resolution, string folderName = "")
        {
            return $"{ResourceLocation(resolution, folderName)}{imgName}"; 
        }
    }
}
