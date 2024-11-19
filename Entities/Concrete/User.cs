using Core.Entities.Concrete;

namespace Entities.Concrete
{
    public class User : AppUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
