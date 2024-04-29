using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using XFLCSMS.Models.Email;
using XFLCSMS.Models.Login;
using XFLCSMS.Models.Register;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace XFLCSMS.Controllers
{
    public class RegisterLoginController : Controller
    {
        private readonly DataContext _context;
        private readonly IEmailServices _emailServices;

        public RegisterLoginController(DataContext context, IEmailServices emailServices)
        {
            _context = context;
            _emailServices = emailServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {

            var viewModel = new RegisterViewModel
            {
                Brokerages = _context.Brokerages.ToList(),
                
            };
            return View(viewModel);



        }


        public IActionResult Verify()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerView)
        {

            if (!ModelState.IsValid)
            {
                registerView.Brokerages = _context.Brokerages.ToList();
                return View(registerView);
            }
            var request = registerView.userRegisterRequest;
            if (_context.Users.Any(u => (u.Email == request.Email || u.UserName == request.UserName)))
            {
                return BadRequest("User already exists.");

            }

            CreatePasswordHash(request.Password,
                 out byte[] passwordHash,
                 out byte[] passwordSalt);


            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken(),
                FullName = request.FullName,
                PhonNumber = request.PhonNumber,
                BrokerageHouseName = request.BrokerageHouseName,
                Branch = request.Branch,
                EmployeeId = request.EmployeeId,
                Terms = request.Terms,
                UserName = request.UserName,
                BrokerageHouseAcronym = request.BrokerageHouseName,
                UStatus = true,
                UCatagory = false,
                UType = false,

            };

            var sendMail = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            if (sendMail != null)
            {
                var Mail = new EmailDto
                {
                    To = request.Email,
                    Subject = "Registration Token for XFL Support System Software",
                    Body = "Dear Concern,\r\n\r\nThank you for choosing our Support System " +
                    "Software.\r\n\r\nTo complete your registration process, please use the following registration token:\r\n" +
                    "Registration Token:" + user.VerificationToken + "\r\n\r\n" +
                    "Please use this token to complete your registration process. If you did not request this token, " +
                    "please ignore this email.\r\n\r\n" +
                    "If you encounter any issues or need assistance, feel free to contact our support team at " +
                    "info@xpertfintech.com.\r\n\r\nThank you,\r\nXpert Fintech Limited"
                };
                _emailServices.SendEmail(Mail);
            }


            //return Ok("User successfully created!");
            return RedirectToAction("Verify");
        }



        [HttpPost]
        public async Task<IActionResult> Verify(Verify verify)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.VerificationToken == verify.Token) &&
            (u.Email == verify.Email || u.UserName == verify.Email));
            if (user == null)
            {
                ViewBag.Message = "Invalid token or Email";
                return View();
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            ViewBag.Message = "User verified";

            return RedirectToAction("Login");
        }




        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email == request.UserId || u.UserName == request.UserId));
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect.");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("Not verified!");
            }

            //var jsonString = JsonConvert.SerializeObject(user);
            //HttpContext.Session.SetString("MyObjectData", jsonString);

            if (user.UStatus == true)
            {
                if (user.UCatagory == true)
                {
                    var jsonString = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("AdminData", jsonString);
                    return RedirectToAction("Dashbord", "Admin");
                }
                else
                {
                    if (user.UType == true)
                    {
                        if (user.Department == "Support Maneger")
                        {
                            var jsonString = JsonConvert.SerializeObject(user);
                            HttpContext.Session.SetString("SMData", jsonString);
                            return RedirectToAction("Dashbord", "SupportManegar");
                        }
                        else if (user.Department == "Support Engineer")
                        {
                            var jsonString = JsonConvert.SerializeObject(user);
                            HttpContext.Session.SetString("SEData", jsonString);
                            return RedirectToAction("Dashbord", "SupportEngineer");
                        }
                        else
                        {
                            var jsonString = JsonConvert.SerializeObject(user);
                            HttpContext.Session.SetString("MakerData", jsonString);
                            return RedirectToAction("Dashbord", "Maker");
                        }
                    }
                    else
                    {
                        var jsonString = JsonConvert.SerializeObject(user);
                        HttpContext.Session.SetString("MakerData", jsonString);
                        return RedirectToAction("Dashbord", "Maker");
                    }
                }
            }
            else
            {
                return BadRequest("You are currenty Inactive Please contract XFL Team");
            }


        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Verify verify)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email == verify.Email)||(u.UserName==verify.Email));
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            user.PasswordResetToken = CreateRandomToken();
            string ResetToken = user.PasswordResetToken;
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            if (ResetToken != null)
            {
                var Mail = new EmailDto
                {
                    To = verify.Email,
                    Subject = "Password Reset Token",
                    Body = "Your Password Reset Token is: " + ResetToken
                };
                _emailServices.SendEmail(Mail);

            }

            return RedirectToAction("ResetPassword");
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ChangePassword()
        {

            return View();
        }

        //public async Task<IActionResult> GetBranches(int id)
        //{

        //    var branches = _context.Branchhs.Where(i=>i.BranchId == id).ToList();
        //    return Json(branches);

        //}



        [HttpGet]
        public async Task<IActionResult> GetBranches(int brokerageId)
        {
            var branches = await _context.Branchhs
                .Where(b => b.BrokerageId == brokerageId)
                 // Select only necessary fields
                .ToListAsync();

            return Json(branches);
        }





        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(3));
        }

        
    }
}
