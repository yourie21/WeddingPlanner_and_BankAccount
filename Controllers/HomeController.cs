using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;

using Microsoft.AspNetCore.Http; //added
using Microsoft.EntityFrameworkCore; //added
using Microsoft.AspNetCore.Identity; //addedfor hash

namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        private ReviewContext _context;
        public HomeController(ReviewContext context) {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index() => View(); 

        [HttpPost("register")] //must be same as asp-action
        public IActionResult register (Userlogin newuser) {
            
            sqlUser validuser = _context.usertable.SingleOrDefault(a=>a.email == newuser.email); //.email is existing usertable email list
            if (validuser != null){ // != email is taken
                ViewBag.email = "This email is taken. Try another email.";
                return View("Index");
            }
            if (ModelState.IsValid != true && validuser != null){ //not valid (error msg will show) & taken email
                ViewBag.email = "This email is taken. Try another email.";
                return View("Index");
            } 
            if (ModelState.IsValid){
            // transfer the form user to sqluser model, which graps the table from the usertable. copy.name is the usertable's name.
                sqlUser copy = new sqlUser();
                copy.name = newuser.name;
                copy.lastname = newuser.lastname;
                copy.email = newuser.email;
                copy.pw = newuser.pw;
                copy.balance = 0.00;
            // hash the password of the copied user
                PasswordHasher<sqlUser> Hasher = new PasswordHasher<sqlUser>();
                copy.pw = Hasher.HashPassword(copy, copy.pw);

                HttpContext.Session.SetString("username", copy.name);
            // let's save them to sql
                _context.usertable.Add(copy);
                _context.SaveChanges();
            // make an id of a made user from usertable now
                sqlUser madeuser = _context.usertable.SingleOrDefault(a=>a.email == copy.email);
                HttpContext.Session.SetInt32("userid", madeuser.id);

                return RedirectToAction("result");
            }
            return View("Index");
        }

        [HttpPost("login")] 
        public IActionResult login (string email, string pw) { 
            sqlUser existinguser = _context.usertable.Where(a=>a.email == email).SingleOrDefault();
            if(existinguser != null && pw != null)  { //if that email exists
                PasswordHasher<sqlUser> Hasher = new PasswordHasher<sqlUser>(); //passwordhasher for sqluser
                if(0 != Hasher.VerifyHashedPassword(existinguser, existinguser.pw, pw)) { //if not 0 meaning password matched
                    HttpContext.Session.SetString("username", existinguser.name); 
                    HttpContext.Session.SetInt32("userid", existinguser.id); 
                    return RedirectToAction("result");
                } else {
                ViewBag.email2 = "Email is found but password does not match.";
                return View("Index");
                }
            } else {
            ViewBag.email2 = "Email is NOT found AND password does not match.";
            return View("Index");
            }
        }
        

        [HttpGet("result")]
        public IActionResult result() {
            List<sqlUser> Allusers = _context.usertable.OrderBy(o=>o.name).ToList(); //what is the reviews table name here for, and _context?       
            ViewBag.username = HttpContext.Session.GetString("username");
            return View(
                "result", Allusers);
        }

        [HttpGet ("account")]
        public IActionResult Account() {
            int? loggedinID = HttpContext.Session.GetInt32("userid"); //logged in user id
            if (loggedinID == null){ //not logged in, just in case this happens
                return RedirectToAction("Index"); 
            } else {
            // get all transaction from transaction table of the logged user, via user's id. user_id foreign key in transaction table    
                List<Transaction> alltrans = _context.transactiontable.Where(x=>x.UserId == loggedinID).ToList();
                ViewBag.alltrans = alltrans; 

            // logged in user's info, it's current balance is in user table. username can also be userrow.name, or just use session    
                sqlUser userrow = _context.usertable.SingleOrDefault(x=>x.id == loggedinID);
                TempData["balance"] = userrow.balance;
                TempData["username"] = HttpContext.Session.GetString("username");

                return View("account");
            }
        }
  
        [HttpPost ("completeTransaction")]
        public IActionResult completeTransaction(double _amount) {
            if (_amount != 0){
            int? loggedinID = HttpContext.Session.GetInt32("userid"); 
                if (loggedinID == null){
                return RedirectToAction("Index");
                }
        //copy the amount into a new sql transaction table, use the known id to transfer and current date
            Transaction newTrans = new Transaction {
                UserId = (int)loggedinID,
                amount = _amount, // from the parameter
                date = DateTime.Now
                };
        // add the _amount into the usertable's balance
            sqlUser userrow = _context.usertable.SingleOrDefault(x=>x.id == loggedinID);
            userrow.balance += _amount;
            // userrow.balance += (double)newTrans.amount;

            _context.Add(newTrans);
            _context.SaveChanges();
            return RedirectToAction("account");
            } else {
                return View("account");
            }
        }

        [HttpGet("logout")]
        public IActionResult logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

//// ********************************************************************************** ////

        [HttpGet("wedding")]
        public IActionResult wedding(){
        //only those logged in can view the wedding page
            if (HttpContext.Session.GetInt32("userid") == null) {
                // TempData["notlogged"] = "Must log in to view the wall";
                return RedirectToAction("Index");
            }
        // Get a user for name and id to match the creator's UserId for delete function
            sqlUser loggedinUser = _context.usertable.SingleOrDefault(x=>x.id == HttpContext.Session.GetInt32("userid"));
            ViewBag.loggedinUser = loggedinUser;
        // Get all RSVPs table from the Wedding class
            List<Wedding> allweddings = _context.weddingtable.Include(x=>x.RSVPs).ToList(); //.OrderBy(x=>x.date) 
            return View("wedding", allweddings); 
            } 
    ///
        [HttpGet("add")]
            public IActionResult add(){ //asp-action name
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
            return View("add"); 
    } 
    ///
        [HttpPost("process")]
        public IActionResult process (Wedding newwed){ //asp-action name
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
            if (ModelState.IsValid) {
                if(newwed.date < DateTime.Today){
                    ModelState.AddModelError("Date", "Date must be in the future.");
                    return View("add");
                }
            // Save'em to SQL. creator id is a column in wedding, to delete its own created wedding
                newwed.UserId = (int)HttpContext.Session.GetInt32("userid"); 
                _context.weddingtable.Add(newwed);
                _context.SaveChanges();
                // return RedirectToAction("wedding");

                //If I want it to go to the detail page, get the new wedding id after sql is saved, new{} is to pass id into About(id)parameter
                return RedirectToAction("about", new { id = newwed.id });
            }
            return View("add"); 
        } 


        [HttpGet("wedding/{id}")]
        public IActionResult about(int id){
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
    
        // { }  Both works. include RSVPs list from model, .theninclude to see values in RSVP, where id of wedding is the clicked id (to give one wedding obj)
            Wedding singlewed = _context.weddingtable.Include(w=>w.RSVPs).ThenInclude(RS=>RS.user).SingleOrDefault(w=>w.id == id);
        // [ { } ] // List to iterate @Model in foreach. but ViewBag.singlewed would've been easier. can loop just the second foreach without the first foreach
            List<Wedding> singlewed2 = _context.weddingtable.Include(w=>w.RSVPs).ThenInclude(RS=>RS.user).Where(w=>w.id == id).ToList();
            return View("about", singlewed2);
        }

     // Delete is only available to Creator(UserId from weddingtable) matching the loggedinID (in wedding.cshtml)
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id){
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
       
        // catch wedding's id from URL, get that wedding's row, remove it.
            Wedding onewedding = _context.weddingtable.SingleOrDefault(x=>x.id == id); 
        // catch related manttable RSVPtable's row that has the WeddingId and delete that row first
            List<RSVP> allRSVP = _context.RSVPtable.Where(x=>x.WeddingId==id).ToList();
            foreach (var row in allRSVP){    
                _context.RSVPtable.Remove(row);
            }
            _context.weddingtable.Remove(onewedding);
            _context.SaveChanges();
            return RedirectToAction("wedding");
        }
        
        [HttpGet("RSVP/{id}")]
        public IActionResult RSVP(int id){
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
        
        // Make rsvp row in RSVPtable by filling in the table's columns
            RSVP makeRsvp = new RSVP();
                makeRsvp.WeddingId = id;
                makeRsvp.UserId = (int)HttpContext.Session.GetInt32("userid");

            _context.RSVPtable.Add(makeRsvp);
            _context.SaveChanges();
            return RedirectToAction("wedding");
        }
        
        [HttpGet("unRSVP/{id}")]
        public IActionResult UNRSVP(int id){
            if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
        // Remove rsvp row in RSVPtable
            int? loggedinId = HttpContext.Session.GetInt32("userid");
            RSVP oneRsvp = _context.RSVPtable.SingleOrDefault(x=>x.WeddingId == id && x.UserId == loggedinId);

            _context.Remove(oneRsvp);
            _context.SaveChanges();
            return RedirectToAction("wedding");
        }


        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}
