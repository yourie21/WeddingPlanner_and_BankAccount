using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankAccount.Models{

  public class Wedding {
    [Key]
    public int id { get; set;}
    public int UserId { get; set;}
    public sqlUser user { get; set; }

    [Required(ErrorMessage="Bride name is required.")]
    public string person1  { get; set;}

    [Required(ErrorMessage="Groom name is required.")]
    public string person2 { get; set;}

    [Required(ErrorMessage="Address is required.")]
    public string address { get; set;}

    [Required(ErrorMessage="Wedding date entry is required.")]
    public DateTime date { get; set;}
    public List<RSVP> RSVPs  {get;set;}
    public Wedding() {
      RSVPs  = new List<RSVP>();
    }
  }
}