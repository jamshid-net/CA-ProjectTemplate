using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTemplate.Domain.Entities.Auth;

namespace ProjectTemplate.Domain.Events;
public class UserCreatedEvent(User user) : BaseEvent
{
    public User User => user;
}
