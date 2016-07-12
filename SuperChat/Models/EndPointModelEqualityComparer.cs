using System.Collections.Generic;
using SuperChat.Models.Chats.Base;

namespace SuperChat.Models
{
    public class EndPointModelEqualityComparer : IEqualityComparer<EndPointModel>
    {
        public bool Equals(EndPointModel x, EndPointModel y)
        {
            return Equals(x.EndPoint.Address, y.EndPoint.Address);
        }

        public int GetHashCode(EndPointModel obj)
        {
            return obj.EndPoint.Address.GetHashCode();
        }
    }
}