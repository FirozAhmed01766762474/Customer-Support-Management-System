using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XFLCSMS.Models.DataTable;

namespace XFLCSMS.Controllers
{
    public class DataTableController : Controller
    {
        private readonly DataContext _context;

        public DataTableController( DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var customerList = _context.Customers.ToList();
            

            return View(customerList);
            //return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create( Customer customer)
        {
             _context.AddAsync(customer);
             _context.SaveChangesAsync();
           
            return View("Index");
        }
        
        public ActionResult GetList()
        {

            //List<Customer> customerList = new List<Customer>();

            var customerList = _context.Customers.ToList();

            return new JsonResult(customerList);
        }
    }
}
