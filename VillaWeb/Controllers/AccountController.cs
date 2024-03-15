using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Domain.Entities;
using VillaWeb.ViewModels;

namespace VillaWeb.Controllers;

public class AccountController : Controller
{
    #region DE

    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    #endregion

    #region LOGIN

    public IActionResult Login(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        LoginVM loginVM = new()
        {
            RedirectUrl = returnUrl
        };

        return View(loginVM);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager
                .PasswordSignInAsync(loginVM.Email, loginVM.Password,
                    loginVM.RememberMe, lockoutOnFailure: false);


            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (await _userManager.IsInRoleAsync(user, SD.RoleAdmin))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(loginVM.RedirectUrl);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        }

        return View(loginVM);
    }

    #endregion

    #region LOGOUT

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region ACCESSDENIED

    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    #region REGISTER

    public IActionResult Register(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (!_roleManager.RoleExistsAsync(SD.RoleAdmin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin)).Wait();
            _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer)).Wait();
        }

        RegisterVM registerVM = new()
        {
            RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }),
            RedirectUrl = returnUrl
        };

        return View(registerVM);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (ModelState.IsValid)
        {
            AppUser user = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                NormalizedEmail = registerVM.Email.ToUpper(),
                EmailConfirmed = true,
                UserName = registerVM.Email,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(registerVM.Role))
                {
                    await _userManager.AddToRoleAsync(user, registerVM.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.RoleCustomer);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(registerVM.RedirectUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Name
        });

        return View(registerVM);
    }

    #endregion
}