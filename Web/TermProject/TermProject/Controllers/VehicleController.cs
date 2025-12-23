using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace TermProject.Controllers;

public class VehicleController : Controller
{
    private readonly string _conn;

    public VehicleController(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    // List page (mevcut)
    public IActionResult Index(string search, string make, string model)
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        return View();
    }

    [HttpGet]
    [Authorize] // ilan verme sayfasýný görmek için giriþ gerekebilir
    public IActionResult Create()
    {
        return View();
    }

    // POST: formu alýr, Marka/Seri/Model varsa kullanýr yoksa oluþturur, Arac ve Ilan ekler.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string Title,
        string Make,
        string Model,
        int? Year,
        int? Mileage,
        string? Fuel,
        string? Transmission,
        int? Engine,
        string? Condition,
        decimal Price,
        string? Description,
        string? ContactName,
        string? ContactPhone,
        string? Region)
    {
        // Kullanýcý id'si
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var kullaniciId))
        {
            TempData["ErrorMessage"] = "Giriþ yapmanýz gerekiyor.";
            return RedirectToAction("Login", "Account");
        }

        try
        {
            await using var conn = new MySqlConnection(_conn);
            await conn.OpenAsync();
            await using var tran = await conn.BeginTransactionAsync();

            // 1) Marka (varsa al, yoksa ekle)
            int markaId;
            {
                const string sel = "SELECT marka_id FROM Marka WHERE marka_adi = @adi LIMIT 1;";
                await using var selCmd = new MySqlCommand(sel, conn, tran);
                selCmd.Parameters.AddWithValue("@adi", Make ?? string.Empty);
                var obj = await selCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                {
                    markaId = Convert.ToInt32(obj);
                }
                else
                {
                    const string ins = "INSERT INTO Marka (marka_adi) VALUES (@adi); SELECT LAST_INSERT_ID();";
                    await using var insCmd = new MySqlCommand(ins, conn, tran);
                    insCmd.Parameters.AddWithValue("@adi", Make ?? string.Empty);
                    var idObj = await insCmd.ExecuteScalarAsync();
                    markaId = Convert.ToInt32(idObj);
                }
            }

            // 2) Seri (basit: seri_adi = Model text)
            int seriId;
            var seriAdi = Model ?? "Genel";
            {
                const string sel = "SELECT seri_id FROM Seri WHERE marka_id = @mid AND seri_adi = @sadi LIMIT 1;";
                await using var selCmd = new MySqlCommand(sel, conn, tran);
                selCmd.Parameters.AddWithValue("@mid", markaId);
                selCmd.Parameters.AddWithValue("@sadi", seriAdi);
                var obj = await selCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                {
                    seriId = Convert.ToInt32(obj);
                }
                else
                {
                    const string ins = "INSERT INTO Seri (marka_id, seri_adi) VALUES (@mid, @sadi); SELECT LAST_INSERT_ID();";
                    await using var insCmd = new MySqlCommand(ins, conn, tran);
                    insCmd.Parameters.AddWithValue("@mid", markaId);
                    insCmd.Parameters.AddWithValue("@sadi", seriAdi);
                    var idObj = await insCmd.ExecuteScalarAsync();
                    seriId = Convert.ToInt32(idObj);
                }
            }

            // 3) Model
            int modelId;
            var modelAdi = Model ?? "Unknown";
            {
                const string sel = "SELECT model_id FROM Model WHERE seri_id = @sid AND model_adi = @madi LIMIT 1;";
                await using var selCmd = new MySqlCommand(sel, conn, tran);
                selCmd.Parameters.AddWithValue("@sid", seriId);
                selCmd.Parameters.AddWithValue("@madi", modelAdi);
                var obj = await selCmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                {
                    modelId = Convert.ToInt32(obj);
                }
                else
                {
                    const string ins = "INSERT INTO Model (seri_id, model_adi) VALUES (@sid, @madi); SELECT LAST_INSERT_ID();";
                    await using var insCmd = new MySqlCommand(ins, conn, tran);
                    insCmd.Parameters.AddWithValue("@sid", seriId);
                    insCmd.Parameters.AddWithValue("@madi", modelAdi);
                    var idObj = await insCmd.ExecuteScalarAsync();
                    modelId = Convert.ToInt32(idObj);
                }
            }

            // 4) Arac ekle
            int aracId;
            var aracDurumu = (Condition ?? "").Equals("Yeni", StringComparison.OrdinalIgnoreCase) ? "Sifir" : "IkinciEl";
            const string insArac = @"
INSERT INTO Arac (model_id, yil, kilometre, vites_tipi, yakit_tipi, motor_hacmi, arac_durumu)
VALUES (@model_id, @yil, @km, @vites, @yakit, @motor, @durum);
SELECT LAST_INSERT_ID();";
            await using (var cmdArac = new MySqlCommand(insArac, conn, tran))
            {
                cmdArac.Parameters.AddWithValue("@model_id", modelId);
                cmdArac.Parameters.AddWithValue("@yil", (object?)Year ?? DBNull.Value);
                cmdArac.Parameters.AddWithValue("@km", (object?)Mileage ?? DBNull.Value);
                cmdArac.Parameters.AddWithValue("@vites", (object?)Transmission ?? DBNull.Value);
                cmdArac.Parameters.AddWithValue("@yakit", (object?)Fuel ?? DBNull.Value);
                cmdArac.Parameters.AddWithValue("@motor", (object?)Engine ?? DBNull.Value);
                cmdArac.Parameters.AddWithValue("@durum", aracDurumu);
                var aObj = await cmdArac.ExecuteScalarAsync();
                aracId = Convert.ToInt32(aObj);
            }

            // 5) Ilan ekle
            const string insIlan = @"
INSERT INTO Ilan (arac_id, kullanici_id, fiyat, ilan_tarihi, takasa_uygun, kimden)
VALUES (@arac_id, @kullanici_id, @fiyat, CURDATE(), @takas, @kimden);";
            await using (var cmdIlan = new MySqlCommand(insIlan, conn, tran))
            {
                cmdIlan.Parameters.AddWithValue("@arac_id", aracId);
                cmdIlan.Parameters.AddWithValue("@kullanici_id", kullaniciId);
                cmdIlan.Parameters.AddWithValue("@fiyat", Price);
                cmdIlan.Parameters.AddWithValue("@takas", false);
                cmdIlan.Parameters.AddWithValue("@kimden", "Sahibinden");
                await cmdIlan.ExecuteNonQueryAsync();
            }

            await tran.CommitAsync();

            TempData["SuccessMessage"] = "Ýlan baþarýyla oluþturuldu.";
            return RedirectToAction("Create");
        }
        catch (MySqlException mex)
        {
            // loglanabilir
            TempData["ErrorMessage"] = "Veritabaný hatasý: " + mex.Message;
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Hata: " + ex.Message;
            return RedirectToAction("Create");
        }
    }

    // Edit/Delete... (varsa) korunabilir
}