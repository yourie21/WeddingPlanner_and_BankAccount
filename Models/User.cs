using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // added

using System.Linq;
using System.Threading.Tasks;

namespace BankAccount.Models {

  public class sqlUser {
    [Key]
    public int id {get; set;}
    public string name { get; set;}
    public string lastname { get; set;}
    public string email { get; set;}
    public string pw { get; set;}
    public double balance{get;set;}
    public List<RSVP> RSVPs  {get;set;}
    public sqlUser() {
      RSVPs  = new List<RSVP>();
    }
  }
}