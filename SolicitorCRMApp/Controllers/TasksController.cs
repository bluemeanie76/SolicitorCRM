using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitorCRMApp.Data;
using SolicitorCRMApp.Models;

namespace SolicitorCRMApp.Controllers;

[Authorize]
public sealed class TasksController : Controller
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPoolRepository _poolRepository;

    public TasksController(ITaskRepository taskRepository, IUserRepository userRepository, IPoolRepository poolRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _poolRepository = poolRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var canViewAllTasks = User.HasClaim("UserType", UserTypeNames.SuperAdministrator)
            || User.HasClaim("UserType", UserTypeNames.Administrator);
        var model = new TaskDashboardViewModel
        {
            CanViewAllTasks = canViewAllTasks,
            AllTasks = canViewAllTasks ? await _taskRepository.GetAllAsync() : Array.Empty<TaskItem>(),
            AssignedTasks = canViewAllTasks ? Array.Empty<TaskItem>() : await _taskRepository.GetAssignedToUserAsync(userId),
            PoolTasks = canViewAllTasks ? Array.Empty<TaskItem>() : await _taskRepository.GetAssignedToUserPoolsAsync(userId)
        };
        return View(model);
    }

    [Authorize(Policy = "AdministratorOnly")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new TaskFormViewModel
        {
            Users = (await _userRepository.GetAllAsync()).ToList(),
            Pools = (await _poolRepository.GetAllAsync()).ToList()
        };
        return View(model);
    }

    [Authorize(Policy = "AdministratorOnly")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskFormViewModel model)
    {
        model.Users = (await _userRepository.GetAllAsync()).ToList();
        model.Pools = (await _poolRepository.GetAllAsync()).ToList();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.AssignedUserId is null && model.AssignedPoolId is null)
        {
            ModelState.AddModelError(nameof(model.AssignedUserId), "Assign the task to a user or a pool.");
            return View(model);
        }

        var createModel = new TaskCreateModel
        {
            IsUrgent = model.IsUrgent,
            ContactName = model.ContactName,
            ContactEmail = model.ContactEmail,
            ContactTelephone = model.ContactTelephone,
            ContactNotes = model.ContactNotes,
            TaskDescription = model.TaskDescription,
            TaskDeadline = model.TaskDeadline,
            AssignedUserId = model.AssignedUserId,
            AssignedPoolId = model.AssignedPoolId,
            CreatedByUserId = GetUserId()
        };

        var taskId = await _taskRepository.CreateAsync(createModel);
        TempData["TaskCreatedMessage"] = $"Task #{taskId} has been created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null || !await CanAccessTaskAsync(task))
        {
            return Forbid();
        }

        var model = await BuildTaskDetailsViewModelAsync(task);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(TaskDetailsViewModel model)
    {
        if (model.Task.Id is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var task = await _taskRepository.GetByIdAsync(model.Task.Id.Value);
        if (task is null || !await CanAccessTaskAsync(task))
        {
            return Forbid();
        }

        if (model.Task.AssignedUserId is null && model.Task.AssignedPoolId is null)
        {
            ModelState.AddModelError(nameof(model.Task.AssignedUserId), "Assign the task to a user or a pool.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildTaskDetailsViewModelAsync(task);
            invalidModel.Task = model.Task;
            return View("Edit", invalidModel);
        }

        await _taskRepository.UpdateAsync(new TaskUpdateModel
        {
            Id = model.Task.Id.Value,
            IsUrgent = model.Task.IsUrgent,
            ContactName = model.Task.ContactName,
            ContactEmail = model.Task.ContactEmail,
            ContactTelephone = model.Task.ContactTelephone,
            ContactNotes = model.Task.ContactNotes,
            TaskDescription = model.Task.TaskDescription,
            TaskDeadline = model.Task.TaskDeadline,
            AssignedUserId = model.Task.AssignedUserId,
            AssignedPoolId = model.Task.AssignedPoolId
        });

        return RedirectToAction(nameof(Edit), new { id = model.Task.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNote(int id, string note)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null || !await CanAccessTaskAsync(task))
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(note))
        {
            await _taskRepository.AddNoteAsync(id, GetUserId(), note);
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogTime(int id, int hours, int minutes)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null || !await CanAccessTaskAsync(task))
        {
            return Forbid();
        }

        if (hours < 0 || minutes < 0 || minutes > 59)
        {
            return RedirectToAction(nameof(Edit), new { id });
        }

        await _taskRepository.LogTimeAsync(new TaskTimeEntryCreateModel
        {
            TaskId = id,
            LoggedByUserId = GetUserId(),
            Hours = hours,
            Minutes = minutes
        });

        return RedirectToAction(nameof(Edit), new { id });
    }

    private int GetUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idValue, out var userId) ? userId : 0;
    }

    private async Task<bool> CanAccessTaskAsync(TaskItem task)
    {
        if (User.HasClaim("UserType", UserTypeNames.SuperAdministrator)
            || User.HasClaim("UserType", UserTypeNames.Administrator))
        {
            return true;
        }

        var userId = GetUserId();
        if (task.AssignedUserId == userId)
        {
            return true;
        }

        var poolTasks = await _taskRepository.GetAssignedToUserPoolsAsync(userId);
        return poolTasks.Any(t => t.Id == task.Id);
    }

    private async Task<TaskDetailsViewModel> BuildTaskDetailsViewModelAsync(TaskItem task)
    {
        return new TaskDetailsViewModel
        {
            Task = new TaskFormViewModel
            {
                Id = task.Id,
                IsUrgent = task.IsUrgent,
                ContactName = task.ContactName,
                ContactEmail = task.ContactEmail,
                ContactTelephone = task.ContactTelephone,
                ContactNotes = task.ContactNotes,
                TaskDescription = task.TaskDescription,
                TaskDeadline = task.TaskDeadline,
                AssignedUserId = task.AssignedUserId,
                AssignedPoolId = task.AssignedPoolId,
                Users = (await _userRepository.GetAllAsync()).ToList(),
                Pools = (await _poolRepository.GetAllAsync()).ToList()
            },
            Notes = await _taskRepository.GetNotesAsync(task.Id),
            TimeEntries = await _taskRepository.GetTimeEntriesAsync(task.Id)
        };
    }
}
