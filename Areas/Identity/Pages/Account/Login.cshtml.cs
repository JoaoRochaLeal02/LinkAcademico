#nullable disable

using System.ComponentModel.DataAnnotations;

using LinkAcademico.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinkAcademico.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser>
            _signInManager;

        private readonly ILogger<LoginModel>
            _logger;

        private readonly UserManager<ApplicationUser>
            _userManager;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;

            _userManager = userManager;

            _logger = logger;
        }

        // =====================================================
        // INPUT
        // =====================================================

        [BindProperty]

        public InputModel Input { get; set; }

        public IList<AuthenticationScheme>
            ExternalLogins
        { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]

        public string ErrorMessage { get; set; }

        // =====================================================
        // INPUT MODEL
        // =====================================================

        public class InputModel
        {
            [Required]
            [EmailAddress]

            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]

            public string Password { get; set; }

            [Display(Name = "Lembrar login")]

            public bool RememberMe { get; set; }
        }

        // =====================================================
        // GET
        // =====================================================

        public async Task OnGetAsync(
            string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(
                    string.Empty,
                    ErrorMessage);
            }

            returnUrl ??=
                Url.Content("~/");

            await HttpContext.SignOutAsync(
                IdentityConstants.ExternalScheme);

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();

            ReturnUrl = returnUrl;
        }

        // =====================================================
        // POST
        // =====================================================

        public async Task<IActionResult>
            OnPostAsync(
                string returnUrl = null)
        {
            returnUrl ??=
                Url.Content("~/");

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                .ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result =
                await _signInManager
                    .PasswordSignInAsync(
                        Input.Email,
                        Input.Password,
                        Input.RememberMe,
                        lockoutOnFailure: false);

            // =================================================
            // LOGIN SUCESSO
            // =================================================

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Usuário realizou login.");

                var user =
                    await _userManager
                        .FindByEmailAsync(
                            Input.Email);

                if (user != null)
                {
                    // =====================================
                    // ESTUDANTE
                    // =====================================

                    if (await _userManager
                        .IsInRoleAsync(
                            user,
                            "Estudante"))
                    {
                        return LocalRedirect(
                            "~/");
                    }

                    // =====================================
                    // EMPRESA
                    // =====================================

                    if (await _userManager
                         .IsInRoleAsync(
                             user,
                         "Empresa"))
                    {
                        return LocalRedirect(
                            "~/");
                    }
                }

                // =========================================
                // PADRÃO
                // =========================================

                return LocalRedirect("~/");
            }

            // =================================================
            // 2FA
            // =================================================

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    "./LoginWith2fa",
                    new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe =
                            Input.RememberMe
                    });
            }

            // =================================================
            // LOCKOUT
            // =================================================

            if (result.IsLockedOut)
            {
                _logger.LogWarning(
                    "Usuário bloqueado.");

                return RedirectToPage(
                    "./Lockout");
            }

            // =================================================
            // LOGIN INVÁLIDO
            // =================================================

            ModelState.AddModelError(
                string.Empty,
                "Tentativa de login inválida.");

            return Page();
        }
    }
}

