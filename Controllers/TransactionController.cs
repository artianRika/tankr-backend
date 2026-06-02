using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankR.Data.Dtos.Transactions;
using TankR.Data.Models;
using TankR.Repos.Interfaces;
using System.Text;
using System.Text.Json;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;

namespace TankR.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IStationRepo _stationRepo;
        private readonly IFuelTypeRepo _fuelTypeRepo;
        private readonly IStationEmployeeRepo _stationEmployeeRepo;
        private readonly IStationFuelPriceRepo _stationFuelPriceRepo;
        private readonly IStationPhotoRepo _stationPhotoRepo;
        private readonly IMapper _mapper;

        private readonly EmailSender _email;

        public TransactionController(
            ITransactionRepo transactionRepo,
            IUserRepo userRepo,
            IStationRepo stationRepo,
            IFuelTypeRepo fuelTypeRepo,
            IStationFuelPriceRepo stationFuelPriceRepo,
            IStationEmployeeRepo stationEmployeeRepo,
            IStationPhotoRepo stationPhotoRepo,
            IMapper mapper,
            EmailSender email)
        {
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
            _stationRepo = stationRepo;
            _fuelTypeRepo = fuelTypeRepo;
            _stationFuelPriceRepo = stationFuelPriceRepo;
            _stationEmployeeRepo = stationEmployeeRepo;
            _stationPhotoRepo = stationPhotoRepo;
            _mapper = mapper;
            _email = email;
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var transaction = await _transactionRepo.GetById(id);
                if (transaction == null) return NotFound();

                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityUserId)) return Unauthorized();

                // Admin can view any transaction
                if (User.IsInRole("Admin"))
                {
                    return Ok(_mapper.Map<TransactionDto>(transaction));
                }

                // Cashier can view transactions from their station
                if (User.IsInRole("Cashier"))
                {
                    var domainCashier = await _userRepo.GetByIdentityId(identityUserId);
                    if (domainCashier == null) return Forbid();
                    
                    var isStationEmployee = await _stationEmployeeRepo.Exists(transaction.StationId, domainCashier.Id);
                    if (isStationEmployee)
                    {
                        return Ok(_mapper.Map<TransactionDto>(transaction));
                    }
                    return Forbid();
                }

                // Customer can only view their own transactions
                if (User.IsInRole("Customer"))
                {
                    var domainUser = await _userRepo.GetById(transaction.CustomerId);
                    if (domainUser == null) return NotFound();
                    
                    if (string.Equals(domainUser.IdentityUserId, identityUserId, StringComparison.Ordinal))
                    {
                        return Ok(_mapper.Map<TransactionDto>(transaction));
                    }
                    return Forbid();
                }

                return Forbid();
            }
            catch (Exception e)
            {
                 return Problem(
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [Authorize(Roles = "Admin, Cashier")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityUserId)) return Unauthorized();

                var domainUser = await _userRepo.GetById(userId);
                if (domainUser == null) return NotFound("User not found");

                // Admin can view transactions for any user
                if (User.IsInRole("Admin"))
                {
                    var txAdmin = await _transactionRepo.GetByUser(userId);
                    return Ok(_mapper.Map<IEnumerable<TransactionDto>>(txAdmin));
                }

                // Cashier can view their own transactions only
                if (User.IsInRole("Cashier"))
                {
                    var domainCashier = await _userRepo.GetByIdentityId(identityUserId);
                    if (domainCashier == null) return Forbid();
                    
                    if (domainCashier.Id == userId)
                    {
                        var txCashier = await _transactionRepo.GetByUser(userId);
                        return Ok(_mapper.Map<IEnumerable<TransactionDto>>(txCashier));
                    }
                    return Forbid();
                }

                return Forbid();
            }
            catch (Exception e)
            {
                return Problem(
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetByStation(int stationId)
        {
            var station = await _stationRepo.GetById(stationId);
            if (station == null) return NotFound($"Station with id: {stationId} not found");
            
            var transactions = await _transactionRepo.GetByStation(stationId);
            return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
        }

        [Authorize(Roles = "Cashier,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionDto dto)
        {
            try
            {

                var user = await _userRepo.GetById(dto.CustomerId);
                if (user == null) return NotFound("User with id: " + dto.CustomerId + " not found");

                var station = await _stationRepo.GetById(dto.StationId);
                if (station == null) return NotFound("Station with id: " + dto.StationId + " not found");

                var identityCashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(identityCashierId)) return Unauthorized();
                
                var domainCashier = await _userRepo.GetByIdentityId(identityCashierId);
                if (domainCashier == null) return NotFound("User not found");
                
                // Cashier must be employed at the station
                if (User.IsInRole("Cashier"))
                {
                    var validCashier = await _stationEmployeeRepo.Exists(dto.StationId, domainCashier.Id);
                    if (!validCashier) return Forbid("This cashier is not employed at that station");
                }
                // Admin can create at any station, set cashier to current user
                else if (User.IsInRole("Admin"))
                {
                    // Admin creating transaction - use their ID as cashier
                    domainCashier = await _userRepo.GetByIdentityId(identityCashierId);
                }
                
                var fuelType = await _fuelTypeRepo.GetById(dto.FuelTypeId);
                if (fuelType == null) return NotFound("Fuel type with id: " + dto.FuelTypeId + " not found");


                var pricePerLiter = await _stationFuelPriceRepo.Get(dto.StationId, dto.FuelTypeId);
                if (pricePerLiter == null) return NotFound($"{station.Name} doesn't offer {fuelType.Name}");
                

                var transaction = _mapper.Map<Transaction>(dto);

                transaction.Liters = dto.Liters;
                transaction.PricePerLiter = pricePerLiter.Price;
                transaction.TotalPrice = transaction.PricePerLiter * transaction.Liters;

                transaction.PointsEarned = Convert.ToInt32(dto.Liters) * 2;

                user.LoyaltyPoints += transaction.PointsEarned;
                await _userRepo.Update(user);

                transaction.CashierId = domainCashier.Id;
                
                await _transactionRepo.Add(transaction);
                /*
                var html = $@"
                    <p>Hello {user.FirstName},</p>
                    <p>Thank you for choosing {transaction.Station.Name}!</p>


                    <h3>⛽ Transaction Details</h3>
                    <ul>
                        <li>🛢<b>Liters:</b>️ {transaction.Liters:N2} of {transaction.FuelType.Name}</li>
                        <li>⭐<b>Points earned:</b> {transaction.PointsEarned} </li>
                        <li>🎉<b>Total points:</b>{user.LoyaltyPoints}</li>
                        <li>💰<b>Total paid:</b>{transaction.TotalPrice:N2} MKD</li>
                    </ul>

                    <p>
                    Warm regards,<br/>
                    <b>The TankR Team 🚀</b>
                    </p>

                    <p style=""font-size: 12px; color: gray;"">
                    This is an automated message. Please do not reply directly to this email.
                    ";
                
                await _email.SendAsync(
                    "artianrika@gmail.com",
                    "Transaction Details",
                   html
                );*/
                
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id },
                    _mapper.Map<TransactionDto>(transaction));
            }
            catch (Exception e)
            {
                return Problem(
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("station/{stationId}/export")]
        public async Task<IActionResult> ExportStation(int stationId, string format = "csv")
        {
            try
            {
                var station = await _stationRepo.GetById(stationId);
                if (station == null) return NotFound($"Station with id: {stationId} not found");

                var transactions = (await _transactionRepo.GetByStation(stationId))?.ToList() ?? new List<Transaction>();

                format = (format ?? "csv").ToLowerInvariant();
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var filename = $"{station.Name}_transactions_{timestamp}.{format}";

                if (format == "json")
                {
                    var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
                    var bytes = Encoding.UTF8.GetBytes(json);
                    return File(bytes, "application/json", filename);
                }
                else if (format == "csv")
                {
                    // Pre-fetch customer data
                    var customerIds = transactions.Select(t => t.CustomerId).Distinct();
                    var customerMap = new Dictionary<int, User>();
                    foreach (var cid in customerIds)
                    {
                        var u = await _userRepo.GetById(cid);
                        if (u != null) customerMap[cid] = u;
                    }

                    var fuelTypeIds = transactions.Select(t => t.FuelTypeId).Distinct();
                    var fuelMap = new Dictionary<int, FuelType>();
                    foreach (var fid in fuelTypeIds)
                    {
                        var ft = await _fuelTypeRepo.GetById(fid);
                        if (ft != null) fuelMap[fid] = ft;
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("Id,Date,CustomerName,Phone,Address,FuelType,Liters,PricePerLiter,TotalPrice,PointsEarned");
                    foreach (var t in transactions)
                    {
                        customerMap.TryGetValue(t.CustomerId, out var customer);
                        fuelMap.TryGetValue(t.FuelTypeId, out var fuel);

                        var customerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : $"Customer {t.CustomerId}";
                        var phone = customer?.PhoneNumber ?? "-";
                        var address = customer?.Address != null 
                            ? $"{customer.Address.Street} {customer.Address.StreetNumber}, {customer.Address.City}, {customer.Address.Country}" 
                            : "-";
                        var fuelName = fuel?.Name ?? $"FuelType {t.FuelTypeId}";

                        // Escape quotes in fields
                        customerName = $"\"{customerName}\"";
                        phone = $"\"{phone}\"";
                        address = $"\"{address}\"";
                        fuelName = $"\"{fuelName}\"";

                        sb.AppendLine($"{t.Id},{t.CreatedAt:yyyy-MM-dd HH:mm:ss},{customerName},{phone},{address},{fuelName},{t.Liters:F2},{t.PricePerLiter:F3},{t.TotalPrice:F2},{t.PointsEarned}");
                    }
                    var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    return File(bytes, "text/csv", filename);
                }
                else if (format == "pdf")
                {
                    // Pre-fetch all related data
                    var customerIds = transactions.Select(t => t.CustomerId).Distinct();
                    var customerMap = new Dictionary<int, User>();
                    foreach (var cid in customerIds)
                    {
                        var u = await _userRepo.GetById(cid);
                        if (u != null) customerMap[cid] = u;
                    }

                    var fuelTypeIds = transactions.Select(t => t.FuelTypeId).Distinct();
                    var fuelMap = new Dictionary<int, FuelType>();
                    foreach (var fid in fuelTypeIds)
                    {
                        var ft = await _fuelTypeRepo.GetById(fid);
                        if (ft != null) fuelMap[fid] = ft;
                    }

                    // Try to download station photo
                    XImage? stationImage = null;
                    var photos = await _stationPhotoRepo.GetAllByStationId(stationId);
                    var firstPhoto = photos?.FirstOrDefault();
                    if (firstPhoto != null)
                    {
                        try
                        {
                            using var http = new System.Net.Http.HttpClient();
                            var imgBytes = await http.GetByteArrayAsync(firstPhoto.ImagePath);
                            stationImage = XImage.FromStream(() => new MemoryStream(imgBytes));
                        }
                        catch { /* skip image if unavailable */ }
                    }

                    // Try to download station logo
                    XImage? stationLogo = null;
                    if (!string.IsNullOrEmpty(station.LogoUrl))
                    {
                        try
                        {
                            using var http = new System.Net.Http.HttpClient();
                            var logoBytes = await http.GetByteArrayAsync(station.LogoUrl);
                            stationLogo = XImage.FromStream(() => new MemoryStream(logoBytes));
                        }
                        catch { /* skip logo if unavailable */ }
                    }

                    // PDF setup
                    var pdf = new PdfDocument();
                    var page = pdf.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    var gfx = XGraphics.FromPdfPage(page);

                    var fontBold = new XFont("Arial", 11, XFontStyle.Bold);
                    var fontSmall = new XFont("Arial", 8);
                    var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
                    var fontSubHeader = new XFont("Arial", 10);

                    const double margin = 30;
                    double pageWidth = page.Width;
                    double usable = pageWidth - margin * 2;
                    double y = margin;

                    // --- Station header with photo on left and logo on right ---
                    double imgSize = 60;
                    
                    // Draw logo on the right if available
                    if (stationLogo != null)
                    {
                        gfx.DrawImage(stationLogo, pageWidth - margin - imgSize, y, imgSize, imgSize);
                    }

                    if (stationImage != null)
                    {
                        gfx.DrawImage(stationImage, margin, y, imgSize, imgSize);
                        var textWidth = stationLogo != null ? usable - imgSize - 10 : usable - imgSize - 10;
                        gfx.DrawString(station.Name, fontHeader, XBrushes.Black,
                            new XRect(margin + imgSize + 10, y + 8, textWidth - imgSize, 20), XStringFormats.TopLeft);
                        if (station.Address != null)
                        {
                            var addr = $"{station.Address.Street} {station.Address.StreetNumber}, {station.Address.City} {station.Address.PostalCode}, {station.Address.Country}";
                            gfx.DrawString(addr, fontSubHeader, XBrushes.DarkGray,
                                new XRect(margin + imgSize + 10, y + 32, textWidth - imgSize, 16), XStringFormats.TopLeft);
                        }
                        y += imgSize + 20;
                    }
                    else
                    {
                        var textWidth = stationLogo != null ? usable - imgSize - 10 : usable;
                        gfx.DrawString(station.Name, fontHeader, XBrushes.Black,
                            new XRect(margin, y, textWidth, 20), XStringFormats.TopLeft);
                        y += 24;
                        if (station.Address != null)
                        {
                            var addr = $"{station.Address.Street} {station.Address.StreetNumber}, {station.Address.City} {station.Address.PostalCode}, {station.Address.Country}";
                            gfx.DrawString(addr, fontSubHeader, XBrushes.DarkGray,
                                new XRect(margin, y, textWidth, 16), XStringFormats.TopLeft);
                            y += 20;
                        }
                    }

                    // Divider - add more spacing
                    y += 8;
                    gfx.DrawLine(XPens.Gray, margin, y, margin + usable, y);
                    y += 10;

                    // Report title + summary
                    gfx.DrawString("Transaction Report", fontBold, XBrushes.Black,
                        new XRect(margin, y, usable, 16), XStringFormats.TopLeft);
                    gfx.DrawString($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC    Total transactions: {transactions.Count}    Total revenue: {transactions.Sum(t => t.TotalPrice):F2}",
                        fontSmall, XBrushes.DarkGray, new XRect(margin, y + 16, usable, 14), XStringFormats.TopLeft);
                    y += 36;

                    // --- Table ---
                    // Column definitions: (header, width)
                    var cols = new (string Header, double Width)[]
                    {
                        ("#",       22),
                        ("Date",    68),
                        ("Customer",85),
                        ("Phone",   65),
                        ("Address", 95),
                        ("Fuel",    50),
                        ("Liters",  38),
                        ("€/L",     36),
                        ("Total €", 42),
                        ("Points",  34),
                    };

                    double rowH = 16;
                    double headerH = 18;

                    void DrawTableHeader()
                    {
                        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(50, 50, 50)), margin, y, usable, headerH);
                        double cx = margin;
                        foreach (var col in cols)
                        {
                            gfx.DrawString(col.Header, fontBold, XBrushes.White,
                                new XRect(cx + 2, y + 2, col.Width - 4, headerH - 2), XStringFormats.TopLeft);
                            cx += col.Width;
                        }
                        y += headerH;
                    }

                    DrawTableHeader();

                    int rowNum = 0;
                    foreach (var t in transactions)
                    {
                        if (y + rowH > page.Height - margin)
                        {
                            page = pdf.AddPage();
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            y = margin;
                            DrawTableHeader();
                        }

                        // Alternating row background
                        if (rowNum % 2 == 1)
                            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 245, 245)), margin, y, usable, rowH);

                        customerMap.TryGetValue(t.CustomerId, out var customer);
                        fuelMap.TryGetValue(t.FuelTypeId, out var fuel);

                        var addrStr = "";
                        if (customer?.Address != null)
                            addrStr = $"{customer.Address.City}, {customer.Address.Country}";

                        var cellValues = new string[]
                        {
                            t.Id.ToString(),
                            t.CreatedAt.ToString("dd.MM.yy HH:mm"),
                            customer != null ? $"{customer.FirstName} {customer.LastName}" : t.CustomerId.ToString(),
                            customer?.PhoneNumber ?? "-",
                            addrStr,
                            fuel?.Name ?? t.FuelTypeId.ToString(),
                            t.Liters.ToString("F2"),
                            t.PricePerLiter.ToString("F3"),
                            t.TotalPrice.ToString("F2"),
                            t.PointsEarned.ToString(),
                        };

                        double cx = margin;
                        for (int i = 0; i < cols.Length; i++)
                        {
                            gfx.DrawString(cellValues[i], fontSmall, XBrushes.Black,
                                new XRect(cx + 2, y + 2, cols[i].Width - 4, rowH - 2), XStringFormats.TopLeft);
                            cx += cols[i].Width;
                        }

                        // Row bottom border
                        gfx.DrawLine(new XPen(XColor.FromArgb(220, 220, 220)), margin, y + rowH, margin + usable, y + rowH);

                        y += rowH;
                        rowNum++;
                    }

                    using var ms = new MemoryStream();
                    pdf.Save(ms);
                    var bytes = ms.ToArray();
                    return File(bytes, "application/pdf", filename);
                }
                else
                {
                    return BadRequest("Invalid format. Supported: csv, json, pdf");
                }
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class ArialFontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        private static string FontPath(string faceName)
        {
            if (OperatingSystem.IsWindows())
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                return faceName switch
                {
                    "Arial#b"  => Path.Combine(dir, "arialbd.ttf"),
                    "Arial#i"  => Path.Combine(dir, "ariali.ttf"),
                    "Arial#bi" => Path.Combine(dir, "arialbi.ttf"),
                    _          => Path.Combine(dir, "arial.ttf"),
                };
            }
            else
            {
                const string dir = "/usr/share/fonts/truetype/liberation";
                return faceName switch
                {
                    "Arial#b"  => Path.Combine(dir, "LiberationSans-Bold.ttf"),
                    "Arial#i"  => Path.Combine(dir, "LiberationSans-Italic.ttf"),
                    "Arial#bi" => Path.Combine(dir, "LiberationSans-BoldItalic.ttf"),
                    _          => Path.Combine(dir, "LiberationSans-Regular.ttf"),
                };
            }
        }

        public byte[] GetFont(string faceName) => System.IO.File.ReadAllBytes(FontPath(faceName));

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var suffix = (isBold, isItalic) switch
            {
                (true, true)  => "#bi",
                (true, false) => "#b",
                (false, true) => "#i",
                _             => ""
            };
            return new FontResolverInfo($"Arial{suffix}");
        }
    }
}
