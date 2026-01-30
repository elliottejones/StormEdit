using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormEdit
{
    public partial class LuaScript: ObservableObject
    {
        [ObservableProperty]
        public string _name;

        private int id;
        
        public LuaScript(int id, string name)
        {
            this.id = id;
            Name = name;
        }
    }
}
