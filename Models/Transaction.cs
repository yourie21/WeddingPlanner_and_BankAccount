using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BankAccount.Models{

  public class Transaction {
    
    public int id{get;set;}
    public double? amount{get;set;}
    public DateTime date{get;set;}
    
    public int UserId {get;set;}
    public sqlUser user {get;set;}

  }
}