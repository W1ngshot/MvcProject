using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Models;
using MvcProject.Models.UserModels;

namespace MvcProject.Controllers;

public class UserController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult GetAll() => View("UserList", _userManager.Users.ToList());

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var user = new User { Email = model.Email, UserName = model.Email};
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        var result = 
            await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }
        
        ModelState.AddModelError("", "Неправильный логин или пароль");
        return View(model);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.FindByEmailAsync(User.Identity!.Name);
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditViewModel {Email = user.Email};
        return View(model);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var user = await _userManager.FindByEmailAsync(User.Identity!.Name);
        if (user == null) 
            return View(model);
        
        user.Email = model.Email;
        user.UserName = model.Email;
                     
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var user = await _userManager.FindByEmailAsync(User.Identity!.Name);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
        }
        
        return RedirectToAction("Index");
    }
}