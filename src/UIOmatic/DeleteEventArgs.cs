using System;

namespace UIOMatic
{
    public class DeleteEventArgs : EventArgs
    {
        public DeleteEventArgs(Type currentType, string tableName, string[] ids)
        {
            CurrentType = currentType;
            TableName = tableName;
            Ids = ids;
        }

        public Type CurrentType { get; set; }

        public string TableName { get; set; }

        public string[] Ids { get; set; }
    }
}