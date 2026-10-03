#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkAcademico.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LinkAcademico.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Required]
            [StringLength(100,
                ErrorMessage = "A senha deve ter pelo menos {2} caracteres.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar senha")]
            [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Selecione o tipo de usuário")]
            [Display(Name = "Tipo de usuário")]
            public string TipoUsuario { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();

            string[] tiposPermitidos =
            {
                "Estudante",
                "Empresa"
            };

            if (!tiposPermitidos.Contains(Input.TipoUsuario))
            {
                ModelState.AddModelError(
                    "Input.TipoUsuario",
                    "Selecione um tipo de usuário válido.");
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                // username
                await _userStore.SetUserNameAsync(
                    user,
                    Input.Email,
                    CancellationToken.None
                );

                // email
                await _emailStore.SetEmailAsync(
                    user,
                    Input.Email,
                    CancellationToken.None
                );

                // salva tipo
                user.TipoUsuario = Input.TipoUsuario;

                // cria usuário
                var result = await _userManager.CreateAsync(
                    user,
                    Input.Password
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário criado com sucesso.");

                    // 🔥 adiciona role automaticamente
                    await _userManager.AddToRoleAsync(
                        user,
                        Input.TipoUsuario
                    );

                    // 🔥 login automático
                    await _signInManager.SignInAsync(
                        user,
                        isPersistent: false
                    );

                    // 🔥 redirecionamento inteligente
                    if (user.TipoUsuario == "Estudante")
                    {
                        return Redirect("~/Experiencias");
                    }
                    else if (user.TipoUsuario == "Empresa")
                    {
                        return Redirect("~/Empresas/Create");
                    }
                    else if (user.TipoUsuario == "Administrador")
                    {
                        return Redirect("~/");
                    }

                    return Redirect("~/");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description
                    );
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Não foi possível criar '{nameof(ApplicationUser)}'."
                );
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "O sistema precisa de suporte a e-mail."
                );
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
