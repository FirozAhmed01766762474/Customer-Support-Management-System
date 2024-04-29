using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Cryptography;
using XFLCSMS.Models.Admin;
using XFLCSMS.Models.Common;
using XFLCSMS.Models.Issue;
using XFLCSMS.Models.Register;

namespace XFLCSMS.Controllers
{
    public class SupportEngineerController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SupportEngineerController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public int EditIssueId = 0;


        public async Task<IActionResult> Dashbord()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
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
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> AllTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;

                var TicketList = _context.Issues.OrderByDescending(i => i.IssueId).ToList();

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
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");  
            }
        }

        public async Task<IActionResult> AssignedTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;

                List<IssueTable> TicketList = _context.Issues.Where(i => i.AssignBy == LogSesson.FullName &&(i.IStatus !="Close"))
                    .OrderByDescending(i => i.IssueId).ToList();

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
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }
        public async Task<IActionResult> UnassignedTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                if (page <= 0) { page = 1; }

                ViewBag.CurrentSortField = sortField;
                ViewBag.CurrentSortAscending = sortAscending;
                List<IssueTable> TicketList = _context.Issues.OrderByDescending(i => i.IssueId)
                    .Where(i => i.AssignOn == null && i.AssignBy == null).ToList();
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
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        public async Task<IActionResult> ClosedTicketList(int page, int rowperpage, string? searchString = null, string?
            sortField = null, bool sortAscending = true)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
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
            catch
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }

        }
        public async Task<IActionResult> TicketView(int? id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
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
            catch 
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> EditTicket(int id)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;
                var issueWithAttachments = _context.Issues
                    .Include(i => i.attachment)  // Include attachments in the query
                    .FirstOrDefault(i => i.IssueId == id);
                var SupportEng = _context.Users.Where(i => i.Department == "Support Engineer").ToList();

                if (issueWithAttachments == null)
                {
                    return NotFound();
                }
                EditIssueId = issueWithAttachments.IssueId;

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
            catch 
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicketttt(MakerView makerView, List<IFormFile> files)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
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
                    if (issue.AssignBy != null)
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


                    return RedirectToAction("AllTicketList", "SupportEngineer");
                }


                return NotFound("Edit is not done");
            }
            catch 
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public async Task<IActionResult> IssueRaiseFrom()
        {
            try
            {
                // Retrieve the JSON string from the session and deserialize it
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

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
            catch 
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueRaiseFrom(IssueViewModel issueViewModel, List<IFormFile> files)
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                var issue = new IssueTable
                {
                    TDate = issueViewModel.issueFrom.dateTime,
                    TNumber = issueViewModel.issueFrom.TicketId,

                    Priority = issueViewModel.issueFrom.Priority,
                    Details = issueViewModel.issueFrom.IssueDetails.ToString(),
                    Comments = issueViewModel.issueFrom.Commands.ToString(),
                    UserId = issueViewModel.issueFrom.UserId,
                    SupportCatagoryId = issueViewModel.issueFrom.SupportCatagoryId,
                    SupportTypeId = issueViewModel.issueFrom.SupportTypeId,
                    SupportSubCatagoryId = issueViewModel.issueFrom.SupportSubCatagoryID,
                    AffectedSectionId = issueViewModel.issueFrom.AffectedSectionId,
                    ITitle = issueViewModel.issueFrom.ITitle,
                    //AffectedSectionId= null,
                    //SupportCatagoryId= null,
                    //SupportSubCatagoryId=null,
                    //SupportTypeId = null,

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

                return RedirectToAction("AllTicketList");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("SEData");
            return RedirectToAction("Login", "RegisterLogin");
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
                return String.Empty;
            }
            return user.SType;
        }


        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
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
                            ViewBag.message = "New Password and Confarm Password are not Same";
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
                            HttpContext.Session.Remove("SEData");
                            return RedirectToAction("Login", "RegisterLogin");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("SEData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            try
            {


                var jsonStringFromSession = HttpContext.Session.GetString("SEData");
                User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
                ViewBag.Profile = LogSesson;

                var supportEngineer = _context.Users.Where(i => i.Department == "Support Engineer").Select(user => user.FullName).ToList();
                var BrocarageHouse = _context.Brokerages.ToList();


                var reportview = new ReportView
                {
                    EmployeeNames = supportEngineer,
                    brocarages = BrocarageHouse,


                };

                return View(reportview);
            }
            catch
            {

                HttpContext.Session.Remove("SMData");
                return RedirectToAction("Login", "RegisterLogin");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Search(ReportView reportView)
        {
            var jsonStringFromSession = HttpContext.Session.GetString("SEData");
            User LogSesson = JsonConvert.DeserializeObject<User>(jsonStringFromSession);
            if (reportView.SESearch.ToDate >= DateTime.Now)
            {
                reportView.SESearch.ToDate = DateTime.Now;
            }

            var searchResults = await _context.Issues.ToListAsync();
            searchResults = searchResults.Where(x => (x.ClosedBy == LogSesson.FullName)||(x.AssignBy==LogSesson.FullName)).ToList();

            if (reportView.SESearch.BrokerageId.HasValue)
                searchResults = searchResults.Where(x => x.BrokerageId == reportView.SESearch.BrokerageId).ToList();
            if (!String.IsNullOrEmpty(reportView.SESearch.Priority))
                searchResults = searchResults.Where(x => x.Priority.Contains(reportView.SESearch.Priority)).ToList();
                
            if ((reportView.SESearch.FromDate != null) && (reportView.SESearch.ToDate != null))
                searchResults = searchResults.Where(x => (x.TDate >= reportView.SESearch.FromDate) && (x.TDate <= reportView.SESearch.ToDate)).ToList();

            string Brocaragename = (reportView.SESearch.BrokerageId > 0) ? GetBrocarageHouseName(reportView.SESearch.BrokerageId) : "All";
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
                ReportName = "Support Engineer"


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
        private string SupportCatagoryName(int? id)
        {
            var user = _context.SupportCatagories.Where(i => i.SupportCatagoryId == id).FirstOrDefault();
            if (user == null)
            {
                return String.Empty;
            }

            return user.SCatagory;
        }

        private string SupportSubCatagoryName(int? id)
        {
            var user = _context.SupportSubCatagories.Where(i => i.SupportSubCatagoryId == id).FirstOrDefault();
            if (user == null)
            {
                return String.Empty;
            }
            return user.SubCatagory;
        }

        private string AffectedSectionName(int? id)
        {
            var user = _context.AffectedSectionss.Where(i => i.AffectedSectionId == id).FirstOrDefault();
            if (user == null)
            {
                return String.Empty;
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
            return String.Empty;

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
            return String.Empty;

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
