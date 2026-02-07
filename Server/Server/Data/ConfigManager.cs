using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Data
{
    public class ConfigManager
    {
        private static ConfigManager instance = new ConfigManager();
        public static ConfigManager Instance => instance;
    }
}