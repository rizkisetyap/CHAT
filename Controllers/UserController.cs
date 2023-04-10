using Chat.Data;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register(string Nama, string Email, string Password)
        {
            try
            {
                var user = new User();
                user.Nama = Nama;
                user.Email = Email;
                user.NormalizedEmail = Email;
                user.UserName = Email;
                var result = await _userManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    return Ok(true);

                }
                else
                {
                    throw new Exception("Failed");
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AuthLogin(string Email, string Password)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(Email, Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    throw new Exception("Invalid Username / password");
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}
