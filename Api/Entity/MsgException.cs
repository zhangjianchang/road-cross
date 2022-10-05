using System;

namespace Api.Entity
{
    public class MsgException : Exception
    {
        public MsgException(string msg) : base(msg)
        {
        }
    }
}
