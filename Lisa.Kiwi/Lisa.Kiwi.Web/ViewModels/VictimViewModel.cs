﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lisa.Kiwi.Web
{
    public class VictimViewModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredError)]
        [DisplayName("Geef hier een beschijving van het slachtoffer:")]
        public string Victim { get; set; }

        public bool HasVictim { get; set; }
    }
}