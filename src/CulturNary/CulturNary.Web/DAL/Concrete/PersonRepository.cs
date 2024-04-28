using CulturNary.DAL.Abstract;
using CulturNary.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CulturNary.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace CulturNary.DAL.Concrete
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private readonly UserManager<SiteUser> _userManager;
        public PersonRepository(CulturNaryDbContext context, UserManager<SiteUser> userManager) : base(context)
        {
            _userManager = userManager;
        }
        public Person GetPersonByIdentityId(string identityId){
            return base.Where(x => x.IdentityId == identityId).FirstOrDefault();
        }
        public async Task<List<SiteUser>> GetUsersWithDietaryRestrictions(FriendSearchModel model, string currentUserId)
        {
            var users = await _userManager.Users
            .Where(user => user.Id != currentUserId)
            .ToListAsync();
            
            return users;
        }
        public Person GetPersonByPersonId(int personId){
            return base.Where(x => x.Id == personId).FirstOrDefault();
        }
    }
}