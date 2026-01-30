using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormEdit
{
    public partial class Creation : ObservableObject
    {
        [ObservableProperty]
        public string _name;
        public ObservableCollection<LuaScript> Scripts { get; set; } = new(); 

        public Creation(string name)
        {
            Name = name;
        }


    }
}
