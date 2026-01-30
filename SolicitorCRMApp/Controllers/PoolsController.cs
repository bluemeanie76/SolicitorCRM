using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitorCRMApp.Data;
using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Controllers;

[Authorize(Policy = "SuperAdministratorOnly")]
public sealed class PoolsController : Controller
{
    private readonly IPoolRepository _poolRepository;
    private readonly IUserRepository _userRepository;

    public PoolsController(IPoolRepository poolRepository, IUserRepository userRepository)
    {
        _poolRepository = poolRepository;
        _userRepository = userRepository;
    }

    public async Task<IActionResult> Index()
    {
        var pools = await _poolRepository.GetAllAsync();
        return View(pools);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new PoolEditViewModel
        {
            Enabled = true,
            Users = (await _userRepository.GetAllAsync()).ToList()
        };
        return View("Edit", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var pool = (await _poolRepository.GetAllAsync()).FirstOrDefault(p => p.Id == id);
        if (pool is null)
        {
            return NotFound();
        }

        var model = new PoolEditViewModel
        {
            Id = pool.Id,
            Name = pool.Name,
            Description = pool.Description,
            Enabled = pool.Enabled,
            Users = (await _userRepository.GetAllAsync()).ToList(),
            AssignedUsers = (await _poolRepository.GetUsersAsync(pool.Id)).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PoolEditViewModel model)
    {
        model.Users = (await _userRepository.GetAllAsync()).ToList();
        model.AssignedUsers = model.Id.HasValue
            ? (await _poolRepository.GetUsersAsync(model.Id.Value)).ToList()
            : new List<User>();

        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }

        var pool = new Pool
        {
            Id = model.Id ?? 0,
            Name = model.Name,
            Description = model.Description,
            Enabled = model.Enabled
        };

        if (model.Id is null)
        {
            var poolId = await _poolRepository.CreateAsync(pool);
            return RedirectToAction(nameof(Edit), new { id = poolId });
        }

        await _poolRepository.UpdateAsync(pool);
        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, bool enable)
    {
        await _poolRepository.SetEnabledAsync(id, enable);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(PoolEditViewModel model)
    {
        if (model.Id is null || model.SelectedUserId is null)
        {
            return RedirectToAction(nameof(Index));
        }

        await _poolRepository.AssignUserAsync(model.Id.Value, model.SelectedUserId.Value);
        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUser(int id, int userId)
    {
        await _poolRepository.RemoveUserAsync(id, userId);
        return RedirectToAction(nameof(Edit), new { id });
    }
}
