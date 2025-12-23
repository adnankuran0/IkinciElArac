using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace TermProject.Controllers;

public class AccountController : Controller
{
    private readonly string _conn;

    public AccountController(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "E-posta ve parola gereklidir.");
            return View();
        }

        const string sql = @"
SELECT k.kullanici_id, k.ad, k.soyad, k.email, k.sifre_hash, r.rol_adi
FROM Kullanicilar k
LEFT JOIN Rol r ON k.rol_id = r.rol_id
WHERE k.email = @email
LIMIT 1;";
        try
        {
            await using var conn = new MySqlConnection(_conn);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
            {
                ModelState.AddModelError("", "E-posta veya parola hatalý.");
                return View();
            }

            var id = rdr.GetInt32("kullanici_id");
            var ad = rdr.GetString("ad");
            var soyad = rdr.GetString("soyad");
            var dbEmail = rdr.GetString("email");
            var sifreHash = rdr.GetString("sifre_hash");
            var rolAdi = rdr.IsDBNull(rdr.GetOrdinal("rol_adi")) ? "User" : rdr.GetString("rol_adi");

            if (!PasswordHelper.Verify(password, sifreHash))
            {
                ModelState.AddModelError("", "E-posta veya parola hatalý.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, $"{ad} {soyad}"),
                new Claim(ClaimTypes.Email, dbEmail),
                new Claim(ClaimTypes.Role, rolAdi)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["SuccessMessage"] = "Giriþ baþarýlý.";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            ModelState.AddModelError("", "Giriþ sýrasýnda hata oluþtu. Tekrar deneyin.");
            return View();
        }
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string ad, string soyad, string email, string password, string? telefon)
    {
        if (string.IsNullOrWhiteSpace(ad) ||
            string.IsNullOrWhiteSpace(soyad) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Tüm alanlar zorunludur.");
            return View();
        }

        try
        {
            await using var conn = new MySqlConnection(_conn);
            await conn.OpenAsync();

            // email kontrolü
            await using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Kullanicilar WHERE email = @email;", conn))
            {
                checkCmd.Parameters.AddWithValue("@email", email);
                var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                if (exists > 0)
                {
                    ModelState.AddModelError("", "Bu e-posta zaten kayýtlý.");
                    return View();
                }
            }

            // rol id al / oluþtur
            int rolId;
            const string rolAdi = "User";
            await using (var selRol = new MySqlCommand("SELECT rol_id FROM Rol WHERE rol_adi = @rolAdi LIMIT 1;", conn))
            {
                selRol.Parameters.AddWithValue("@rolAdi", rolAdi);
                var obj = await selRol.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                {
                    rolId = Convert.ToInt32(obj);
                }
                else
                {
                    await using var insRol = new MySqlCommand("INSERT INTO Rol (rol_adi) VALUES (@rolAdi); SELECT LAST_INSERT_ID();", conn);
                    insRol.Parameters.AddWithValue("@rolAdi", rolAdi);
                    var idObj = await insRol.ExecuteScalarAsync();
                    rolId = Convert.ToInt32(idObj);
                }
            }

            var hash = PasswordHelper.Hash(password);

            // kullanýcý ekle ve id al
            await using (var insUser = new MySqlCommand(@"INSERT INTO Kullanicilar (ad, soyad, email, sifre_hash, telefon, rol_id)
                                                         VALUES (@ad, @soyad, @email, @sifre_hash, @telefon, @rol_id);
                                                         SELECT LAST_INSERT_ID();", conn))
            {
                insUser.Parameters.AddWithValue("@ad", ad);
                insUser.Parameters.AddWithValue("@soyad", soyad);
                insUser.Parameters.AddWithValue("@email", email);
                insUser.Parameters.AddWithValue("@sifre_hash", hash);
                insUser.Parameters.AddWithValue("@telefon", (object?)telefon ?? DBNull.Value);
                insUser.Parameters.AddWithValue("@rol_id", rolId);

                var newIdObj = await insUser.ExecuteScalarAsync();
                if (newIdObj == null)
                {
                    ModelState.AddModelError("", "Kayýt eklenemedi. Tekrar deneyin.");
                    return View();
                }

                var newId = Convert.ToInt32(newIdObj);

                // otomatik login
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newId.ToString()),
                    new Claim(ClaimTypes.Name, $"{ad} {soyad}"),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, rolAdi)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            TempData["SuccessMessage"] = "Kayýt baþarýlý. Hoþgeldiniz!";
            return RedirectToAction("Index", "Home");
        }
        catch (MySqlException mex)
        {
            ModelState.AddModelError("", "Veritabaný hatasý: " + mex.Message);
            return View();
        }
        catch
        {
            ModelState.AddModelError("", "Kayýt sýrasýnda hata oluþtu. Tekrar deneyin.");
            return View();
        }
    }

    // POST logout (güvenli)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Oturum kapatýldý.";
        return RedirectToAction("Index", "Home");
    }

    // GET /Account/Logout — sunum/test amaçlý, GET isteðine de cevap verir
    [HttpGet("Account/Logout")]
    public async Task<IActionResult> LogoutGet()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Oturum kapatýldý.";
        return RedirectToAction("Index", "Home");
    }

    // Basit PBKDF2 helper (local, güvenli)
    private static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var derive = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, Iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
            var key = derive.GetBytes(KeySize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string hashed)
        {
            try
            {
                var parts = hashed.Split('.', 3);
                if (parts.Length != 3) return false;
                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var key = Convert.FromBase64String(parts[2]);

                using var derive = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
                var keyToCheck = derive.GetBytes(key.Length);
                return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(key, keyToCheck);
            }
            catch
            {
                return false;
            }
        }
    }
}