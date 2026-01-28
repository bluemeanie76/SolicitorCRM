using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitorCRMApp.Data;
using SolicitorCRMApp.Models;
using SolicitorCRMApp.Services;

namespace SolicitorCRMApp.Controllers;

[Authorize(Policy = "AdministratorOnly")]
public sealed class AdminController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AdminController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new UserFormViewModel
        {
            Enabled = true,
            UserTypes = (await _userRepository.GetUserTypesAsync()).ToList()
        };
        return View("Edit", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var model = new UserFormViewModel
        {
            Id = user.Id,
            Title = user.Title,
            FirstName = user.FirstName,
            Surname = user.Surname,
            Email = user.Email,
            JobTitle = user.JobTitle,
            Department = user.Department,
            UserTypeId = user.UserTypeId,
            Enabled = user.Enabled,
            UserTypes = (await _userRepository.GetUserTypesAsync()).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(UserFormViewModel model)
    {
        model.UserTypes = (await _userRepository.GetUserTypesAsync()).ToList();

        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        if (model.Id is null)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required for new users.");
                return View("Edit", model);
            }

            var (hash, salt) = _passwordHasher.HashPassword(model.Password);
            await _userRepository.CreateAsync(model, hash, salt);
        }
        else
        {
            string? hash = null;
            string? salt = null;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                (hash, salt) = _passwordHasher.HashPassword(model.Password);
            }

            await _userRepository.UpdateAsync(model, hash, salt);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, bool enable)
    {
        await _userRepository.SetEnabledAsync(id, enable);
        return RedirectToAction(nameof(Index));
    }
}
