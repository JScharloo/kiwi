﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lisa.Kiwi.WebApi
{
    [Table("Reports")]
    public class ReportData
    {
        public ReportData()
        {
            StatusChanges = new List<StatusChangeData>();
            Created = DateTimeOffset.Now;
            IsVisible = true;
        }

        public int Id { get; set; }

        public string Category { get; set; }
        public bool IsVisible { get; set; }

        public DateTimeOffset Created { get; set; }

        public int Status { get; set; }

        public string Guid { get; set; }

        public string Description { get; set; }
        
        //First Aid
        public bool? IsUnconscious { get; set; }

        //Bullying
        public string Victim { get; set; }

        //Weapons
        public string WeaponType { get; set; }
        public string WeaponLocation { get; set; }

        //Fight
        public int? FighterCount { get; set; }
        public bool? IsWeaponPresent { get; set; }

        //Drugs
        public string DrugsAction { get; set; }

        //Theft
        public string StolenObject { get; set; }
        public DateTime? DateOfTheft { get; set; }

        public Guid EditToken { get; set; }

        
        public virtual ICollection<StatusChangeData> StatusChanges { get; set; }
        public virtual LocationData Location { get; set; }
        public virtual PerpetratorData Perpetrator { get; set; }
        public virtual ContactData Contact { get; set; }
        public virtual VehicleData Vehicle { get; set; }

    }
}