using System;

namespace UIOMatic
{
    public class DeleteEventArgs : EventArgs
    {
        public DeleteEventArgs(Type objectType, string[] ids)
        {
            ObjectType = objectType;
            Ids = ids;
        }

        public Type ObjectType { get; set; }

        public string[] Ids { get; set; }
    }
}