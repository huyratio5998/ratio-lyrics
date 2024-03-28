using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Areas.Admin.Models.User;
using Ratio_Lyrics.Web.Helpers;
using Ratio_Lyrics.Web.Models.Enums;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/userManagements")]
    //[Authorize(Roles = "Admin,Manager,SuperAdmin")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        private const int pageSizeClientDesktopDefault = 12;
        private const int pageSizeClientMobileDefault = 8;
        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterOrder(string actionRedirect, string? name, string? phoneNumber, string? email, int? page = 1)
        {
            var listFilterItems = new List<FacetFilterItem>();
            listFilterItems.Add(new FacetFilterItem() { FieldName = "IsClientUser", Type = FilterType.Bool.ToString(), Value = "true" });
            if (!string.IsNullOrWhiteSpace(name))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Name", Type = FilterType.Text.ToString(), Value = name });
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "PhoneNumber", Type = FilterType.Text.ToString(), Value = phoneNumber });
            if (!string.IsNullOrWhiteSpace(email))
                listFilterItems.Add(new FacetFilterItem() { FieldName = "Email", Type = FilterType.Text.ToString(), Value = email });

            if (page <= 1) page = null;

            return RedirectToAction(actionRedirect, new { filterItems = listFilterItems.FilterItemToJson(), page = page });
        }

        [HttpGet]
        public async Task<IActionResult> Index(BaseSearchArgs args)
        {
            var users = await _userService.Gets(args);

            if (users == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "UserManagements";
            ViewBag.Action = "Index";
            //ViewBag.DetailParam = "Index";

            return View(users);
        }

        [Route("employees")]
        [HttpGet]
        public async Task<IActionResult> Employees(BaseSearchArgs args)
        {
            var users = await _userService.Gets(args);
            if (users == null) return RedirectToAction("Index", "Home");

            ViewBag.Area = "Admin";
            ViewBag.Controller = "UserManagements";
            ViewBag.Action = "Employees";
            //ViewBag.DetailParam = "Employees";

            return View(users);
        }

        [Route("detail")]
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (id == Guid.Empty) return RedirectToAction("Index", "Home");

            var userDetail = await _userService.Get(id.ToString());

            if (userDetail == null) return RedirectToAction("Index", "Home");

            return View(userDetail);
        }

        [HttpGet("employeeDetail")]
        public async Task<IActionResult> EmployeeDetail(Guid id)
        {
            if (id == Guid.Empty) return RedirectToAction("Index", "Home");

            var employeeDetail = await _userService.Get(id.ToString());

            if (employeeDetail == null) return RedirectToAction("Index", "Home");

            return View(employeeDetail);
        }

        [HttpGet("createEmployee")]
        public async Task<IActionResult> CreateEmployee()
        {
            var availableRoles = await _userService.GetRoles();
            if (availableRoles == null || !availableRoles.Any()) return View();

            var result = new UserViewModel()
            {
                AvailableRoles = availableRoles.Select(x => x.Name ?? string.Empty).ToList(),
            };

            return View(result);
        }

        [HttpPost("createEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(UserViewModel employee)
        {
            employee.AvailableRoles = (await _userService.GetRoles()).Select(x => x.Name ?? string.Empty)?.ToList();
            if (employee == null || !ModelState.IsValid || string.IsNullOrWhiteSpace(employee.UserName) || string.IsNullOrWhiteSpace(employee.Password))
            {
                ViewBag.ErrorMessage = "Bad request!";
                return View(employee);
            }

            var result = await _userService.CreateEmployee(employee);

            if (result.Equals(Guid.Empty.ToString()))
            {
                ViewBag.ErrorMessage = "Fail to create employee!";
                return View(employee);
            }
            return RedirectToAction("Employees");
        }

        [HttpGet("updateEmployee")]
        public async Task<IActionResult> UpdateEmployee(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var employeeDetail = await _userService.Get(userId.ToString());
            if (employeeDetail == null) return RedirectToAction("Employees");

            employeeDetail.AvailableRoles = (await _userService.GetRoles()).Select(x => x.Name ?? string.Empty)?.ToList();

            return View(employeeDetail);
        }

        [HttpPost("updateEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(UserViewModel employee)
        {
            var availableRoles = (await _userService.GetRoles()).Select(x => x.Name ?? string.Empty)?.ToList();
            employee.AvailableRoles = availableRoles;
            if (employee == null || !ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Bad Request!";
                return View(employee);
            }

            var result = await _userService.UpdateEmployee(employee);

            if (!result)
            {
                ViewBag.ErrorMessage = "Fail to update employee info!";
                return View(employee);
            }

            return RedirectToAction("Employees");
        }

        [HttpGet("deleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var employeeDetail = await _userService.Get(userId.ToString());

            if (employeeDetail == null) return RedirectToAction("Employees");

            return View(employeeDetail);
        }

        [HttpPost("deleteEmployeeConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployeeConfirmed(Guid userId)
        {
            if (userId == Guid.Empty) return RedirectToAction("Employees");

            var deleteStatus = await _userService.Delete(userId.ToString());

            if (!deleteStatus)
            {
                var employeeDetail = await _userService.Get(userId.ToString());
                return View(employeeDetail);
            }

            return RedirectToAction("Employees");
        }
    }
}
