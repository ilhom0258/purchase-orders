using OberMind.PurchaseOrders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OberMind.PurchaseOrders.Application.DTOs
{
    public class UserDTO
    {
        public string Name { get; set; }
        public static UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                Name = user.Name
            };
        }
    }
}
