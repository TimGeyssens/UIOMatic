using System;

namespace UIOMatic
{
    public class DeleteEventArgs : EventArgs
    {
        public DeleteEventArgs(Type currentType, string[] ids)
        {
            CurrentType = currentType;
            Ids = ids;
        }

        public Type CurrentType { get; set; }

        public string[] Ids { get; set; }
    }
}