using System;
using UnityEngine;

namespace SF.DataModule
{
    public class DataProperty : PropertyAttribute
    {
        public Type DataType;
        public DataProperty(Type dataType)
        {
            DataType = dataType;
        }
    }
}
