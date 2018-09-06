using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankAccount.Models{

  public class RSVP {
    [Key]
    public int id { get; set; }

    public int WeddingId { get; set; }
    public Wedding wedding { get; set; }

    public int UserId { get; set; }
    public sqlUser user { get; set; }
  }
}