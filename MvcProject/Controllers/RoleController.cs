using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Models;
using MvcProject.Models.RoleModels;
using MvcProject.Models.UserModels;

namespace MvcProject.Controllers;

public class RoleController: Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    
    public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public IActionResult Index() => View(_roleManager.Roles.ToList());

    [HttpGet]
    public IActionResult Add() => View();
    
    [HttpPost]
    public async Task<IActionResult> Add(string name)
    {
        if (string.IsNullOrEmpty(name)) 
            return View(name);
        
        var result = await _roleManager.CreateAsync(new IdentityRole(name));
        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(name);
    }

    [HttpGet]
    public IActionResult Give() => View();
    
    [HttpPost]
    public async Task<IActionResult> Give(GiveViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "user not found");
            return View(model);
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var identityRole = await _roleManager.FindByNameAsync(model.Role);

        if (identityRole == null)
        {
            ModelState.AddModelError(string.Empty, "role not found");
            return View(model);
        }
        
        if (!userRoles.Contains(identityRole.Name))
            await _userManager.AddToRoleAsync(user, identityRole.Name);
 
        return RedirectToAction("Index");
    }
}