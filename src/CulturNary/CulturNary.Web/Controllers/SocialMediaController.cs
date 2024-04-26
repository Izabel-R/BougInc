using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CulturNary.Web.Models;
using CulturNary.DAL.Abstract;
using Microsoft.AspNetCore.Authorization;
using CulturNary.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CulturNary.Web.Controllers
{
    [Route("SocialMedia")]
    [Authorize(Roles = "Signed,Admin")]
    public class SocialMediaController : Controller
    {
        private readonly IPersonRepository _personRepository;

        public SocialMediaController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        // GET: SocialMedia/Friends
        [HttpGet("Friends")]
        public async Task<IActionResult> Friends()
        {
            var model = new FriendSearchModel
            {
                Users = new List<SiteUser>()
            };

            return View(model);
        }

        [HttpPost("Friends")]
        public async Task<IActionResult> Friends(FriendSearchModel model)
        {
            if (model == null)
            {
                model = new FriendSearchModel();
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.Users = await _personRepository.GetUsersWithDietaryRestrictions(model, currentUserId);

            // Initialize FriendshipStatus list
            model.FriendshipStatus = new List<string>();

            foreach (var user in model.Users)
            {
                // Check if they are friends
                if (await _personRepository.AreFriends(currentUserId, user.Id))
                {
                    model.FriendshipStatus.Add("Friended");
                }
                // Check if there's a pending friend request
                else if (await _personRepository.IsFriendRequestPending(currentUserId, user.Id))
                {
                    model.FriendshipStatus.Add("Friend Request Pending");
                }
                else
                {
                    model.FriendshipStatus.Add("Send Friend Request");
                }
            }

            model.IsSubmitted = true;
            return View(model);
        }
    }
}