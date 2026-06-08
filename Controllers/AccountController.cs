using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HUTECH_Hospital.Data;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HUTECH_Hospital.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Tạm thời nếu đă đăng nhập thì chặn lại
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register (Dành riêng cho Patient)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    var hasPassword = await _userManager.HasPasswordAsync(existingUser);
                    if (!hasPassword) {
                        ModelState.AddModelError(string.Empty, "Email này đã đăng ký qua Google. Vui lòng đăng nhập Google.");
                    } else {
                        ModelState.AddModelError(string.Empty, "Email này đã được đăng ký. Vui lòng đăng nhập.");
                    }
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Luôn gán quyền Patient mặc định cho đăng ký công khai
                    await _userManager.AddToRoleAsync(user, "Patient");

                    // Tạo bản ghi Patient tương ứng
                    var patient = new Patient
                    {
                        ApplicationUserId = user.Id,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        StudentCode = model.StudentCode,
                        CreatedAt = DateTime.Now
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Lấy ra user để kiểm tra Role
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user!);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (roles.Contains("Doctor"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
                    }

                    // Mặc định hoặc Patient => Check HealthSurvey
                    if (roles.Contains("Patient"))
                    {
                        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
                        if (patient != null)
                        {
                            var hasSurvey = await _context.HealthSurveys.AnyAsync(s => s.PatientId == patient.Id);
                            if (!hasSurvey)
                            {
                                return RedirectToAction("Create", "HealthSurvey");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                    return View(model);
                }
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi từ nhà cung cấp bên ngoài: {remoteError}");
                return View("Login");
            }
            
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            // Đăng nhập luôn nếu Google account đã link
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // Nếu user chưa map
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var hasPassword = await _userManager.HasPasswordAsync(user);
                    if (hasPassword) 
                    {
                        // User đã có password -> Cấm dùng Google
                        ViewData["ReturnUrl"] = returnUrl;
                        ModelState.AddModelError(string.Empty, "Tài khoản đã được tạo bằng mật khẩu. Không thể dùng Google để đăng nhập.");
                        return View("Login");
                    }
                    
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // Lần đầu bằng Google => Auto Create Account
                    var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "Người dùng Google";

                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = fullName,
                        CreatedAt = DateTime.Now
                    };
                    var createResult = await _userManager.CreateAsync(newUser);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "Patient");
                        var patient = new Patient
                        {
                            ApplicationUserId = newUser.Id,
                            FullName = fullName,
                            CreatedAt = DateTime.Now
                        };
                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                        
                        await _userManager.AddLoginAsync(newUser, info);
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            ModelState.AddModelError(string.Empty, "Lỗi đăng nhập qua Google.");
            return View("Login");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
