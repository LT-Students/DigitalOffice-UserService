using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserNameResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        static object CreateObj(Guid id, string firstName)
        {
            return new
            {
                Id = id,
                FirstName = firstName
            };
        }
    }
}