using System;

namespace ColdWarPrototype2.Gui
{
    public class ProvinceEventArgs : EventArgs
    {
        public int ProvinceId { get; }
        public ProvinceEventArgs(int id) { ProvinceId = id; }
    }

    public class CountryEventArgs : EventArgs
    {
        public string CountryTag { get; }
        public CountryEventArgs(string tag) { CountryTag = tag; }
    }
}
