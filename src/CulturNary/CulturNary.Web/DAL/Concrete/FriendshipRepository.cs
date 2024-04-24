using CulturNary.DAL.Abstract;
using CulturNary.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CulturNary.DAL.Concrete
{
    public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(CulturNaryDbContext context) : base(context) 
        { 

        }

        public List<Friendship> GetFriendshipByPersonId(int personId)
        {
            return _dbSet
                .Where(f => f.Person1Id == personId || f.Person2Id == personId)
                .ToList();
        }
    }
}