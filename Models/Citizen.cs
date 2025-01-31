﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_proc.Models
{
    public class Citizen : User
    {
        [Required(ErrorMessage = "Please enter Firstname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Lastname")]
        public string LastName { get; set; }    
        [StringLength(16, MinimumLength = 5, ErrorMessage = "Phone number is not validated")]
        public string Phone { get; set; }

        public string? CIN { get; set; }
        [JsonIgnore]
        public virtual ICollection<AskForInfo>? AskForInfo { get; set; }















        public Citizen()
        {this.createdAt=   new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();}

    }
}
