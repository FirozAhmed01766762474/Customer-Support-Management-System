using MailKit.Search;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Cryptography;
using XFLCSMS.Models.Admin;
using XFLCSMS.Models.Affected;
using XFLCSMS.Models.Branch;
using XFLCSMS.Models.Brocarage;
using XFLCSMS.Models.Common;
using XFLCSMS.Models.Issue;
using XFLCSMS.Models.Register;
using XFLCSMS.Models.Support;
using XFLCSMS.Models.Todos;

namespace XFLCSMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashbord()
         {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                // Total Ticket
                int TotalInTicket = _context.Issues.Count();
                int TotalInclosed = _context.Issues.Count(i => i.IStatus == "Close");
                int TotalInQueue = TotalInTicket - TotalInclosed;
                //Today
                int TodayTotal = _context.Issues.Count(i => i.TDate.Date == DateTime.Now.Date);
                int TodayClose = _context.Issues.Count(i => (i.TDate.Date == DateTime.Now.Date) && (i.IStatus == "Close"));
                int TodayQueue = TodayTotal - TodayClose;
                //Week
                DateTime lastWeekStartDate = DateTime.Now.Date.AddDays(-7);
                int LastWeekTotal = _context.Issues.Count(i => i.TDate.Date >= lastWeekStartDate && i.TDate.Date <= DateTime.Now.Date);
                int LastWeekClosed = _context.Issues.Count(i => i.TDate.Date >= lastWeekStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close");
                int lastWeekQueue = LastWeekTotal - LastWeekClosed;

                // Last month's total issues
                DateTime lastMonthStartDate = DateTime.Now.Date.AddMonths(-1);
                int LastMonthTotal = _context.Issues.Count(i => i.TDate.Date >= lastMonthStartDate && i.TDate.Date <= DateTime.Now.Date);
                int LastMonthClosed = _context.Issues.Count(i => i.TDate.Date >= lastMonthStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close");
                int LastMonthQueue = LastMonthTotal - LastMonthClosed;

                // Last year's total issues
                DateTime lastYearStartDate = DateTime.Now.Date.AddYears(-1);
                int LastYearTotal = _context.Issues.Count(i => i.TDate.Date >= lastYearStartDate && i.TDate.Date <= DateTime.Now.Date);
                int LastYearClosed = _context.Issues.Count(i => i.TDate.Date >= lastYearStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close");
                int LastYearQueue = LastYearTotal - LastYearClosed;

                ///for Brocarage Chart
                ///
                var brokerages = _context.Brokerages.Include(b => b.Issues).ToList();

                var chartData = new
                {
                    labels = brokerages.Select(b => b.BrokerageHouseName),
                    datasets = new[]
                    {
                new
                {
                    label = "Total Tickets",
                    data = brokerages.Select(b => b.Issues.Count),
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    borderColor = "rgba(75, 192, 192, 1)",
                    borderWidth = 1
                },
                new
                {
                    label = "Close Tickets",
                    data = brokerages.Select(b => b.Issues.Count(I=>I.IStatus=="Close")),
                    backgroundColor = "rgba(0, 255, 0, 0.2)",
                    borderColor = "rgba(0, 255, 0, 1)",
                    borderWidth = 1
                },
                new
                {
                    label = "Inqueue Tickets",
                    data = brokerages.Select(b => b.Issues.Count(I=>I.IStatus!="Close")),
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    borderColor = "rgba(255, 206, 86, 1)",
                    borderWidth = 1
                },
                // Repeat the structure for "In Queue" and "Closed Tickets" using relevant data
            }
                };




                var TicketCount = new Dashboard
                {
                    TotalTicket = TotalInTicket,
                    TotalClosed = TotalInclosed,
                    TotalQueue = TotalInQueue,
                    TodayTotalTicket = TodayTotal,
                    TodayTotalClosed = TodayClose,
                    TodayTotalQueue = TodayQueue,
                    WeeklyTotalTicket = LastWeekTotal,
                    WeeklyTotalClosed = LastWeekClosed,
                    WeeklyTotalQueue = lastWeekQueue,
                    MonthlyTotalTicket = LastMonthTotal,
                    MonthlyTotalClosed = LastMonthClosed,
                    MonthlyTotalQueue = LastMonthQueue,
                    YearlyTotalTicket = LastYearTotal,
                    YearlyTotalClosed = LastYearClosed,
                    YearlyTotalQueue = LastYearQueue,
                    ChartData = chartData


                };







                return View(TicketCount);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public IActionResult AdminView(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                List<IssueTable> TicketList = _context.Issues.OrderByDescending(i => i.IssueId).ToList();

                if (!string.IsNullOrEmpty(searchString))
                {


                    TicketList = TicketList.Where(e =>
                                            e.TNumber.ToLower().Contains(searchString.ToLower()) ||
                                            e.ITitle?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.Priority.ToLower().Contains(searchString.ToLower()) || // Convert Priority to string for searching
                                            e.IStatus?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.AssignOn?.ToString().ToLower().Contains(searchString.ToLower()) == true ||
                                            e.TDate.ToString().ToLower().Contains(searchString.ToLower()) ||
                                            e.AssignBy?.ToLower().Contains(searchString.ToLower()) == true).ToList();

                }

                switch (sortField)
                {
                    case "TNumber":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.TNumber).ToList() :
                            TicketList.OrderByDescending(item => item.TNumber).ToList();
                        break;
                    case "Tickets":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.ITitle).ToList() :
                            TicketList.OrderByDescending(item => item.ITitle).ToList();
                        break;
                    case "Approval Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.AssignBy).ToList() :
                            TicketList.OrderByDescending(item => item.AssignBy).ToList();
                        break;
                    case "Priority":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.Priority).ToList() :
                            TicketList.OrderByDescending(item => item.Priority).ToList();
                        break;
                    case "Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.IStatus).ToList() :
                            TicketList.OrderByDescending(item => item.IStatus).ToList();
                        break;

                    // Add cases for other fields as needed
                    default:
                        // Default sorting if no valid sort field provided
                        TicketList = TicketList.OrderByDescending(item => item.IssueId).ToList();
                        break;
                }

                int tot_records = TicketList.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;


                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<IssueTable> IssueList = TicketList.Skip(skip_records).Take(take_records).ToList();
                return View(IssueList);
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login","RegisterLogin");
            }
        }

        public async Task<IActionResult> TicketView(int? id)
        {
            var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
            User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
            ViewBag.Profile = LogSesson;
            try
            {
                var issueWithAttachments = _context.Issues
                    .Include(i => i.attachment)  // Include attachments in the query
                    .FirstOrDefault(i => i.IssueId == id);

                if (issueWithAttachments == null)
                {
                    return NotFound();
                }

                var makerView = new MakerView
                {
                    IssueId = issueWithAttachments.IssueId,
                    CreatedOn = issueWithAttachments.TDate,
                    CreatedBy = UserName(issueWithAttachments.UserId),
                    AssgnOn = issueWithAttachments.AssignOn,
                    AssgnBy = issueWithAttachments.AssignBy,
                    SupportType = SupportTypeName(issueWithAttachments.SupportTypeId),
                    SupportCatagory = SupportCatagoryName(issueWithAttachments.SupportCatagoryId),
                    SupportSubCatagory = SupportSubCatagoryName(issueWithAttachments.SupportSubCatagoryId),
                    AffectedSection = AffectedSectionName(issueWithAttachments.AffectedSectionId),
                    TicketDetails = issueWithAttachments.Details,
                    Command = issueWithAttachments.Comments,
                    TicketStatus = issueWithAttachments.IStatus,
                    Priority = issueWithAttachments.Priority,
                    Attachments = issueWithAttachments.attachment,
                    ApproveOn = issueWithAttachments.ApproveOn,
                    ApproveBy = issueWithAttachments.ApproveBy,
                    IssueTitle = issueWithAttachments.ITitle


                };

                return View(makerView);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> EditTicket(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var issueWithAttachments = _context.Issues
                    .Include(i => i.attachment)  // Include attachments in the query
                    .FirstOrDefault(i => i.IssueId == id);
                //var SupportEng = _context.Users.Where(i => i.Designation == "Support Engineer").ToList();
                var SupportEng = _context.Users.Where(user => user.Department == "Support Engineer").ToList();

                if (issueWithAttachments == null)
                {
                    return NotFound();
                }


                var EditView = new MakerView
                {
                    IssueId = issueWithAttachments.IssueId,
                    CreatedOn = issueWithAttachments.TDate,
                    CreatedBy = UserName(issueWithAttachments.UserId),
                    AssgnOn = issueWithAttachments.AssignOn,
                    AssgnBy = issueWithAttachments.AssignBy,
                    SupportType = SupportTypeName(issueWithAttachments.SupportTypeId),
                    SupportCatagory = SupportCatagoryName(issueWithAttachments.SupportCatagoryId),
                    SupportSubCatagory = SupportSubCatagoryName(issueWithAttachments.SupportSubCatagoryId),
                    AffectedSection = AffectedSectionName(issueWithAttachments.AffectedSectionId),
                    TicketDetails = issueWithAttachments.Details,
                    Command = issueWithAttachments.Comments,
                    TicketStatus = issueWithAttachments.IStatus,
                    Attachments = issueWithAttachments.attachment,
                    IssueTitle = issueWithAttachments.ITitle,
                    Priority = issueWithAttachments.Priority,
                    SupportEngineers = SupportEng,
                    ApproveOn = issueWithAttachments.ApproveOn,
                    ApproveBy = issueWithAttachments.ApproveBy,

                };
                return View(EditView);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicketttt(MakerView makerView, List<IFormFile> files)
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var issue = await _context.Issues.FirstOrDefaultAsync(a => a.IssueId == makerView.IssueId);
                if (issue != null)
                {
                    issue.ITitle = makerView.IssueTitle;
                    issue.Details = makerView.TicketDetails;
                    issue.Comments = makerView.Command;
                    issue.AssignOn = makerView.AssgnOn;
                    issue.AssignBy = makerView.AssgnBy;
                    issue.UpdatedOn = DateTime.Now;
                    issue.UpdatedBy = LogSesson.FullName.ToString();
                    if (makerView.AssgnBy == null)
                    {
                        issue.AssignOn = null;
                        issue.AssignBy = null;
                    }
                    if (issue.AssignBy != null && LogSesson.UType == true)
                    {
                        issue.ApproveOn = DateTime.Now;
                        issue.ApproveBy = LogSesson.FullName;
                    }
                    else
                    {
                        issue.ApproveOn = null;
                        issue.ApproveBy = null;
                    }
                    issue.IStatus = makerView.IStatus;
                    if (makerView.IStatus == "Close")
                    {
                        issue.ClosedOn = DateTime.Now;
                        issue.ClosedBy = LogSesson.FullName;
                    }
                    else
                    {
                        issue.ClosedOn = null;
                        issue.ClosedBy = null;
                    }

                    _context.Update(issue);
                    await _context.SaveChangesAsync();


                    if (files != null)
                    {
                        var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uplods");
                        Directory.CreateDirectory(uploadFolder); // Create the directory once outside the loop

                        foreach (var file in files)
                        {
                            var fileName = file.FileName; // Ensure unique file names
                            var filePath = Path.Combine(uploadFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var attachment = new Attachment
                            {
                                FileName = file.FileName,
                                AttachmentLoc = filePath,
                                IssueId = makerView.IssueId
                            };

                            _context.Attachments.Add(attachment);
                        }

                        await _context.SaveChangesAsync();
                    }


                    return RedirectToAction("AdminView", "Admin");
                }


                return NotFound("Edit is not done");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> ClosedTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                List<IssueTable> TicketList = _context.Issues.OrderByDescending(i => i.IssueId).Where(i => i.IStatus == "Close").ToList();

                if (!string.IsNullOrEmpty(searchString))
                {


                    TicketList = TicketList.Where(e =>
                                            e.TNumber.ToLower().Contains(searchString.ToLower()) ||
                                            e.ITitle?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.Priority.ToLower().Contains(searchString.ToLower()) || // Convert Priority to string for searching
                                            e.IStatus?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.AssignOn?.ToString().ToLower().Contains(searchString.ToLower()) == true ||
                                            e.TDate.ToString().ToLower().Contains(searchString.ToLower()) ||
                                            e.AssignBy?.ToLower().Contains(searchString.ToLower()) == true).ToList();

                }

                switch (sortField)
                {
                    case "TNumber":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.TNumber).ToList() :
                            TicketList.OrderByDescending(item => item.TNumber).ToList();
                        break;
                    case "Tickets":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.ITitle).ToList() :
                            TicketList.OrderByDescending(item => item.ITitle).ToList();
                        break;
                    case "Approval Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.AssignBy).ToList() :
                            TicketList.OrderByDescending(item => item.AssignBy).ToList();
                        break;
                    case "Priority":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.Priority).ToList() :
                            TicketList.OrderByDescending(item => item.Priority).ToList();
                        break;
                    case "Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.IStatus).ToList() :
                            TicketList.OrderByDescending(item => item.IStatus).ToList();
                        break;

                    // Add cases for other fields as needed
                    default:
                        // Default sorting if no valid sort field provided
                        TicketList = TicketList.OrderByDescending(item => item.IssueId).ToList();
                        break;
                }

                int tot_records = TicketList.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;


                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<IssueTable> IssueList = TicketList.Skip(skip_records).Take(take_records).ToList();
                return View(IssueList);
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        public async Task<IActionResult> UnassignedTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                List<IssueTable> TicketList = _context.Issues.OrderByDescending(i => i.IssueId).Where(i => i.AssignOn == null && i.AssignBy == null).ToList();
                if (!string.IsNullOrEmpty(searchString))
                {


                    TicketList = TicketList.Where(e =>
                                            e.TNumber.ToLower().Contains(searchString.ToLower()) ||
                                            e.ITitle?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.Priority.ToLower().Contains(searchString.ToLower()) || // Convert Priority to string for searching
                                            e.IStatus?.ToLower().Contains(searchString.ToLower()) == true ||
                                            e.AssignOn?.ToString().ToLower().Contains(searchString.ToLower()) == true ||
                                            e.TDate.ToString().ToLower().Contains(searchString.ToLower()) ||
                                            e.AssignBy?.ToLower().Contains(searchString.ToLower()) == true).ToList();

                }

                switch (sortField)
                {
                    case "TNumber":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.TNumber).ToList() :
                            TicketList.OrderByDescending(item => item.TNumber).ToList();
                        break;
                    case "Tickets":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.ITitle).ToList() :
                            TicketList.OrderByDescending(item => item.ITitle).ToList();
                        break;
                    case "Approval Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.AssignBy).ToList() :
                            TicketList.OrderByDescending(item => item.AssignBy).ToList();
                        break;
                    case "Priority":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.Priority).ToList() :
                            TicketList.OrderByDescending(item => item.Priority).ToList();
                        break;
                    case "Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.IStatus).ToList() :
                            TicketList.OrderByDescending(item => item.IStatus).ToList();
                        break;

                    // Add cases for other fields as needed
                    default:
                        // Default sorting if no valid sort field provided
                        TicketList = TicketList.OrderByDescending(item => item.IssueId).ToList();
                        break;
                }

                int tot_records = TicketList.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;


                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<IssueTable> IssueList = TicketList.Skip(skip_records).Take(take_records).ToList();
                return View(IssueList);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        public IActionResult CreateBrocarage(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                
                return View();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrocarage([Bind("BrokerageId,BrokerageHouseName,BrokerageHouseAcronym")] Brokerage brokerage)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                await _context.AddAsync(brokerage);
                await _context.SaveChangesAsync();
                return RedirectToAction("BrocarageHouseList", "Admin"); // Redirect to the action that displays a list of brokerages     
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        [HttpDelete]
        public IActionResult DeleteTicket(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var issueToDelete = _context.Issues.Find(id);
                if (issueToDelete != null)
                {
                    _context.Issues.Remove(issueToDelete);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> UserList()
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var users = _context.Users.ToList();
                return View(users);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> UserView(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);

                if (user == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                var userView = new UserView
                {
                    Id = user.Id,
                    FullName = user.UserName,
                    Email = user.Email,
                    PhonNumber = user.PhonNumber,
                    Designation = user.Designation,
                    BrokerageHouseName = GetBrocarageHouseName(user.BrokerageHouseName),
                    Branch = GetBranchName(user.Branch),
                    EmployeeId = user.EmployeeId,
                    UserName = user.UserName,
                    UCatagory = user.UCatagory,
                    UType = user.UType,
                    UStatus = user.UStatus
                };


                return View(userView);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);

                if (user == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                var userView = new UserView
                {
                    Id = user.Id,
                    FullName = user.UserName,
                    Email = user.Email,
                    PhonNumber = user.PhonNumber,
                    Designation = user.Designation,
                    BrokerageHouseName = GetBrocarageHouseName(user.BrokerageHouseName),
                    Branch = GetBranchName(user.Branch),
                    EmployeeId = user.EmployeeId,
                    UserName = user.UserName,
                    UCatagory = user.UCatagory,
                    UType = user.UType,
                    UStatus = user.UStatus
                };
                return View(userView);
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


            
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser( UserView userView)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userView.Id);

                if (user != null)
                {
                    user.UType = userView.UType;
                    user.UStatus = userView.UStatus;
                    user.UCatagory = userView.UCatagory;
                    user.Department = userView.Department;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("UserList");
                }
                return RedirectToAction("UserList");
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> BrocarageHouseList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var Brocareges = await _context.Brokerages.ToListAsync();
                return View(Brocareges);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        public async Task<IActionResult> ViewBrocarageHouse(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var brocarage = await _context.Brokerages.FirstOrDefaultAsync(item => item.BrokerageId == id);

                if (brocarage == null)
                {
                    return NotFound();
                }

                return View(brocarage);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> DownloadAttachment(int? att)
        {
            if (att == null)
            {
                return BadRequest("Attachment ID is missing in the query parameters.");
            }

            var attachment = await _context.Attachments.FirstOrDefaultAsync(i => i.AttachmentId == att);

            if (attachment == null)
            {
                return NotFound("Attachment not found.");
            }

            var filePath = attachment.AttachmentLoc;
            var fileName = attachment.FileName;

            if (System.IO.File.Exists(filePath))
            {
                return PhysicalFile(filePath, "application/pdf", fileName);
            }
            else
            {
                return NotFound("File not found."); // or handle as appropriate
            }
        }

        public async Task<IActionResult> EditBrocarage(int id)
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var brocarage = await _context.Brokerages.FirstOrDefaultAsync(item => item.BrokerageId == id);

                if (brocarage == null)
                {
                    return NotFound();
                }

                return View(brocarage);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateBrocarage(Brokerage brokeragesss)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);

                var brocarage = await _context.Brokerages.FirstOrDefaultAsync(item => item.BrokerageId == brokeragesss.BrokerageId);

                if (brocarage != null)
                {
                    brocarage.BrokerageHouseName = brokeragesss.BrokerageHouseName;
                    brocarage.BrokerageHouseAcronym = brokeragesss.BrokerageHouseAcronym;
                    _context.Update(brocarage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("BrocarageHouseList", "Admin");
                }
                return NotFound();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin"); 
            }

        }

        [HttpDelete]
        public IActionResult DeleteBrocarage(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var issueToDelete = _context.Brokerages.Find(id);
                if (issueToDelete != null)
                {
                    _context.Brokerages.Remove(issueToDelete);
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }


        public async Task<IActionResult> BranchList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var Branch = await _context.Branchhs.ToListAsync();
                List<BranchView> branches = new List<BranchView>();
                foreach (var branch in Branch)
                {
                    BranchView b = new BranchView();
                    b.BranchId = branch.BranchId;
                    b.BranchName = branch.BranchName;
                    b.BrokerageHouseName = GetBrocarageHouseName(branch.BrokerageId);
                    branches.Add(b);
                }

                return View(branches);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public IActionResult CreateBranch()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var Brocarage = _context.Brokerages.ToList();

                BranchView BB = new BranchView();
                BB.brocarage = Brocarage;

                return View(BB);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(BranchView branchView)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                Branchh branchh = new Branchh();
                branchh.BranchName = branchView.BranchName;
                branchh.BrokerageId = branchView.BrokerageId;
                await _context.Branchhs.AddAsync(branchh);
                await _context.SaveChangesAsync();
                return RedirectToAction("BranchList", "Admin");
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> ViewBranch(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var branch = await _context.Branchhs.FirstOrDefaultAsync(item => item.BrokerageId == id);

                if (branch == null)
                {
                    return NotFound();
                }

                BranchView b = new BranchView();

                b.BranchId = branch.BranchId;
                b.BranchName = branch.BranchName;
                b.BrokerageHouseName = GetBrocarageHouseName(branch.BrokerageId);

                return View(b);
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> EditBranch(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var branch = await _context.Branchhs.FirstOrDefaultAsync(item => item.BranchId == id);
                var Brocarage = await _context.Brokerages.ToListAsync();

                if (branch == null)
                {
                    return NotFound();
                }

                BranchView b = new BranchView();

                b.BranchId = branch.BranchId;
                b.BranchName = branch.BranchName;
                b.BrokerageHouseName = GetBrocarageHouseName(branch.BrokerageId);
                b.brocarage = Brocarage;

                return View(b);
            }
            catch(Exception ex)
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }
        [HttpPost]
        public async Task<IActionResult> UpdateBranch(BranchView branchView)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var branch = await _context.Branchhs.FirstOrDefaultAsync(item => item.BranchId == branchView.BranchId);

                if (branch != null)
                {
                    branch.BranchName = branchView.BranchName;

                    _context.Branchhs.Update(branch);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("BranchList", "Admin");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpDelete]
        public IActionResult DeleteBranch(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var issueToDelete = _context.Branchhs.FirstOrDefault(i=>i.BranchId== id);
                if (issueToDelete != null)
                {
                    _context.Branchhs.Remove(issueToDelete);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> SupportTypeList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportType = await _context.SupportTypes.ToListAsync();
                return View(supportType);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> ViewSupportType(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var brocarage = await _context.SupportTypes.FirstOrDefaultAsync(item => item.SupportTypeId == id);

                if (brocarage == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(brocarage);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public IActionResult CreateSupportType()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                return View();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupportType([Bind("SupportTypeId,SType")] SupportType supportType)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);

                await _context.AddAsync(supportType);
                await _context.SaveChangesAsync();
                return RedirectToAction("SupportTypeList", "Admin"); // Redirect to the action that displays a list of brokerages
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> EditSupportType(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var brocarage = await _context.SupportTypes.FirstOrDefaultAsync(item => item.SupportTypeId == id);

                if (brocarage == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(brocarage);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateSupportType(SupportType brokeragesss)
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var brocarage = await _context.SupportTypes.FirstOrDefaultAsync(item => item.SupportTypeId == brokeragesss.SupportTypeId);

                if (brocarage != null)
                {
                    brocarage.SType = brokeragesss.SType;

                    _context.Update(brocarage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SupportTypeList", "Admin");
                }
                return NotFound();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpDelete]
        public IActionResult DeleteSupportType(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var issueToDelete = _context.SupportTypes.Find(id);
                if (issueToDelete != null)
                {
                    _context.SupportTypes.Remove(issueToDelete);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> SupportCatagoryList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportCatagory = await _context.SupportCatagories.ToListAsync();
                return View(supportCatagory);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> ViewSupportCatagory(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportCatagory = await _context.SupportCatagories.FirstOrDefaultAsync(item => item.SupportCatagoryId == id);

                if (supportCatagory == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(supportCatagory);
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public IActionResult CreateSupportCatagory()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                return View();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupportCatagory([Bind("SupportCatagoryId,SCatagory")] SupportCatagory supportCatagory)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                await _context.AddAsync(supportCatagory);
                await _context.SaveChangesAsync();
                return RedirectToAction("SupportCatagoryList", "Admin"); // Redirect to the action that displays a list of brokerages
            }
            catch (Exception ex) 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> EditSupportCatagory(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportCatagory = await _context.SupportCatagories.FirstOrDefaultAsync(item => item.SupportCatagoryId == id);

                if (supportCatagory == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(supportCatagory);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateSupportCatagory(SupportCatagory supportCatagory)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var scatagory = await _context.SupportCatagories.FirstOrDefaultAsync(item =>
                item.SupportCatagoryId == supportCatagory.SupportCatagoryId);

                if (scatagory != null)
                {
                    scatagory.SCatagory = supportCatagory.SCatagory;

                    _context.Update(scatagory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SupportCatagoryList", "Admin");
                }
                return NotFound();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpDelete]
        public IActionResult DeleteSupportCatagory(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var scatagory = _context.SupportCatagories.Find(id);
                if (scatagory != null)
                {
                    _context.SupportCatagories.Remove(scatagory);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> SupportSubCatagoryList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportSubCatagory = await _context.SupportSubCatagories.ToListAsync();
                return View(supportSubCatagory);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> ViewSupportSubCatagory(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportSubCatagory = await _context.SupportSubCatagories.FirstOrDefaultAsync(item => item.SupportSubCatagoryId == id);

                if (supportSubCatagory == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(supportSubCatagory);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public IActionResult CreateSupportSubCatagory()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                return View();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupportSubCatagory([Bind("SupportSubCatagoryId,SubCatagory")] SupportSubCatagory supportSubCatagory)
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                await _context.AddAsync(supportSubCatagory);
                await _context.SaveChangesAsync();
                return RedirectToAction("SupportSubCatagoryList", "Admin"); // Redirect to the action that displays a list of brokerages
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> EditSupportSubCatagory(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var supportSubCatagory = await _context.SupportSubCatagories.FirstOrDefaultAsync(item => item.SupportSubCatagoryId == id);

                if (supportSubCatagory == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(supportSubCatagory);
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateSupportSubCatagory(SupportSubCatagory supportSubCatagory)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var subcatagory = await _context.SupportSubCatagories.FirstOrDefaultAsync(item => item.SupportSubCatagoryId == supportSubCatagory.SupportSubCatagoryId);

                if (subcatagory != null)
                {
                    subcatagory.SubCatagory = supportSubCatagory.SubCatagory;

                    _context.Update(subcatagory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SupportSubCatagoryList", "Admin");
                }
                return NotFound();
            }
            catch(Exception ex)
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpDelete]
        public IActionResult DeleteSupportSubCatagory(int id)
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var subcatagory = _context.SupportSubCatagories.Find(id);
                if (subcatagory != null)
                {
                    _context.SupportSubCatagories.Remove(subcatagory);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch 
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }



        public async Task<IActionResult> AffectedSectionList()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var affectedSectios = await _context.AffectedSectionss.ToListAsync();
                return View(affectedSectios);
            }
            catch {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> ViewAffectedSection(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var affectedsection = await _context.AffectedSectionss.FirstOrDefaultAsync(item => item.AffectedSectionId == id);

                if (affectedsection == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(affectedsection);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public IActionResult CreateAffectedSection()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                return View();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAffectedSection([Bind("AffectedSectionId,ASection")] AffectedSection affectedSection)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                await _context.AddAsync(affectedSection);
                await _context.SaveChangesAsync();
                return RedirectToAction("AffectedSectionList", "Admin"); // Redirect to the action that displays a list of brokerages
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        public IActionResult DeleteAttachment(int attachmentId)
        {
            var attachment = _context.Attachments.Find(attachmentId);

            if (attachment == null)
            {
                return NotFound();
            }

            _context.Attachments.Remove(attachment);
            _context.SaveChanges();

            return Ok();
        }

        public async Task<IActionResult> EditAffectedSection(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var affectedSection = await _context.AffectedSectionss.FirstOrDefaultAsync(item => item.AffectedSectionId == id);

                if (affectedSection == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                return View(affectedSection);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateAffectedSection(AffectedSection affectedSection)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var aSection = await _context.AffectedSectionss.FirstOrDefaultAsync(item => item.AffectedSectionId == affectedSection.AffectedSectionId);

                if (aSection != null)
                {
                    aSection.ASection = affectedSection.ASection;

                    _context.Update(aSection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AffectedSectionList", "Admin");
                }
                return NotFound();
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpDelete]
        public IActionResult DeleteAffectedSection(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                // Perform the deletion logic here (remove the issue from the database)
                var asection = _context.AffectedSectionss.Find(id);
                if (asection != null)
                {
                    _context.AffectedSectionss.Remove(asection);
                    _context.SaveChanges();
                    return Ok(); // Return a success status
                }
                return NotFound(); // Return a not found status if the issue doesn't exist
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
            User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
            ViewBag.Profile = LogSesson;

            var supportEngineer = _context.Users.Where(i => i.Department == "Support Engineer").Select(user=>user.FullName).ToList();
            var BrocarageHouse = _context.Brokerages.ToList();
     

            var reportview = new ReportView
            {
                EmployeeNames = supportEngineer,
                brocarages = BrocarageHouse,
                

            };

            return View(reportview);
        }


        [HttpGet]
        public async Task<IActionResult> Search(ReportView reportView)
        {


            if(reportView.search.ToDate>=DateTime.Now)
            {
                reportView.search.ToDate=DateTime.Now;
            }

            var searchResults = await _context.Issues.ToListAsync();

            if (reportView.search.BrokerageId.HasValue)
                searchResults = searchResults.Where(x => x.BrokerageId == reportView.search.BrokerageId).ToList();
            if (!String.IsNullOrEmpty(reportView.search.Priority))
                searchResults = searchResults.Where(x => x.Priority.Contains(reportView.search.Priority)).ToList();
            if (!String.IsNullOrEmpty(reportView.search.AStatus))
                searchResults = searchResults.Where(x => x.IStatus.Contains(reportView.search.AStatus)).ToList();
            if (reportView.search.EmployeeName != null)
                searchResults = searchResults.Where(x => x.ClosedBy == reportView.search.EmployeeName).ToList();
            if((reportView.search.FromDate != null) && (reportView.search.ToDate != null))
                searchResults = searchResults.Where(x => (x.TDate >= reportView.search.FromDate) && (x.TDate <= reportView.search.ToDate)).ToList();

            string Brocaragename = (reportView.search.BrokerageId > 0) ? GetBrocarageHouseName(reportView.search.BrokerageId) : "All";
            string EmployeeNamee = (reportView.search.EmployeeName !=null) ? reportView.search.EmployeeName : "All";



            int TotalTickett = searchResults.Count();
            int TotalOpenTickett = searchResults.Where(x => x.IStatus == "Open").Count();
            int TotalCloseTickett = searchResults.Where(x => x.IStatus == "Close").Count();
            int TotalInquee = TotalTickett - TotalOpenTickett - TotalCloseTickett;


            HeaderInfo SectionInfo = new HeaderInfo
            {
                BrokerageHouseName = Brocaragename,
                EmployeeName = EmployeeNamee,
                TotalTicket = TotalTickett,
                TotalOpenTicket = TotalOpenTickett,
                TotalCloseTicket = TotalCloseTickett,
                TotalInque = TotalInquee,
                ReportName = "Admin"


            };

            // Populate the Issues property of the ReportView model with search results
            var reportViewWithSearchResults = new ReportView
            {
                Issues = searchResults,
                HeaderInfo = SectionInfo,

            };

            // Return the partial view with the populated reportView model
            return PartialView("_SearchResults", reportViewWithSearchResults);
        }


        [HttpGet]
        public async Task<IActionResult> ViewTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                // Fetch all Todo items from the database
                //var todos = await _context.Todos.Where(item => item.UserId == LogSesson.Id && item.Status == "In progress").ToListAsync();
                var todos = await _context.Todos
                            .Where(item => item.UserId == LogSesson.Id && item.Status == "In progress")
                            .OrderByDescending(item => item.Id)
                            .ToListAsync();
                //List<Todo> todoss = todos;
                if (!string.IsNullOrEmpty(searchString))
                {
                    todos = todos.Where(todo =>
                                            todo.Todoname.ToLower().Contains(searchString.ToLower()) ||
                                            todo.Status?.ToLower().Contains(searchString.ToLower()) == true ||
                                            todo.CreatedOn.ToString().ToLower().Contains(searchString.ToLower())
                                     ).ToList();
                }

                switch (sortField)
                {
                    case "Task":
                        todos = sortAscending ? todos.OrderBy(item => item.Todoname).ToList() :
                            todos.OrderByDescending(item => item.Todoname).ToList();
                        break;
                    case "Status":
                        todos = sortAscending ? todos.OrderBy(item => item.Status).ToList() :
                            todos.OrderByDescending(item => item.Status).ToList();
                        break;
                    case "CreatedOn":
                        todos = sortAscending ? todos.OrderBy(item => item.CreatedOn).ToList() :
                            todos.OrderByDescending(item => item.CreatedOn).ToList();
                        break;
                    default:
                        // Default sorting if no valid sort field provided (sort by Id by default)
                        todos = todos.OrderByDescending(item => item.Id).ToList();
                        break;
                }


                int tot_records = todos.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;

                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<Todo> todoss = todos.Skip(skip_records).Take(take_records).OrderByDescending(item => item.Id).ToList();



                TodoviewModel todoviewModel = new TodoviewModel()
                {
                    Todos = todoss,
                    Todo = null,

                };

                // Return the view with the list of Todo items
                return View(todoviewModel);

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddTodo(TodoviewModel newTodo)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                Todo todo = new Todo()
                {
                    CreatedOn = DateTime.Now,
                    Todoname = newTodo.Todo.Todoname,
                    Status = "In progress",
                    UserId = newTodo.Todo.UserId,
                    BrokerageId = LogSesson.BrokerageHouseName,
                };

                await _context.Todos.AddAsync(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewTodo");

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetTodo(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
                var statusOptions = new List<SelectListItem>
                {
                    new SelectListItem("In progress", "In progress"),
                    new SelectListItem("Done", "Done"),
                    new SelectListItem("Canceled", "Canceled")
                };

                ViewBag.StatusOptions = statusOptions;

                return View(todo);

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }

        [HttpPost]
        public async Task<IActionResult> Updatetodo(Todo model)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);

                var existingTodo = await _context.Todos.FindAsync(model.Id);
                if (existingTodo == null)
                {
                    return NotFound();
                }

                existingTodo.Todoname = model.Todoname;
                existingTodo.Status = model.Status;

                _context.Todos.Update(existingTodo);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewTodo");

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }

        [HttpGet]
        public async Task<IActionResult> AllTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                // Fetch all Todo items from the database
                var todos = await _context.Todos.Where(item => item.UserId == LogSesson.Id).ToListAsync();
                //List<Todo> todoss = todos;

                if (!string.IsNullOrEmpty(searchString))
                {
                    todos = todos.Where(todo =>
                                            todo.Todoname.ToLower().Contains(searchString.ToLower()) ||
                                            todo.Status?.ToLower().Contains(searchString.ToLower()) == true ||
                                            todo.CreatedOn.ToString().ToLower().Contains(searchString.ToLower())
                                     ).ToList();
                }

                switch (sortField)
                {
                    case "Task":
                        todos = sortAscending ? todos.OrderBy(item => item.Todoname).ToList() :
                            todos.OrderByDescending(item => item.Todoname).ToList();
                        break;
                    case "Status":
                        todos = sortAscending ? todos.OrderBy(item => item.Status).ToList() :
                            todos.OrderByDescending(item => item.Status).ToList();
                        break;
                    case "CreatedOn":
                        todos = sortAscending ? todos.OrderBy(item => item.CreatedOn).ToList() :
                            todos.OrderByDescending(item => item.CreatedOn).ToList();
                        break;
                    default:
                        // Default sorting if no valid sort field provided (sort by Id by default)
                        todos = todos.OrderByDescending(item => item.Id).ToList();
                        break;
                }


                int tot_records = todos.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;

                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<Todo> todoss = todos.Skip(skip_records).Take(take_records).ToList();



                TodoviewModel todoviewModel = new TodoviewModel()
                {
                    Todos = todoss,
                    Todo = null,

                };

                // Return the view with the list of Todo items
                return View(todoviewModel);

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpGet]
        public async Task<IActionResult> CompletedTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {

            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                // Fetch all Todo items from the database
                var todos = await _context.Todos.Where(item => item.UserId == LogSesson.Id && item.Status == "Done").ToListAsync();
                //List<Todo> todoss = todos;
                if (!string.IsNullOrEmpty(searchString))
                {
                    todos = todos.Where(todo =>
                                            todo.Todoname.ToLower().Contains(searchString.ToLower()) ||
                                            todo.Status?.ToLower().Contains(searchString.ToLower()) == true ||
                                            todo.CreatedOn.ToString().ToLower().Contains(searchString.ToLower())
                                     ).ToList();
                }

                switch (sortField)
                {
                    case "Task":
                        todos = sortAscending ? todos.OrderBy(item => item.Todoname).ToList() :
                            todos.OrderByDescending(item => item.Todoname).ToList();
                        break;
                    case "Status":
                        todos = sortAscending ? todos.OrderBy(item => item.Status).ToList() :
                            todos.OrderByDescending(item => item.Status).ToList();
                        break;
                    case "CreatedOn":
                        todos = sortAscending ? todos.OrderBy(item => item.CreatedOn).ToList() :
                            todos.OrderByDescending(item => item.CreatedOn).ToList();
                        break;
                    default:
                        // Default sorting if no valid sort field provided (sort by Id by default)
                        todos = todos.OrderByDescending(item => item.Id).ToList();
                        break;
                }


                int tot_records = todos.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;

                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<Todo> todoss = todos.Skip(skip_records).Take(take_records).ToList();



                TodoviewModel todoviewModel = new TodoviewModel()
                {
                    Todos = todoss,
                    Todo = null,

                };

                // Return the view with the list of Todo items
                return View(todoviewModel);

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }


        [HttpGet]
        public async Task<IActionResult> TodoReports()
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var supportEngineer = _context.Users.Where(i => i.Department == "Support Engineer").ToList();

                var reportView = new TodoReportView
                {
                    Todos = null,
                    ListofEmployee = supportEngineer,
                    search = null,
                    TodoHederInfo = null,
                    brokerages = null,

                };


                return View(reportView);
            }
            catch
            {

                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TodoSearch(TodoReportView reportView)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                if (reportView.search != null)
                {
                    // Check if FromDate is not null and is greater than or equal to the current date
                    if (reportView.search.FromDate.HasValue && reportView.search.FromDate >= DateTime.Now)
                    {
                        reportView.search.FromDate = DateTime.Now;
                    }

                    // Check if ToDate is not null and is greater than or equal to the current date
                    if (reportView.search.ToDate.HasValue && reportView.search.ToDate >= DateTime.Now)
                    {
                        reportView.search.ToDate = DateTime.Now;
                    }
                }

                var searchResults = _context.Todos
                    .Where(item => item.UserId == LogSesson.Id)
                    .OrderByDescending(item => item.Id)
                    .ToList();


                if (!String.IsNullOrEmpty(reportView.search.Status))
                    searchResults = searchResults.Where(x => x.Status.Contains(reportView.search.Status)).ToList();

                if ((reportView.search.FromDate != null) && (reportView.search.ToDate != null))
                    searchResults = searchResults.Where(x => (x.CreatedOn >= reportView.search.FromDate) && (x.CreatedOn <= reportView.search.ToDate)).ToList();

                string Brocaragename = GetBrocarageHouseName(LogSesson.BrokerageHouseName);
                string EmployeeNamee = LogSesson.FullName;



                int TotalTodo = searchResults.Count();
                int TotalInprogressTodo = searchResults.Where(x => x.Status == "In progress").Count();
                int TotalCompletedTodoo = searchResults.Where(x => x.Status == "Completed").Count();
                int TotalCancledTodo = TotalTodo - TotalInprogressTodo - TotalCompletedTodoo;


                TodoHederInfo SectionInfo = new TodoHederInfo
                {
                    BrokerageHouseName = Brocaragename,
                    EmployeeName = EmployeeNamee,
                    TotalTodo = TotalTodo,
                    TotalInprogress = TotalInprogressTodo,
                    TotalCanseled = TotalCancledTodo,
                    TotalCompleteTodo = TotalCompletedTodoo,
                    ReportName = "Admin"


                };

                // Populate the Issues property of the ReportView model with search results
                var reportViewWithSearchResults = new TodoReportView
                {
                    Todos = searchResults,
                    TodoHederInfo = SectionInfo,

                };

                // Return the partial view with the populated reportView model
                return PartialView("_todoSearchResult", reportViewWithSearchResults);

            }
            catch
            {

                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }


        [HttpGet]
        public async Task<IActionResult> ViewAllTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                // Fetch all Todo items from the database
                //var todos = await _context.Todos.Where(item => item.UserId == LogSesson.Id && item.Status == "In progress").ToListAsync();
                var todos = await _context.Todos
                            .OrderByDescending(item => item.Id)
                            .ToListAsync();
                //List<Todo> todoss = todos;
                if (!string.IsNullOrEmpty(searchString))
                {
                    todos = todos.Where(todo =>
                                            todo.Todoname.ToLower().Contains(searchString.ToLower()) ||
                                            todo.Status?.ToLower().Contains(searchString.ToLower()) == true ||
                                            todo.CreatedOn.ToString().ToLower().Contains(searchString.ToLower())
                                     ).ToList();
                }

                switch (sortField)
                {
                    case "Task":
                        todos = sortAscending ? todos.OrderBy(item => item.Todoname).ToList() :
                            todos.OrderByDescending(item => item.Todoname).ToList();
                        break;
                    case "Status":
                        todos = sortAscending ? todos.OrderBy(item => item.Status).ToList() :
                            todos.OrderByDescending(item => item.Status).ToList();
                        break;
                    case "CreatedOn":
                        todos = sortAscending ? todos.OrderBy(item => item.CreatedOn).ToList() :
                            todos.OrderByDescending(item => item.CreatedOn).ToList();
                        break;
                    default:
                        // Default sorting if no valid sort field provided (sort by Id by default)
                        todos = todos.OrderByDescending(item => item.Id).ToList();
                        break;
                }


                int tot_records = todos.Count;
                int pagesize = rowperpage > 0 ? rowperpage : 10;
                int number_of_button = 4;

                Pager P = new Pager(tot_records, page, pagesize, number_of_button, searchString);
                ViewBag.pager = P;
                int skip_records = (page - 1) * pagesize;
                int take_records = pagesize;
                List<Todo> todoss = todos.Skip(skip_records).Take(take_records).OrderByDescending(item => item.Id).ToList();



                TodoviewModel todoviewModel = new TodoviewModel()
                {
                    Todos = todoss,
                    Todo = null,

                };

                // Return the view with the list of Todo items
                return View(todoviewModel);

            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }


        [HttpGet]
        public async Task<IActionResult> AllTodoReports()
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var supportEngineer = _context.Users.ToList();
                var BrocarageHouse = _context.Brokerages.ToList();

                var reportView = new TodoReportView
                {
                    Todos = null,
                    ListofEmployee = supportEngineer,
                    search = null,
                    TodoHederInfo = null,
                    brokerages = BrocarageHouse,

                };


                return View(reportView);
            }
            catch
            {

                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        [HttpGet]
        public async Task<IActionResult> AllTodoSearch(TodoReportView reportView)
        {

            var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
            User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);


            if (reportView.search != null)
            {
                // Check if FromDate is not null and is greater than or equal to the current date
                if (reportView.search.FromDate.HasValue && reportView.search.FromDate >= DateTime.Now)
                {
                    reportView.search.FromDate = DateTime.Now;
                }

                // Check if ToDate is not null and is greater than or equal to the current date
                if (reportView.search.ToDate.HasValue && reportView.search.ToDate >= DateTime.Now)
                {
                    reportView.search.ToDate = DateTime.Now;
                }
            }

            var searchResults = await _context.Todos.ToListAsync();

            if ((reportView.search.FromDate != null) && (reportView.search.ToDate != null))
                searchResults = searchResults.Where(x => (x.CreatedOn >= reportView.search.FromDate) && (x.CreatedOn <= reportView.search.ToDate)).ToList();

            if (!String.IsNullOrEmpty(reportView.search.Status))
                searchResults = searchResults.Where(x => x.Status.Contains(reportView.search.Status)).ToList();

            if (reportView.search.BrocarageHouseName.HasValue)
                searchResults = searchResults.Where(x => x.BrokerageId == reportView.search.BrocarageHouseName).ToList();
            if (reportView.search.EmployeeName.HasValue)
                searchResults = searchResults.Where(x => x.UserId==reportView.search.EmployeeName).ToList();



            string Brocaragename = reportView.search.BrocarageHouseName !=0 ?GetBrocarageHouseName(reportView.search.BrocarageHouseName) :"All";
            string EmployeeNamee = (reportView.search.EmployeeName != null) ? GetEmployeeName(reportView.search.EmployeeName) : "All";




            int TotalTodo = searchResults.Count();
            int TotalInprogressTodo = searchResults.Where(x => x.Status == "In progress").Count();
            int TotalCompletedTodoo = searchResults.Where(x => x.Status == "Completed").Count();
            int TotalCancledTodo = TotalTodo - TotalInprogressTodo - TotalCompletedTodoo;


            TodoHederInfo SectionInfo = new TodoHederInfo
            {
                BrokerageHouseName = Brocaragename,
                EmployeeName = EmployeeNamee,
                TotalTodo = TotalTodo,
                TotalInprogress = TotalInprogressTodo,
                TotalCanseled = TotalCancledTodo,
                TotalCompleteTodo = TotalCompletedTodoo,
                ReportName = "Admin"


            };

            // Populate the Issues property of the ReportView model with search results
            var reportViewWithSearchResults = new TodoReportView
            {
                Todos = searchResults,
                TodoHederInfo = SectionInfo,

            };

            // Return the partial view with the populated reportView model
            return PartialView("_todoSearchResult", reportViewWithSearchResults);
        }







        public IActionResult Logout()
        {
             HttpContext.Session.Remove("AdminData");
            return RedirectToAction("Login", "RegisterLogin");
        }
        public async Task< IActionResult> Profile(int  id)
        {

            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == id);

                if (user == null)
                {
                    return NotFound(); // Or handle the case where the user is not found
                }

                var userView = new UserView
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhonNumber = user.PhonNumber,
                    Designation = user.Designation,
                    BrokerageHouseName = GetBrocarageHouseName(user.BrokerageHouseName),
                    Branch = GetBranchName(user.Branch),
                    EmployeeId = user.EmployeeId,
                    UserName = user.UserName,
                    UCatagory = user.UCatagory,
                    UType = user.UType,
                    UStatus = user.UStatus
                };


                return View(userView);
            }
            catch
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }



        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("AdminData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var pass = new Password
                {
                    UserId = LogSesson.Id
                };
                return View(pass);
            }
            catch
            {
                return RedirectToAction("Login", "RegisterLogin");
            }
        }



        [HttpPost]
        public async Task<IActionResult> ChangePassword(Password password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == password.UserId);
                if (user != null)
                {
                    if (!VerifyPasswordHash(password.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                    {
                        ViewBag.message = "Current Password is not Correct";
                        return View();
                    }
                    else
                    {
                        if (password.NewPassword != password.ConNewPassword)
                        {
                            ViewBag.message = "New Password and Confarm password are not Same";
                            return View();
                        }
                        else
                        {
                            CreatePasswordHash(password.NewPassword,
                                out byte[] passwordHash,
                                out byte[] passwordSalt);

                            user.PasswordHash = passwordHash;
                            user.PasswordSalt = passwordSalt;

                            _context.Users.Update(user);
                            _context.SaveChanges();
                            HttpContext.Session.Remove("AdminData");
                            return RedirectToAction("Login", "RegisterLogin");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("AdminData");
                return RedirectToAction("Login", "RegisterLogin");
            }
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







        private string GetBrocarageHouseName(int? id)
        {
            var Brocarage = _context.Brokerages.ToList();
            foreach (var item in Brocarage)
            {
                if(item.BrokerageId== id)
                {
                    return item.BrokerageHouseName;
                }
            }
            return null;
        }

        private string GetEmployeeName(int? id)
        {
            var user = _context.Users.ToList();
            foreach (var item in user)
            {
                if (item.Id == id)
                {
                    return item.FullName;
                }
            }
            return null;
        }

        private string GetBranchName(int id)
        {
            var Brocarage = _context.Branchhs.ToList();
            foreach (var item in Brocarage)
            {
                if (item.BrokerageId == id)
                {
                    return item.BranchName;
                }
            }
            return null;
        }


        private string UserName(int id)
        {
            var user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            return user.FullName;
        }
        private string SupportTypeName(int? id)
        {
            var user = _context.SupportTypes.Where(i => i.SupportTypeId == id).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            return user.SType;
        }



        private string SupportCatagoryName(int? id)
        {
            var user = _context.SupportCatagories.Where(i => i.SupportCatagoryId == id).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return user.SCatagory;
        }

        private string SupportSubCatagoryName(int? id)
        {
            var user = _context.SupportSubCatagories.Where(i => i.SupportSubCatagoryId == id).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            return user.SubCatagory;
        }

        private string AffectedSectionName(int? id)
        {
            var user = _context.AffectedSectionss.Where(i => i.AffectedSectionId == id).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            return user.ASection;
        }


    }
}
