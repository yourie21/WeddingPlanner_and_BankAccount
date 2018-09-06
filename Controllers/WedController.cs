using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;

using Microsoft.AspNetCore.Http; //added
using Microsoft.EntityFrameworkCore; //added
// using Microsoft.AspNetCore.Identity; //addedfor hash

namespace BankAccount.Controllers
{
  public class WedController : Controller
  {
    private ReviewContext _context;
    public WedController(ReviewContext context) {
        _context = context;
    }
    // [HttpGet("wedding/dashboard")]
    // public IActionResult wedding(){
    // //only those logged in can view the wedding page
    //   if (HttpContext.Session.GetString("username") == null) return RedirectToAction("Index", "Home");
      
    //   return View(); 
    //   } 

    // [HttpGet("wedding/add")]
    // public IActionResult add(){ //asp-action name
    //   if (HttpContext.Session.GetString("username") == null) return RedirectToAction("Index", "Home");
    //   return View("add"); 
    // } 

    // [HttpPost("wedding/process")]
    // public IActionResult adding (Wedding newwed){ //asp-action name
    //   if (HttpContext.Session.GetString("username") == null) return RedirectToAction("Index", "Home");
    //   if (ModelState.IsValid) {
    //   // Save'em to SQL. creator id is a column in wedding, to delete its own created wedding
    //     newwed.CreatorId = (int)HttpContext.Session.GetInt32("userid"); 
    //     _context.weddingtable.Add(newwed);
    //     _context.SaveChanges();
    //     return RedirectToAction("account", "Home");
    //   }
    //   ViewBag.addingerror = "Entries were not valid.  Try again.";
    //   return View("add"); 
    // } 


    // [HttpGet("wedding/{id}")]
    // public IActionResult about(int id){
    //   if (HttpContext.Session.GetString("username") == null) return RedirectToAction("Index", "Home");
   

    //   return View("about");
    // }


  }
}