using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Cryptography;
using XFLCSMS.Models.Admin;
using XFLCSMS.Models.Common;
using XFLCSMS.Models.Issue;
using XFLCSMS.Models.Register;
using XFLCSMS.Models.Todos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace XFLCSMS.Controllers
{
    public class MakerController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MakerController(DataContext context, IWebHostEnvironment webHostEnvironment)
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
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                int UserID = LogSesson.Id;
                // Total Ticket
                int TotalInTicket = _context.Issues.Count(i => i.UserId == UserID && (i.UserId == UserID));
                int TotalInclosed = _context.Issues.Count(i => i.IStatus == "Close" && (i.UserId == UserID));
                int TotalInQueue = TotalInTicket - TotalInclosed;
                //Today
                int TodayTotal = _context.Issues.Count(i => (i.TDate.Date == DateTime.Now.Date) && (i.UserId == UserID));
                int TodayClose = _context.Issues.Count(i => (i.TDate.Date == DateTime.Now.Date) && (i.IStatus == "Close") && (i.UserId == UserID));
                int TodayQueue = TodayTotal - TodayClose;
                //Week
                DateTime lastWeekStartDate = DateTime.Now.Date.AddDays(-7);
                int LastWeekTotal = _context.Issues.Count(i => i.TDate.Date >= lastWeekStartDate && i.TDate.Date <= DateTime.Now.Date && i.UserId == UserID);
                int LastWeekClosed = _context.Issues.Count(i => i.TDate.Date >= lastWeekStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close" && (i.UserId == UserID));
                int lastWeekQueue = LastWeekTotal - LastWeekClosed;

                // Last month's total issues
                DateTime lastMonthStartDate = DateTime.Now.Date.AddMonths(-1);
                int LastMonthTotal = _context.Issues.Count(i => i.TDate.Date >= lastMonthStartDate && i.TDate.Date <= DateTime.Now.Date && (i.UserId == UserID));
                int LastMonthClosed = _context.Issues.Count(i => i.TDate.Date >= lastMonthStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close" && (i.UserId == UserID));
                int LastMonthQueue = LastMonthTotal - LastMonthClosed;

                // Last year's total issues
                DateTime lastYearStartDate = DateTime.Now.Date.AddYears(-1);
                int LastYearTotal = _context.Issues.Count(i => i.TDate.Date >= lastYearStartDate && i.TDate.Date <= DateTime.Now.Date && (i.UserId == UserID));
                int LastYearClosed = _context.Issues.Count(i => i.TDate.Date >= lastYearStartDate && i.TDate.Date <= DateTime.Now.Date && i.IStatus == "Close" && (i.UserId == UserID));
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        [HttpGet]
        public async Task<IActionResult> MackerTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;

                var TicketList = _context.Issues
                .Where(item => item.UserId == LogSesson.Id)
                .OrderByDescending(item => item.IssueId)
                .ToList();

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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }




        }

        [HttpGet]
        public async Task<IActionResult> Pagination(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;

                var TicketList = _context.Issues
                .Where(item => item.UserId == LogSesson.Id)
                .OrderByDescending(item => item.IssueId)
                .ToList();

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
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.TNumber).ToList() : TicketList.OrderByDescending(item => item.TNumber).ToList();
                        break;
                    case "Tickets":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.ITitle).ToList() : TicketList.OrderByDescending(item => item.ITitle).ToList();
                        break;
                    case "Approval Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.AssignBy).ToList() : TicketList.OrderByDescending(item => item.AssignBy).ToList();
                        break;
                    case "Priority":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.Priority).ToList() : TicketList.OrderByDescending(item => item.Priority).ToList();
                        break;
                    case "Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.IStatus).ToList() : TicketList.OrderByDescending(item => item.IStatus).ToList();
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }




        }

        public async Task<IActionResult> UnassignedTicketList(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                var TicketList = _context.Issues.OrderByDescending(i => i.IssueId).Where(i => i.AssignOn == null
                && i.AssignBy == null && i.UserId == LogSesson.Id).ToList();

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
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.TNumber).ToList() : TicketList.OrderByDescending(item => item.TNumber).ToList();
                        break;
                    case "Tickets":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.ITitle).ToList() : TicketList.OrderByDescending(item => item.ITitle).ToList();
                        break;
                    case "Approval Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.AssignBy).ToList() : TicketList.OrderByDescending(item => item.AssignBy).ToList();
                        break;
                    case "Priority":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.Priority).ToList() : TicketList.OrderByDescending(item => item.Priority).ToList();
                        break;
                    case "Status":
                        TicketList = sortAscending ? TicketList.OrderBy(item => item.IStatus).ToList() : TicketList.OrderByDescending(item => item.IStatus).ToList();
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        public async Task<IActionResult> ClosedTicketList(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                List<IssueTable> TicketList = await _context.Issues.OrderByDescending(i => i.IssueId).Where(i => i.IStatus == "Close"
                && i.UserId == LogSesson.Id).ToListAsync();
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> IssueRaiseFrom()
        {
            try
            {


                // Retrieve the JSON string from the session and deserialize it
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");

                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var errorMessage = TempData["ErrorMessage"] as string;
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    // Do something with the error message, e.g., pass it to the view
                    ViewBag.ErrorMessage = errorMessage;
                }

                var brocarage = await _context.Brokerages.ToListAsync();
                string[] acronymsArray = new string[brocarage.Count];
                //Dictionary<string, int> nameCount = new Dictionary<string, int>();
                int i = 0;
                foreach (var item in brocarage)
                {
                    string acronyme = item.BrokerageHouseAcronym;
                    acronymsArray[i] = acronyme;
                    i++;

                }

                var issueLoginInfo = new IssueLoginInfo
                {
                    UserId = LogSesson.Id,
                    BrocarageHouseName = CreateHouseName(LogSesson.BrokerageHouseName),
                    BranchName = CreateBranchName(LogSesson.Branch),
                    TicketID = GenerateTicketId(LogSesson.BrokerageHouseName)
                };

                var viewModel = new IssueViewModel
                {
                    SupportTypes = _context.SupportTypes.ToList(),
                    SupportCatagories = _context.SupportCatagories.ToList(),
                    SupportSubCatagories = _context.SupportSubCatagories.ToList(),
                    AffectedSections = _context.AffectedSectionss.ToList(),
                    Brokerages = _context.Brokerages.ToList(),
                    Branchhs = _context.Branchhs.ToList(),
                    LoginInfo = issueLoginInfo

                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueRaiseFrom(IssueViewModel issueViewModel, List<IFormFile> files)
        {
            try
            {

                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);


                var issue = new IssueTable
                {
                    TDate = issueViewModel.issueFrom.dateTime,
                    TNumber = issueViewModel.issueFrom.TicketId,

                    Priority = issueViewModel.issueFrom.Priority,
                    Details = issueViewModel.issueFrom.IssueDetails,
                    Comments = issueViewModel.issueFrom.Commands,
                    UserId = issueViewModel.issueFrom.UserId,
                    SupportCatagoryId = issueViewModel.issueFrom.SupportCatagoryId,
                    SupportTypeId = issueViewModel.issueFrom.SupportTypeId,
                    SupportSubCatagoryId = issueViewModel.issueFrom.SupportSubCatagoryID,
                    AffectedSectionId = issueViewModel.issueFrom.AffectedSectionId,
                    ITitle = issueViewModel.issueFrom.ITitle,
                    IStatus = "Open",
                    AssignOn = null,
                    AssignBy = null,

                    BrokerageId = CreateBrocarageID(issueViewModel.issueFrom.BrocarageHouseName)
                };
                _context.Issues.Add(issue);
                _context.SaveChanges();
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
                        IssueId = issue.IssueId
                    };

                    _context.Attachments.Add(attachment);


                }

                await _context.SaveChangesAsync();

                return RedirectToAction("MackerTicketList");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> TicketView(int? id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
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
            catch (Exception ex)
            {
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        public async Task<IActionResult> EditTicket(int id)
        {

            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var issueWithAttachments = _context.Issues
                    .Include(i => i.attachment)  // Include attachments in the query
                    .FirstOrDefault(i => i.IssueId == id);
                var SupportEng = _context.Users.Where(i => i.Designation == "Support Engineer").ToList();

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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicketttt(MakerView makerView, List<IFormFile> files)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var issue = await _context.Issues.FirstOrDefaultAsync(a => a.IssueId == makerView.IssueId);
                if (issue != null)
                {
                    issue.ITitle = makerView.IssueTitle;
                    issue.Details = makerView.TicketDetails;
                    issue.Comments = makerView.Command;
                    issue.Priority = makerView.Priority;
                    issue.UpdatedOn = DateTime.Now;
                    issue.UpdatedBy = LogSesson.FullName;
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


                    return RedirectToAction("MackerTicketList", "Maker");
                }


                return NotFound("Edit is not done");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");

            }
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var reportView = new ReportView
                {
                    Issues = null,

                };


                return View(reportView);
            }
            catch
            {

                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Search(ReportView reportView)
        {
            var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
            User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
            if (reportView.MakerSearch.FromDate >= DateTime.Now)
            {
                reportView.SESearch.FromDate = DateTime.Now;
            }
            if (reportView.MakerSearch.ToDate >= DateTime.Now)
            {
                reportView.SESearch.ToDate = DateTime.Now;
            }

            var searchResults = _context.Issues
                .Where(item => item.UserId == LogSesson.Id)
                .OrderByDescending(item => item.IssueId)
                .ToList();


            if (!String.IsNullOrEmpty(reportView.MakerSearch.Priority))
                searchResults = searchResults.Where(x => x.Priority.Contains(reportView.MakerSearch.Priority)).ToList();

            if (!String.IsNullOrEmpty(reportView.MakerSearch.AStatus))
                searchResults = searchResults.Where(x => x.IStatus.Contains(reportView.MakerSearch.AStatus)).ToList();

            if ((reportView.MakerSearch.FromDate != null) && (reportView.MakerSearch.ToDate != null))
                searchResults = searchResults.Where(x => (x.TDate >= reportView.MakerSearch.FromDate) && (x.TDate <= reportView.MakerSearch.ToDate)).ToList();

            string Brocaragename = GetBrocarageHouseName(LogSesson.BrokerageHouseName);
            string EmployeeNamee = LogSesson.FullName;



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
                ReportName = "Maker"


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
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MakerData");
            return RedirectToAction("Login", "RegisterLogin");
        }

        [HttpGet]
        public async Task<IActionResult> TodoReports()
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var reportView = new TodoReportView
                {
                    Todos = null,
                    ListofEmployee = null,
                    search = null,
                    TodoHederInfo = null,
                    brokerages = null,

                };


                return View(reportView);
            }
            catch
            {

                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TodoSearch(TodoReportView reportView)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                    ReportName = "Maker"


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

                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }


        public async Task<IActionResult> Profile(int id)
        {

            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                            HttpContext.Session.Remove("MakerData");
                            return RedirectToAction("Login", "RegisterLogin");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("MakerData");
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

        [HttpGet]
        public async Task<IActionResult> ViewTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }


        [HttpGet]
        public async Task<IActionResult> AllTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpGet]
        public async Task<IActionResult> CompletedTodo(int page, int rowperpage, string? searchString = null, string? sortField = null, bool sortAscending = true)
        {

            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }


        [HttpPost]
        public async Task<IActionResult> AddTodo(TodoviewModel newTodo)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetTodo(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
                return RedirectToAction("Login", "RegisterLogin");
            }


        }

        [HttpPost]
        public async Task<IActionResult> Updatetodo(Todo model)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("MakerData");
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
                HttpContext.Session.Remove("MakerData");
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
                if (item.BrokerageId == id)
                {
                    return item.BrokerageHouseName;
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
        private string CreateBranchName(int id)
        {
            var Brocaragessss = _context.Branchhs.ToList();
            foreach (var item in Brocaragessss)
            {
                if (item.BranchId == id)
                {
                    string BranchName = item.BranchName;
                    return BranchName;
                }

            }
            return null;

        }

        private string CreateHouseName(int id)
        {
            var Brocaragessss = _context.Brokerages.ToList();
            foreach (var item in Brocaragessss)
            {
                if (item.BrokerageId == id)
                {
                    string HouseName = item.BrokerageHouseName;
                    return HouseName;
                }

            }
            return null;

        }

        private int CreateBrocarageID(string brocarageName)
        {
            var Brocaragessss = _context.Brokerages.ToList();
            foreach (var item in Brocaragessss)
            {
                if (item.BrokerageHouseName == brocarageName)
                {
                    return item.BrokerageId;
                }

            }
            return 0;

        }

        private string GenerateTicketId(int id)
        {
            var Brocaragessss = _context.Brokerages.ToList();
            foreach (var item in Brocaragessss)
            {

                if (item.BrokerageId == id)
                {
                    var testData = _context.Issues.ToList();
                    int max = 0000000;
                    //Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    foreach (var item2 in testData)
                    {
                        string name = item2.TNumber.Split('_')[0];
                        int value = Int32.Parse(item2.TNumber.Split('_')[1]);

                        if (name == item.BrokerageHouseAcronym)
                        {
                            if (value > max)
                                max = value;
                        }

                    }

                    max = max + 1;
                    string formattedNumber = max.ToString("D7");
                    string ticketID = item.BrokerageHouseAcronym + '_' + formattedNumber;
                    return ticketID;
                }

            }


            return null;
        }
    }
}
